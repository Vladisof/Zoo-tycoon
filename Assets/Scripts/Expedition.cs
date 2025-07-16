using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Expedition : MonoBehaviour
{
    [Header("Expedition Settings")]
    [SerializeField] private float expeditionTime = 60f; // Default time 60 seconds
    [SerializeField] private int rewardAmount = 100; // Default reward 100
    [SerializeField] private Button startButton;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject expeditionCompleteEffect;
    [SerializeField] private MoneyController moneyController; // Reference to your MoneyController
    [SerializeField] private GameObject rewardPrefab; // Префаб, який може з'явитися
    [SerializeField] private Transform rewardSpawnPoint; // Точка появи префабу
    [SerializeField] private GameObject startObject;
    [SerializeField] private GameObject winPL;
    private string rewardKey => $"Expedition_{expeditionId}_RewardPrefab";
    
    private bool isExpeditionActive = false;
    private float currentTime = 0f;
    private string expeditionId;
    private DateTime expeditionEndTime;
    
    // Keys for PlayerPrefs
    private string ActiveKey => $"Expedition_{expeditionId}_Active";
    private string EndTimeKey => $"Expedition_{expeditionId}_EndTime";
    private string DurationKey => $"Expedition_{expeditionId}_Duration";
    private string RewardKey => $"Expedition_{expeditionId}_Reward";
    private string UnlockedKey => $"Expedition_{expeditionId}_Unlocked";
    
    private void Awake()
    {
        // Generate a unique ID for this expedition based on position/name if not set
        if (string.IsNullOrEmpty(expeditionId))
        {
            expeditionId = gameObject.name + "_" + transform.position.ToString();
        }
    }
    
    private void Start()
    {
        // Відновлення префабу при перезапуску гри
        if (PlayerPrefs.GetInt(rewardKey, 0) == 1)
        {
            Instantiate(rewardPrefab, rewardSpawnPoint.position, Quaternion.identity);
        }
        // Find MoneyController if not assigned
        if (moneyController == null)
        {
            moneyController = FindObjectOfType<MoneyController>();
        }
        
        // Check if this expedition was unlocked before
        bool wasUnlocked = PlayerPrefs.GetInt(UnlockedKey, 0) == 1;
        
        // If it was unlocked, make it active now
        if (wasUnlocked)
        {
            gameObject.SetActive(true);
            Debug.Log($"Expedition {expeditionId} is active and was previously unlocked.");
        }
        
        
        if (startButton != null)
        {
            startButton.onClick.AddListener(StartExpedition);
        }
        
        // Check if expedition was active when game closed
        isExpeditionActive = PlayerPrefs.GetInt(ActiveKey, 0) == 1;
        
        if (isExpeditionActive)
        {
            // Get stored end time
            string endTimeString = PlayerPrefs.GetString(EndTimeKey, "");
            
            if (!string.IsNullOrEmpty(endTimeString))
            {
                // Parse the stored end time
                DateTime endTime = DateTime.Parse(endTimeString);
                DateTime now = DateTime.Now;
                
                // Calculate remaining time
                TimeSpan remainingTime = endTime - now;
                
                if (remainingTime.TotalSeconds <= 0)
                {
                    // Expedition is complete
                    CompleteExpedition();
                }
                else
                {
                    // Continue expedition with remaining time
                    currentTime = (float)remainingTime.TotalSeconds;
                    startButton.interactable = false;
                }
            }
        }
        
        UpdateTimerDisplay();
        
        startObject.gameObject.SetActive(false);
    }
    
    private void Update()
    {
        if (isExpeditionActive)
        {
            // Calculate remaining time based on real-world time
            TimeSpan remainingTime = expeditionEndTime - DateTime.Now;
            currentTime = (float)remainingTime.TotalSeconds;
        
            UpdateTimerDisplay();
        
            if (currentTime <= 0)
            {
                CompleteExpedition();
            }
        }
    }
    
    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
    
    public void StartExpedition()
    {
        if (!isExpeditionActive)
        {
            isExpeditionActive = true;
            currentTime = expeditionTime;
            startButton.interactable = false;

            // Calculate and store end time
            expeditionEndTime = DateTime.Now.AddSeconds(expeditionTime);

            // Save expedition state
            PlayerPrefs.SetInt(ActiveKey, 1);
            PlayerPrefs.SetString(EndTimeKey, expeditionEndTime.ToString());
            PlayerPrefs.SetFloat(DurationKey, expeditionTime);
            PlayerPrefs.SetInt(RewardKey, rewardAmount);
            PlayerPrefs.Save();
        }
    }
    
    private void CompleteExpedition()
    {
        isExpeditionActive = false;

        // Додавання нагороди гравцю
        if (moneyController != null)
        {
            moneyController.AddMoney(rewardAmount);
        }

        // Логіка появи префабу з шансом
        float chance = UnityEngine.Random.Range(0f, 1f); // Генерація випадкового числа
        if (chance <= 0.05f) // Шанс 5%
        {
            Debug.Log("Префаб з'явився!");
            winPL.SetActive(true);
            moneyController.AddMoneyPerSecond(1);
            Instantiate(rewardPrefab, rewardSpawnPoint.position, Quaternion.identity);
            PlayerPrefs.SetInt(rewardKey, 1); // Збереження стану отриманого префабу
            PlayerPrefs.Save();
        }

        // Ефект завершення експедиції
        if (expeditionCompleteEffect != null)
        {
            expeditionCompleteEffect.SetActive(true);
            StartCoroutine(HideEffectAfterDelay(2f));
        }

        // Скидання кнопки
        startButton.interactable = true;
        timerText.text = "";

        // Очищення стану експедиції
        PlayerPrefs.SetInt(ActiveKey, 0);
        PlayerPrefs.DeleteKey(EndTimeKey);
        PlayerPrefs.Save();
    }
    
    private IEnumerator HideEffectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (expeditionCompleteEffect != null)
        {
            expeditionCompleteEffect.SetActive(false);
        }
    }
    
    // Called by the house purchase system to activate this expedition
    public void UnlockExpedition()
    {
        gameObject.SetActive(true);

        // Save unlocked state
        PlayerPrefs.SetInt(UnlockedKey, 1);
        PlayerPrefs.Save();
        Debug.Log($"Expedition unlocked and saved with key {UnlockedKey}");
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && isExpeditionActive)
        {
            // Game is being paused, save the current state
            DateTime endTime = DateTime.Now.AddSeconds(currentTime);
            PlayerPrefs.SetString(EndTimeKey, endTime.ToString());
            PlayerPrefs.Save();
        }
    }
    
    private void OnApplicationQuit()
    {
        if (isExpeditionActive)
        {
            // Game is quitting, save the current state
            DateTime endTime = DateTime.Now.AddSeconds(currentTime);
            PlayerPrefs.SetString(EndTimeKey, endTime.ToString());
            PlayerPrefs.Save();
        }
    }
    
    // Helper methods to get/set properties
    public float GetExpeditionTime()
    {
        return expeditionTime;
    }
    
    public void SetExpeditionTime(float time)
    {
        expeditionTime = time;
    }
    
    public int GetRewardAmount()
    {
        return rewardAmount;
    }
    
    public void SetRewardAmount(int amount)
    {
        rewardAmount = amount;
    }
    
    public void SetExpeditionId(string id)
    {
        expeditionId = id;
    }
    
    public string GetExpeditionId()
    {
        return expeditionId;
    }
}
