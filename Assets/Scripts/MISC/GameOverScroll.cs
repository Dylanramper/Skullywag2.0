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

    public void ShowGameOver()
    {
        gameObject.SetActive(true);
        StartCoroutine(GameOverSequence());
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
}
