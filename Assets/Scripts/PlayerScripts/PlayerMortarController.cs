
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
    [SerializeField] private GameObject homingRocketPrefab;
    [SerializeField] private GameObject targetReticlePrefab;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private ItemInventory itemInventory;

    [Header("Targeting UI")]
    [SerializeField] private GameObject targetingPrompt;
    [SerializeField] private GameObject homingTargetPrompt;

    [Header("Homing Rocket Camera Zoom")]
    [SerializeField] private float zoomOutAmount = 1.5f;
    [SerializeField] private float zoomDuration = 0.25f;

    private enum TargetingType
    {
        None,
        Mortar,
        ChainShot,
        HomingRocket
    }

    private TargetingType currentTargetingType = TargetingType.None;

    private bool isTargeting;
    private bool waitingForRelease;
    private GameObject activeReticle;

    private float originalCameraSize;
    private Coroutine cameraZoomCoroutine;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (itemInventory == null)
            itemInventory = FindAnyObjectByType<ItemInventory>();

        if (targetingPrompt != null)
            targetingPrompt.SetActive(true);

        if (homingTargetPrompt != null)
            homingTargetPrompt.SetActive(false);

        if (mainCamera != null && mainCamera.orthographic)
            originalCameraSize = mainCamera.orthographicSize;
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

    // Mortar Shot targeting
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

    // Chain Shot targeting
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

    // Homing Rocket targeting
    public void BeginHomingRocketTargeting()
    {
        if (isTargeting)
            return;

        if (itemInventory == null ||
            itemInventory.GetHomingRocketCount() <= 0)
        {
            Debug.Log("No Homing Rockets available.");
            return;
        }

        if (mortarFirePoint == null ||
            homingRocketPrefab == null ||
            mainCamera == null)
        {
            Debug.LogError(
                "PlayerMortarController is missing required Homing Rocket references.");
            return;
        }

        if (!mainCamera.orthographic)
        {
            Debug.LogError(
                "Homing Rocket zoom currently requires an orthographic camera.");
            return;
        }

        if (homingRocketPrefab.GetComponent<HomingRocketProjectile>() == null)
        {
            Debug.LogError(
                "Homing Rocket prefab needs a HomingRocketProjectile component.");
            return;
        }

        originalCameraSize = mainCamera.orthographicSize;

        currentTargetingType = TargetingType.HomingRocket;
        StartTargeting();

        if (homingTargetPrompt != null)
            homingTargetPrompt.SetActive(true);

        SetCameraZoom(originalCameraSize + zoomOutAmount);

        Debug.Log("Tap an enemy ship to lock on with the Homing Rocket.");
    }

    private void StartTargeting()
    {
        isTargeting = true;
        waitingForRelease = true;

        if (targetingPrompt != null)
            targetingPrompt.SetActive(false);
    }

    private void TrySelectTarget(Vector2 screenPosition, bool isTouch)
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

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(
            new Vector3(
                screenPosition.x,
                screenPosition.y,
                -mainCamera.transform.position.z));

        Vector2 targetPosition = worldPosition;

        TargetingType selectedType = currentTargetingType;

        // Homing Rocket requires an actual enemy.
        if (selectedType == TargetingType.HomingRocket)
        {
            Transform enemyTarget = FindEnemyAtPosition(targetPosition);

            // Keep targeting active if the player taps empty water.
            if (enemyTarget == null)
            {
                Debug.Log("Tap directly on an enemy ship to lock on.");
                return;
            }

            CancelTargeting();

            if (!itemInventory.ConsumeHomingRocket())
                return;

            GameObject rocketObject = Instantiate(
                homingRocketPrefab,
                mortarFirePoint.position,
                mortarFirePoint.rotation);

            HomingRocketProjectile rocket =
                rocketObject.GetComponent<HomingRocketProjectile>();

            if (rocket == null)
            {
                Debug.LogError(
                    "Homing Rocket prefab needs HomingRocketProjectile.");
                Destroy(rocketObject);
                return;
            }

            rocket.SetTarget(enemyTarget);

            Debug.Log("Homing Rocket launched!");
            return;
        }

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
                    "Chain Shot prefab needs a ChainShotProjectile.");
                Destroy(projectile);
                return;
            }

            chainShot.Launch(mortarFirePoint.position, targetPosition);
            return;
        }

        // Mortar Shot path
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

        mortar.Launch(mortarFirePoint.position, targetPosition);

        StartCoroutine(RemoveReticleAfterTravel(mortar.travelTime));
    }

    private Transform FindEnemyAtPosition(Vector2 position)
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(position);

        foreach (Collider2D hit in hits)
        {
            EnemyShip enemy = hit.GetComponentInParent<EnemyShip>();

            if (enemy != null)
                return enemy.transform;

            BaseBoss boss = hit.GetComponentInParent<BaseBoss>();

            if (boss != null)
                return boss.transform;
        }

        return null;
    }

    private void SetCameraZoom(float targetSize)
    {
        if (mainCamera == null || !mainCamera.orthographic)
            return;

        if (cameraZoomCoroutine != null)
            StopCoroutine(cameraZoomCoroutine);

        cameraZoomCoroutine = StartCoroutine(
            AnimateCameraZoom(targetSize));
    }

    private IEnumerator AnimateCameraZoom(float targetSize)
    {
        float startSize = mainCamera.orthographicSize;
        float elapsed = 0f;

        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;

            float t = zoomDuration > 0f
                ? Mathf.Clamp01(elapsed / zoomDuration)
                : 1f;

            mainCamera.orthographicSize =
                Mathf.Lerp(startSize, targetSize, t);

            yield return null;
        }

        mainCamera.orthographicSize = targetSize;
        cameraZoomCoroutine = null;
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
        bool wasHomingRocket =
            currentTargetingType == TargetingType.HomingRocket;

        isTargeting = false;
        waitingForRelease = false;
        currentTargetingType = TargetingType.None;

        if (targetingPrompt != null)
            targetingPrompt.SetActive(true);

        if (homingTargetPrompt != null)
            homingTargetPrompt.SetActive(false);

        if (wasHomingRocket && mainCamera != null &&
            mainCamera.orthographic)
        {
            SetCameraZoom(originalCameraSize);
        }
    }
}
