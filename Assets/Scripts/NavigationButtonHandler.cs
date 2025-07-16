using UnityEngine;
using UnityEngine.UI;

public class NavigationButtonHandler : MonoBehaviour
{
    public ActiveObjectIndicator indicator;
    public Button[] navigationButtons;

    void Start()
    {
        // Set up each button to point to a specific object
        for (int i = 0; i < navigationButtons.Length; i++)
        {
            int objectID = i; // Create local copy for lambda
            navigationButtons[i].onClick.AddListener(() => PointToObject(objectID));
        }
    }

    void PointToObject(int objectID)
    {
        indicator.NewActiveObject(objectID);
    }
}