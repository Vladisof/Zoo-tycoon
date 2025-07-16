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
    
    private static ExpeditionManager _instance;
    public static ExpeditionManager Instance { get { return _instance; } }
    
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
}
