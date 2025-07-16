using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class Shop : MonoBehaviour
{
    public GameObject purschaisePanel;
    public MoneyController GameManager;
    public AudioManager audioManager;
    public TextMeshProUGUI rewardText;
    public TextMeshProUGUI money1Text;
    public string coins1000 = "com.onezoomanager.singlecoins";
    

    public void OnPurchaseComplete(Product product)
    {
        Debug.Log("Покупка прошла успешно");
        if (product.definition.id == coins1000)
        {
            GameManager.AddMoney(5000);
            audioManager.PlaySound(3);
            rewardText.text = "5000";
            purschaisePanel.SetActive(true);
        }
    }

    
    public void UpdateMoney1(Product product)
    {
        money1Text.text = product.metadata.localizedPrice + " " + product.metadata.isoCurrencyCode;
    }
}
