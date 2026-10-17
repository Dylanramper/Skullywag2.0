
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMortarController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform mortarFirePoint;
    [SerializeField] private GameObject mortarProjectilePrefab;
    [SerializeField] private GameObject chainShotProjectilePrefab;
    [SerializeField] private GameObject targetReticlePrefab;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private ItemInventory itemInventory;

    [Header("Targeting UI")]
    [SerializeField] private GameObject targetingPrompt;

    private enum TargetingType
    {
        None,
        Mortar,
        ChainShot
    }

    private TargetingType currentTargetingType = TargetingType.None;

    private bool isTargeting;
    private bool waitingForRelease;
    private GameObject activeReticle;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (itemInventory == null)
            itemInventory = FindAnyObjectByType<ItemInventory>();

        if (targetingPrompt != null)
            targetingPrompt.SetActive(true);
    }

    private void Update()
    {
        if (!isTargeting)
            return;

        if (waitingForRelease)
        {
            bool mouseReleased =
                Mouse.current == null ||
                !Mouse.current.leftButton.isPressed;

            bool touchReleased =
                Touchscreen.current == null ||
                !Touchscreen.current.primaryTouch.press.isPressed;

            if (mouseReleased && touchReleased)
                waitingForRelease = false;

            return;
        }

        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Vector2 touchPosition =
                Touchscreen.current.primaryTouch.position.ReadValue();

            TrySelectTarget(touchPosition, true);
            return;
        }

        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            TrySelectTarget(
                Mouse.current.position.ReadValue(), false);
        }
    }

    // Existing Mortar Shot targeting
    public void BeginTargeting()
    {
        if (isTargeting)
            return;

        if (itemInventory == null ||
            itemInventory.GetMortarShotCount() <= 0)
        {
            Debug.Log("No Mortar Shots available.");
            return;
        }

        if (mortarFirePoint == null ||
            mortarProjectilePrefab == null ||
            targetReticlePrefab == null ||
            mainCamera == null)
        {
            Debug.LogError(
                "PlayerMortarController is missing required Mortar references.");
            return;
        }

        currentTargetingType = TargetingType.Mortar;
        StartTargeting();

        Debug.Log("Tap a location to fire the Mortar Shot.");
    }

    // New Chain Shot targeting
    public void BeginChainShotTargeting()
    {
        if (isTargeting)
            return;

        if (itemInventory == null ||
            itemInventory.GetChainShotCount() <= 0)
        {
            Debug.Log("No Chain Shots available.");
            return;
        }

        if (mortarFirePoint == null ||
            chainShotProjectilePrefab == null ||
            mainCamera == null)
        {
            Debug.LogError(
                "PlayerMortarController is missing required Chain Shot references.");
            return;
        }

        if (chainShotProjectilePrefab.GetComponent<ChainShotProjectile>() == null)
        {
            Debug.LogError(
                "Chain Shot prefab needs a ChainShotProjectile component.");
            return;
        }

        currentTargetingType = TargetingType.ChainShot;
        StartTargeting();

        Debug.Log("Tap a location to fire the Chain Shot.");
    }

    private void StartTargeting()
    {
        isTargeting = true;
        waitingForRelease = true;

        if (targetingPrompt != null)
            targetingPrompt.SetActive(false);
    }

    private void TrySelectTarget(
        Vector2 screenPosition, bool isTouch)
    {
        if (EventSystem.current != null)
        {
            bool overUI = isTouch
                ? EventSystem.current.IsPointerOverGameObject(
                    Touchscreen.current.primaryTouch.touchId.ReadValue())
                : EventSystem.current.IsPointerOverGameObject();

            if (overUI)
                return;
        }

        Vector3 worldPosition =
            mainCamera.ScreenToWorldPoint(
                new Vector3(
                    screenPosition.x,
                    screenPosition.y,
                    -mainCamera.transform.position.z));

        Vector2 targetPosition = worldPosition;

        // Save the selected type before CancelTargeting resets it.
        TargetingType selectedType = currentTargetingType;

        CancelTargeting();

        // Chain Shot path
        if (selectedType == TargetingType.ChainShot)
        {
            if (!itemInventory.ConsumeChainShot())
                return;

            GameObject projectile = Instantiate(
                chainShotProjectilePrefab,
                mortarFirePoint.position,
                Quaternion.identity);

            ChainShotProjectile chainShot =
                projectile.GetComponent<ChainShotProjectile>();

            if (chainShot == null)
            {
                Debug.LogError(
                    "Chain Shot prefab needs ChainShotProjectile.");
                Destroy(projectile);
                return;
            }

            chainShot.Launch(
                mortarFirePoint.position,
                targetPosition);

            return;
        }

        // Existing Mortar Shot path
        if (selectedType != TargetingType.Mortar)
            return;

        if (!itemInventory.ConsumeMortarShot())
            return;

        activeReticle = Instantiate(
            targetReticlePrefab,
            targetPosition,
            Quaternion.identity);

        GameObject mortarProjectile = Instantiate(
            mortarProjectilePrefab,
            mortarFirePoint.position,
            Quaternion.identity);

        MortarProjectile mortar =
            mortarProjectile.GetComponent<MortarProjectile>();

        if (mortar == null)
        {
            Debug.LogError(
                "Mortar projectile prefab needs MortarProjectile.");
            Destroy(mortarProjectile);
            Destroy(activeReticle);
            activeReticle = null;
            return;
        }

        mortar.Launch(
            mortarFirePoint.position,
            targetPosition);

        StartCoroutine(RemoveReticleAfterTravel(
            mortar.travelTime));
    }

    private IEnumerator RemoveReticleAfterTravel(float travelTime)
    {
        yield return new WaitForSeconds(travelTime);

        if (activeReticle != null)
            Destroy(activeReticle);

        activeReticle = null;
    }

    public void CancelTargeting()
    {
        isTargeting = false;
        waitingForRelease = false;
        currentTargetingType = TargetingType.None;

        if (targetingPrompt != null)
            targetingPrompt.SetActive(true);
    }
}
