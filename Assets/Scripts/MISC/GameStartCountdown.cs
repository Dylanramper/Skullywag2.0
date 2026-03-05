using System.Collections;
using UnityEngine;
using TMPro;

public class GameStartCountdown : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public float countdownTime = 5f;

    public GameObject player;
    private bool gameStarted = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(StartCountdown());
        gameStarted = false;
    }

    IEnumerator StartCountdown()
    {
        float timeRemaining = countdownTime;

        //Disable PlayerMovement and Shooting scripts
        player.GetComponent<PlayerMovement>().enabled = false;
        //player.GetComponent<PlayerCannons>().enabled = false;

        while (timeRemaining > 0)
        {
            countdownText.text = Mathf.Ceil(timeRemaining).ToString();
            yield return new WaitForSeconds(1f);
            timeRemaining--;
        }

        countdownText.text = "TAKE SAIL!";
        yield return new WaitForSeconds(1f);

        countdownText.gameObject.SetActive(false);

        //Enable Player Movement and Shooting scripts
        player.GetComponent<PlayerMovement>().enabled = true;
        //player.GetComponent<PlayerCannons>().enabled = true;

        gameStarted = true;
    }
}
