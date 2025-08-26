using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoneyController : MonoBehaviour
{
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI addMoneyPerSecondText;
    private float money;
    private float addMoneyPerSecond;

    private void Start()
    {
        money = PlayerPrefs.GetFloat("Money", 9000);
        addMoneyPerSecond = PlayerPrefs.GetFloat("AddMoneyPerSecond", 0);
        UpdateMoneyText();
        UpdateMoneyPerSecondText();
    }

    private void Update()
    {
        money += addMoneyPerSecond * Time.deltaTime;
        UpdateMoneyText();
    }

    public void AddMoney(float amount)
    {
        money += amount;
        SaveMoney();
        UpdateMoneyText();
    }

    public void AddMoneyPerSecond(float amount)
    {
        addMoneyPerSecond += amount;
        SaveMoneyPerSecond();
        UpdateMoneyPerSecondText();
    }

    public bool SubtractMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            SaveMoney();
            UpdateMoneyText();
            Debug.Log("Операция выполнена успешно.");
            return true;
        }
        else
        {
            Debug.LogWarning("Недостаточно монет для выполнения операции.");
            return false;
        }
    }

    private void UpdateMoneyText()
    {
        if (money < 5)
        {
            money = 5;
            SaveMoney();
        }
        moneyText.text = "" + money.ToString("F0");
    }

    private void UpdateMoneyPerSecondText()
    {
        addMoneyPerSecondText.text = "" + addMoneyPerSecond.ToString("F1") + "";
    }

    private void SaveMoneyPerSecond()
    {
        PlayerPrefs.SetFloat("AddMoneyPerSecond", addMoneyPerSecond);
        PlayerPrefs.Save();
    }
    
    private void MinusMoneyPerSecond()
        {
            addMoneyPerSecond = 0;
            SaveMoneyPerSecond();
            UpdateMoneyPerSecondText();
        }
            

    private void SaveMoney()
    {
        PlayerPrefs.SetFloat("Money", money);
        PlayerPrefs.Save();
    }

    private void OnApplicationQuit()
    {
        SaveMoney();
    }
}