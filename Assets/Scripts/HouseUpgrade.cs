using UnityEngine;
using TMPro;
using DefaultNamespace;

public class HouseUpgrade : MonoBehaviour
{
    public GameObject[] upgradeObjects; // 0 - базовий, 1-3 - апгрейди
    public float[] moneyPerSecondLevels; // 0-3: для кожного рівня
    public int[] upgradePrices; // Ціна для кожного апгрейду
    public MoneyController moneyController;
    public TMP_Text priceText; // Текст для відображення ціни
    public AudioManager audioManager; // For playing sounds

    private int currentLevel = 0; // 0 - базовий, 1-3 - апгрейди

    private void Start()
    {
        // Відновлення рівня збереженого будинку
        currentLevel = PlayerPrefs.GetInt(gameObject.name + "_level", 0);
        UpdateHouseVisuals();
        UpdatePriceText();

        if (currentLevel >= 3)
        {
            gameObject.SetActive(false); // Якщо максимальний рівень, приховати об'єкт
        }

        // Subscribe to the interaction event
        EventManager.Instance.onInteraction.AddListener(OnInteraction);
    }

    private void OnDestroy()
    {
        // Unsubscribe when destroyed
        if (EventManager.Instance != null)
            EventManager.Instance.onInteraction.RemoveListener(OnInteraction);
    }

    private void OnInteraction(GameObject interactingObject)
    {
        // Only respond if this specific object is being interacted with
        if (interactingObject != this.gameObject && !interactingObject.transform.IsChildOf(this.transform))
        {
            // Not interacting with this specific object, so return without doing anything
            return;
        }

        UpgradeHouse();
    }

    private void UpgradeHouse()
    {
        if (currentLevel >= 3) return; // Максимальний рівень

        if (moneyController != null && moneyController.SubtractMoney(upgradePrices[currentLevel]))
        {
            currentLevel++;
            if (currentLevel > 3)
            {
                gameObject.SetActive(false);
            }
            PlayerPrefs.SetInt(gameObject.name + "_level", currentLevel);
            PlayerPrefs.Save();
            Debug.Log(gameObject.name + "_level"+ " = " + currentLevel);

            UpdateHouseVisuals();
            UpdatePriceText();
            moneyController.AddMoneyPerSecond(moneyPerSecondLevels[currentLevel]);
            
            // Play sound if audio manager is available
            if (audioManager != null)
                audioManager.PlaySound(3);
                
            // Unlock corresponding expedition if available
            if (ExpeditionManager.Instance != null)
            {
                ExpeditionManager.Instance.OnHousePurchased(gameObject.name);
            }
        }
    }

    private void UpdateHouseVisuals()
    {
        // Show or hide objects based on level progression
        for (int i = 0; i < upgradeObjects.Length; i++)
        {
            // Show this level if it's the current level or a previous upgrade
            upgradeObjects[i].SetActive(i <= currentLevel);
        }
    }

    private void UpdatePriceText()
    {
        if (priceText != null)
        {
            if (currentLevel < 3)
            {
                priceText.text = upgradePrices[currentLevel].ToString() + "$";
            }
            else
            {
                priceText.text = "MAX";
            }
        }
    }
}