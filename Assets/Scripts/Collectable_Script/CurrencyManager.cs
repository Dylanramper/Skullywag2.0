using UnityEngine;
using TMPro;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    [SerializeField]
    private TextMeshProUGUI coinText;

    private int currentCoins = 0;

    private const string CoinsKey = "PlayerCoins";

    private void Awake()
    {
        //Don't destroy this object when loading new scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadCoins();
    }

    //Add coins to player's total
    public void AddCoins(int amount)
    {
        if (amount <= 0)
            return;

        currentCoins += amount;

        SaveCoins();
        UpdateCoinUI();

        Debug.Log($"Added {amount} coins. Total coins: {currentCoins}");
    }

    //Remove coins form player's total, returns true if successful, false if not enough coins
    public bool SpendCoins(int amount)
    {
        if(amount <= 0)
            return false;

        if(currentCoins < amount)
        {
            Debug.Log("Not Enough Coins!");
            return false;
        }

        currentCoins -= amount;

        SaveCoins();
        UpdateCoinUI();

        Debug.Log($"Spent {amount} coins. Total coins: {currentCoins}");

        return true;
    }

    //Returns an amount that is grater than the required cost
    public bool HasEnoughCoins(int amount)
    {
        return currentCoins >= amount;
    }

    public int GetCoins()
    {
        return currentCoins;
    }

    //Save system for coins
    private void SaveCoins()
    {
        PlayerPrefs.SetInt(CoinsKey, currentCoins);
        PlayerPrefs.Save();
    }

    private void LoadCoins()
    {
        currentCoins = PlayerPrefs.GetInt(CoinsKey, 0);

        UpdateCoinUI();

        Debug.Log($"Loaded {currentCoins} coins.");
    }

    private void UpdateCoinUI()
    {
        if(coinText != null)
        {
            coinText.text = currentCoins.ToString();
        }
    }
}
