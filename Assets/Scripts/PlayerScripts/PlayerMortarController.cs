
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerMortarController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform mortarFirePoint;
    [SerializeField] private GameObject mortarProjectilePrefab;
    [SerializeField] private GameObject targetReticlePrefab;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private ItemInventory itemInventory;

    [Header("Targeting UI")]
    [SerializeField] private GameObject targetingPrompt;

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
                "PlayerMortarController is missing required references.");
            return;
        }

        isTargeting = true;
        waitingForRelease = true;

        if (targetingPrompt != null)
            targetingPrompt.SetActive(false);

        Debug.Log("Tap a location to fire the Mortar Shot.");
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
                new Vector3(screenPosition.x,
                            screenPosition.y,
                            -mainCamera.transform.position.z));

        Vector2 targetPosition = worldPosition;

        CancelTargeting();

        if (!itemInventory.ConsumeMortarShot())
            return;

        activeReticle = Instantiate(
            targetReticlePrefab,
            targetPosition,
            Quaternion.identity);

        GameObject projectile = Instantiate(
            mortarProjectilePrefab,
            mortarFirePoint.position,
            Quaternion.identity);

        MortarProjectile mortar =
            projectile.GetComponent<MortarProjectile>();

        if (mortar == null)
        {
            Debug.LogError(
                "Mortar projectile prefab needs MortarProjectile.");
            Destroy(projectile);
            Destroy(activeReticle);
            activeReticle = null;
            return;
        }

        mortar.Launch(mortarFirePoint.position, targetPosition);

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

        if (targetingPrompt != null)
            targetingPrompt.SetActive(true);
    }
}
