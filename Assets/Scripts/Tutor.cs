using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutor : MonoBehaviour
{
    public PlayerTouchMovement playerTouchMovement;
    public Button buttonUnlock;
    public Button buttonClose;
    public GameObject tutor;

    private const string FirstLaunchKey = "FirstLaunch";

    private void Start()
    {
        buttonUnlock.onClick.AddListener(UnlockTutor);
        buttonClose.onClick.AddListener(CloseTutor);

        if (PlayerPrefs.GetInt(FirstLaunchKey, 1) == 1)
        {
            tutor.SetActive(true);
            playerTouchMovement.OnDeactivate();
            PlayerPrefs.SetInt(FirstLaunchKey, 0);
            PlayerPrefs.Save();
        }
        else
        {
            tutor.SetActive(false);
        }
    }

    private void CloseTutor()
    {
        tutor.SetActive(false);
        playerTouchMovement.OnActivate();
    }

    private void UnlockTutor()
    {
        tutor.SetActive(true);
        playerTouchMovement.OnDeactivate();
    }
}