using System.Collections;
using UnityEngine;
using TMPro;

public class GameStartCountdown : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public float countdownTime = 5f;
    public GameObject pauseButton;
    public GameObject player;
    [SerializeField] PauseManager manager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(StartCountdown());
        pauseButton.SetActive(false);
    }

    IEnumerator AnimatePop()
    {
        countdownText.transform.localScale = Vector3.one * 1.5f;

        float timer = 0f;
        float duration = 0.2f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            countdownText.transform.localScale = Vector3.Lerp(Vector3.one * 1.5f, Vector3.one, t);

            yield return null;

        }

        countdownText.transform.localScale = Vector3.one;
    }

    IEnumerator StartCountdown()
    {
        float timeRemaining = countdownTime;

        //Disable PlayerMovement and Shooting scripts
        player.GetComponent<PlayerMovement>().enabled = false;

        while (timeRemaining > 0)
        {
            countdownText.text = Mathf.Ceil(timeRemaining).ToString();
            yield return new WaitForSeconds(1f);
            timeRemaining--;
            StartCoroutine(AnimatePop());
            AudioManager.Instance.PlayTextSFX();
            manager.textCountdownActive = true;
        }

        countdownText.text = "TAKE SAIL!";
        yield return new WaitForSeconds(1f);

        countdownText.gameObject.SetActive(false);

        //Enable Player Movement and Shooting scripts
        player.GetComponent<PlayerMovement>().enabled = true;
        manager.textCountdownActive = false;
        pauseButton.SetActive(true);
    }
}
