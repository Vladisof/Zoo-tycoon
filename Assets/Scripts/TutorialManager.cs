using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialPanel; // Перетащи сюда объект туториала в инспекторе

    private const string TutorialKey = "TutorialShown";

    void Start()
    {
        if (PlayerPrefs.GetInt(TutorialKey, 0) == 0)
        {
            tutorialPanel.SetActive(true);
            PlayerPrefs.SetInt(TutorialKey, 1);
            PlayerPrefs.Save();
        }
        else
        {
            tutorialPanel.SetActive(false);
        }
    }
}