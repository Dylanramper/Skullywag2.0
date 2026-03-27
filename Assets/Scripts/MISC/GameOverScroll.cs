using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameOverScroll : MonoBehaviour
{
    [SerializeField] private RectTransform scrollPanel;

    [Header("Paper Images")]
    [SerializeField] private Image paperLeft;
    [SerializeField] private Image paperRight;
    [SerializeField] private GameObject Buttons;

    [Header("Scroll Sides")]
    [SerializeField] private RectTransform scrollLeft;
    [SerializeField] private GameObject scrollCenter;
    [SerializeField] private RectTransform scrollRight;
    [SerializeField] private GameObject scrollLeftGO;
    [SerializeField] private GameObject scrollRightGO;

    [Header("Animation Settings")]
    [SerializeField] private float openDuration = 0.8f;

    [Header("Slide Animation")]
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private float startYOffset = -800f;
    [SerializeField] private float bounceHeight = 40f;
    [SerializeField] private float bounceDuration = 0.15f;

    private Vector2 panelTargetPos;

    private Vector2 scrollLeftStart;
    private Vector2 scrollRightStart;

    private void Awake()
    {
        scrollCenter.SetActive(true);
        scrollLeftGO.SetActive(false);
        scrollRightGO.SetActive(false);
        Buttons.SetActive(false);
        scrollLeftStart = scrollLeft.anchoredPosition;
        scrollRightStart = scrollRight.anchoredPosition;

        gameObject.SetActive(false);
        paperLeft.fillAmount = 0f;
        paperRight.fillAmount = 0f;

        panelTargetPos = scrollPanel.anchoredPosition;

        scrollPanel.anchoredPosition = panelTargetPos + Vector2.up * startYOffset;
    }

    public void CloseMenu(System.Action onComplete)
    {
        StartCoroutine(CloseSequence(onComplete));
    }

    public void ShowScroll()
    {
        gameObject.SetActive(true);

        // Reset position so it slides in properly every time
        scrollPanel.anchoredPosition = panelTargetPos + Vector2.up * startYOffset;

        StartCoroutine(GameOverSequence());
    }

    public void ShowGameOver()
    {
        gameObject.SetActive(true);
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator CloseSequence(System.Action onComplete)
    {
        // Hide buttons immediately
        Buttons.SetActive(false);

        yield return StartCoroutine(CloseScrollAnimation());
        yield return StartCoroutine(SlideOut());
        onComplete?.Invoke();
        scrollCenter.SetActive(true);
        scrollLeftGO.SetActive(false);
        scrollRightGO.SetActive(false);
        Buttons.SetActive(false);
        scrollLeftStart = scrollLeft.anchoredPosition;
        scrollRightStart = scrollRight.anchoredPosition;

        gameObject.SetActive(false);
        paperLeft.fillAmount = 0f;
        paperRight.fillAmount = 0f;
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }

    private IEnumerator GameOverSequence()
    {
        yield return StartCoroutine(SlideIn());
        yield return StartCoroutine(OpenScrollAnimation());
    }

    private IEnumerator SlideIn()
    {
        float elapsed = 0f;

        Vector2 startPos = scrollPanel.anchoredPosition;
        Vector2 overshootPos = panelTargetPos + Vector2.up * bounceHeight;

        while(elapsed < slideDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / slideDuration);

            scrollPanel.anchoredPosition = Vector2.Lerp(startPos, overshootPos, t);

            yield return null;
        }

        //small bounce
        elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / bounceDuration);

            scrollPanel.anchoredPosition = Vector2.Lerp(overshootPos, panelTargetPos, t);

            yield return null;
        }
        scrollCenter.SetActive(false);
        scrollPanel.anchoredPosition = panelTargetPos;
    }

    private IEnumerator SlideOut()
    {
        float elapsed = 0f;

        Vector2 startPos = scrollPanel.anchoredPosition;
        Vector2 endPos = panelTargetPos + Vector2.up * startYOffset * 1.5f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / slideDuration);

            scrollPanel.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            yield return null;
        }

    }

    private IEnumerator OpenScrollAnimation()
    {
        scrollLeftGO.SetActive(true);
        scrollRightGO.SetActive(true);

        float elapsed = 0f;

        while (elapsed < openDuration)
        {
            elapsed += Time.unscaledDeltaTime; //Unscaled if game is paused
            float t = Mathf.Clamp01(elapsed / openDuration);

            //Paper fills from center
            paperLeft.fillAmount = t;
            paperRight.fillAmount = t;

            //Scroll sides move outward
            scrollLeft.anchoredPosition = new Vector2(scrollLeftStart.x * t, scrollLeftStart.y);
            scrollRight.anchoredPosition = new Vector2(scrollRightStart.x * t, scrollRightStart.y);

            yield return null;
        }
        Buttons.SetActive(true);
        paperLeft.fillAmount = 1f;
        paperRight.fillAmount = 1f;
        scrollLeft.anchoredPosition = new Vector2(scrollLeftStart.x, scrollLeftStart.y);
        scrollRight.anchoredPosition = new Vector2(scrollRightStart.x, scrollRightStart.y);
    }

    private IEnumerator CloseScrollAnimation()
    {
        float elapsed = 0f;
        //AudioManager (Page Turn)
        while (elapsed < openDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = 1 - (elapsed / openDuration); // REVERSE

            paperLeft.fillAmount = t;
            paperRight.fillAmount = t;

            scrollLeft.anchoredPosition = new Vector2(scrollLeftStart.x * t, scrollLeftStart.y);
            scrollRight.anchoredPosition = new Vector2(scrollRightStart.x * t, scrollRightStart.y);

            yield return null;
        }

        paperLeft.fillAmount = 0f;
        paperRight.fillAmount = 0f;

        scrollLeft.anchoredPosition = new Vector2(scrollLeftStart.x, scrollLeftStart.y);
        scrollRight.anchoredPosition = new Vector2(scrollRightStart.x, scrollRightStart.y);

        scrollLeftGO.SetActive(false);
        scrollRightGO.SetActive(false);
        scrollCenter.SetActive(true);
    }
}
