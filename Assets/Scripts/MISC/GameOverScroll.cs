using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameOverScroll : MonoBehaviour
{
    [Header("Paper Images")]
    [SerializeField] private Image paperLeft;
    [SerializeField] private Image paperRight;

    [Header("Scroll Sides")]
    [SerializeField] private RectTransform scrollLeft;
    [SerializeField] private RectTransform scrollRight;

    [Header("Animation Settings")]
    [SerializeField] private float openDuration = 0.8f;
    [SerializeField] private float scrollSideMoveDistance = 200f;

    private Vector2 scrollLeftStart;
    private Vector2 scrollRightStart;

    private void Awake()
    {
        scrollLeftStart = scrollLeft.anchoredPosition;
        scrollRightStart = scrollRight.anchoredPosition;

        gameObject.SetActive(false);
        paperLeft.fillAmount = 0f;
        paperRight.fillAmount = 0f;
    }

    public void ShowGameOver()
    {
        gameObject.SetActive(true);
        StartCoroutine(OpenScrollAnimation());
    }

    private IEnumerator OpenScrollAnimation()
    {
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

        paperLeft.fillAmount = 1f;
        paperRight.fillAmount = 1f;
        scrollLeft.anchoredPosition = new Vector2(scrollLeftStart.x, scrollLeftStart.y);
        scrollRight.anchoredPosition = new Vector2(scrollRightStart.x, scrollRightStart.y);
    }
}
