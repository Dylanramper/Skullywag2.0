using TMPro;
using UnityEngine;

public class CoinCounterUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;

    private void Start()
    {
        UpdateCoinText();
    }

    private void Update()
    {
        if (CurrencyManager.Instance != null)
        {
            UpdateCoinText();
        }
    }

    private void UpdateCoinText()
    {
        coinText.text = CurrencyManager.Instance.GetCoins().ToString();
    }
}