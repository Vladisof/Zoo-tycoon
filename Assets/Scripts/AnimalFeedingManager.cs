using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnimalFeedingManager : MonoBehaviour
{
    [System.Serializable]
    public class Animal
    {
        public string animalName;
        public Transform animalPoint;
        [HideInInspector] public bool isHungry = false;
        [HideInInspector] public float nextHungryTime = 0f;
        [HideInInspector] public bool isPurchased = false;
    }

    public GameObject FoneForText;
    public AudioManager audioManager;
    public List<Animal> animals;
    public float minHungryInterval = 1f; // 5 хвилин
    public float maxHungryInterval = 10f; // 10 хвилин
    public Transform foodPickupPoint;
    public TextMeshProUGUI hungryText;
    public GameObject foodOnPlayer; // Об'єкт, який показує, що гравець має корм
    public MoneyController moneyController; // Reference to MoneyController
    public TextMeshProUGUI currentMinusText; // UI text to show current deduction
    private float currentMoneyDeduction = 0f; // Track current deduction per second

    private bool playerHasFood = false;

    void Start()
    {
        currentMoneyDeduction = 0f; // Reset deduction on start
        LoadPurchasedAnimals(); // Load purchased animals first
        StartCoroutine(HungryCheckRoutine());
    }

    void ScheduleNextHungry(Animal animal)
    {
        if (!animal.isPurchased) return;
    
        animal.nextHungryTime = Time.time + Random.Range(minHungryInterval, maxHungryInterval);
        animal.isHungry = false;
    }

    void UpdateMoneyDeduction()
    {
        int hungryAnimalsCount = 0;

        foreach (var animal in animals)
        {
            if (animal.isPurchased && animal.isHungry)
            {
                hungryAnimalsCount++;
            }
        }

        float newDeduction = hungryAnimalsCount * 2f;

        // Only update if the deduction amount changed
        if (newDeduction != currentMoneyDeduction)
        {
            // Remove the previous deduction and add the new one
            moneyController.AddMoneyPerSecond(currentMoneyDeduction); // Add back previous deduction (remove it)
            moneyController.AddMoneyPerSecond(-newDeduction); // Apply new deduction
        
            currentMoneyDeduction = newDeduction;
        }

        // Update UI text
        if (currentMinusText != null)
        {
            if (currentMoneyDeduction > 0)
            {
                currentMinusText.text = $"-{currentMoneyDeduction:F1}";
                currentMinusText.gameObject.SetActive(true);
            }
            else
            {
                currentMinusText.gameObject.SetActive(false);
            }
        }
    }
    IEnumerator HungryCheckRoutine()
    {
        while (true)
        {
            bool anyHungry = false;
            float previousDeduction = currentMoneyDeduction;
        
            foreach (var animal in animals)
            {
                if (!animal.isPurchased) continue;

                if (!animal.isHungry && Time.time >= animal.nextHungryTime)
                {
                    animal.isHungry = true;
                }
                if (animal.isHungry)
                {
                    hungryText.text = $"{animal.animalName} wants to eat";
                    hungryText.gameObject.SetActive(true);
                    FoneForText.SetActive(true);
                    anyHungry = true;
                    break;
                }
            }
        
            if (!anyHungry)
            {
                hungryText.text = "";
                hungryText.gameObject.SetActive(false);
                FoneForText.SetActive(false);
            }
        
            // Update money deduction only if it changed
            UpdateMoneyDeduction();
        
            yield return new WaitForSeconds(1f);
        }
    }
    
    // Call this method when player purchases an animal
    public void PurchaseAnimal(string animalName)
    {
        foreach (var animal in animals)
        {
            if (animal.animalName == animalName)
            {
                Debug.Log($"Purchasing animal: {animal.animalName}");
                animal.isPurchased = true;
                SavePurchasedAnimals(); // Save the purchased state
                ScheduleNextHungry(animal); // Start hunger cycle for newly purchased animal
                break;
            }
        }
    }

    // Викликається, коли гравець підходить до foodPickupPoint і бере корм
    public void PickupFood()
    {
        audioManager.PlaySound(4);
        playerHasFood = true;
        foodOnPlayer.SetActive(true); // Показує, що гравець має корм
    }

    // Викликається, коли гравець підходить до тварини і намагається погодувати
    public void TryFeedAnimal(Transform animalTransform)
    {
        if (!playerHasFood) return;

        foreach (var animal in animals)
        {
            if (animal.animalPoint == animalTransform && animal.isHungry)
            {
                animal.isHungry = false;
                ScheduleNextHungry(animal);
                playerHasFood = false;
                foodOnPlayer.SetActive(false);
                audioManager.PlaySound(5);
                hungryText.text = "";

                // Update money deduction after feeding
                UpdateMoneyDeduction();
                SavePurchasedAnimals(); // Save after feeding
                break;
            }
        }
    }
    
    void SavePurchasedAnimals()
    {
        for (int i = 0; i < animals.Count; i++)
        {
            PlayerPrefs.SetInt($"Animal_{i}_Purchased", animals[i].isPurchased ? 1 : 0);
            PlayerPrefs.SetInt($"Animal_{i}_Hungry", animals[i].isHungry ? 1 : 0);
            PlayerPrefs.SetFloat($"Animal_{i}_NextHungryTime", animals[i].nextHungryTime);
        }
        PlayerPrefs.Save();
    }
    
    void LoadPurchasedAnimals()
    {
        for (int i = 0; i < animals.Count; i++)
        {
            animals[i].isPurchased = PlayerPrefs.GetInt($"Animal_{i}_Purchased", 0) == 1;
            animals[i].isHungry = PlayerPrefs.GetInt($"Animal_{i}_Hungry", 0) == 1;
            animals[i].nextHungryTime = PlayerPrefs.GetFloat($"Animal_{i}_NextHungryTime", 0f);
        
            if (animals[i].isPurchased && !animals[i].isHungry)
            {
                // If animal is purchased but not hungry, make sure it has a valid next hungry time
                if (animals[i].nextHungryTime <= Time.time)
                {
                    ScheduleNextHungry(animals[i]);
                }
            }
        }
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            ResetMoneyDeduction();
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            ResetMoneyDeduction();
        }
    }

    void OnDestroy()
    {
        ResetMoneyDeduction();
    }

    void ResetMoneyDeduction()
    {
        if (currentMoneyDeduction > 0)
        {
            moneyController.AddMoneyPerSecond(currentMoneyDeduction); // Return the deducted amount
            currentMoneyDeduction = 0f;
        }
        SavePurchasedAnimals(); // Save current animal states before exit
    }
}