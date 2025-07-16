using UnityEngine;
using TMPro;

public class HouseLevelsDisplay : MonoBehaviour
{
    public TextMeshProUGUI[] houseLevelTexts; // Assign 8 TMP_Texts in Inspector
    public string[] houseNames;        // Assign 8 house GameObject names in Inspector
    private const int maxLevel = 3;

    void Update()
    {
        for (int i = 0; i < houseLevelTexts.Length && i < houseNames.Length; i++)
        {
            int level = PlayerPrefs.GetInt(houseNames[i] + "_level", 0);
            houseLevelTexts[i].text = $"Lvl {level}/{maxLevel}";
        }
    }
}