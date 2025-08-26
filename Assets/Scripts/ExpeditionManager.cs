using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpeditionManager : MonoBehaviour
{
    [System.Serializable]
    public class HouseExpeditionPair
    {
        public string houseId; // ID or name of the house
        public Expedition linkedExpedition; // Reference to expedition object
    }

    [Header("Expedition Settings")]
    [SerializeField] private List<HouseExpeditionPair> expeditionMappings = new List<HouseExpeditionPair>();
    
    [Header("UI Settings")]
    [SerializeField] private GameObject noExpeditionsObject;
    
    private static ExpeditionManager _instance;
    public static ExpeditionManager Instance { get { return _instance; } }
    
    private const string ANY_EXPEDITION_PURCHASED_KEY = "AnyExpeditionPurchased";
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    
    private void Start()
    {
        UpdateNoExpeditionsObjectVisibility();
    }
    
    private void UpdateNoExpeditionsObjectVisibility()
    {
        if (noExpeditionsObject != null)
        {
            bool anyExpeditionPurchased = PlayerPrefs.GetInt(ANY_EXPEDITION_PURCHASED_KEY, 0) == 1;
            noExpeditionsObject.SetActive(!anyExpeditionPurchased);
        }
    }
    // Call this method when a house is purchased
    public void OnHousePurchased(string houseId)
    {
        bool foundMatch = false;

        foreach (HouseExpeditionPair pair in expeditionMappings)
        {
            if (pair.houseId == houseId)
            {
                if (pair.linkedExpedition != null)
                {
                    pair.linkedExpedition.UnlockExpedition();
                    
                    // Mark that at least one expedition has been purchased
                    PlayerPrefs.SetInt(ANY_EXPEDITION_PURCHASED_KEY, 1);
                    PlayerPrefs.Save();
                    
                    // Hide the "no expeditions" object permanently
                    UpdateNoExpeditionsObjectVisibility();
                    
                    foundMatch = true;
                    break;
                }
                else
                {
                    foundMatch = true;
                    break;
                }
            }
        }

        if (!foundMatch)
        {
            Debug.LogWarning("No expedition mapping found for house: " + houseId);
        }
    }
    
    // Helper method to get all expedition mappings
    public List<HouseExpeditionPair> GetExpeditionMappings()
    {
        return expeditionMappings;
    }
    
    // Helper method to add a new mapping in runtime if needed
    public void AddExpeditionMapping(string houseId, Expedition expedition)
    {
        HouseExpeditionPair newPair = new HouseExpeditionPair
        {
            houseId = houseId,
            linkedExpedition = expedition
        };
        
        expeditionMappings.Add(newPair);
    }
    
    public void ResetExpeditionPurchases()
    {
        PlayerPrefs.DeleteKey(ANY_EXPEDITION_PURCHASED_KEY);
        PlayerPrefs.Save();
        UpdateNoExpeditionsObjectVisibility();
    }
}
