using UnityEngine;
using UnityEngine.UI;

public class ActiveObjectIndicator : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject[] primaryObjects; // Array of primary objects
    public GameObject[] backupObjects; // Array of backup objects
    public RectTransform indicator;
    public float edgeOffset = 50f;

    private GameObject activeObject;
    private int currentObjectId = -1;
    private bool isIndicatorVisible = true;

    void Start()
    {
        // No initial active object
        indicator.gameObject.SetActive(false);
    }

    public void HideIndicator()
    {
        indicator.gameObject.SetActive(false);
        isIndicatorVisible = false;
    }

    public void ShowIndicator()
    {
        if (activeObject != null)
        {
            indicator.gameObject.SetActive(true);
            isIndicatorVisible = true;
        }
    }

    // Call this method from UI buttons to change the target object
    public void NewActiveObject(int iD)
    {
        if (iD >= 0 && iD < primaryObjects.Length)
        {
            currentObjectId = iD;
            GameObject objectToUse;
        
            // Check if primary object exists and is active
            if (primaryObjects[iD] != null && primaryObjects[iD].activeSelf)
            {
                objectToUse = primaryObjects[iD];
            }
            // Otherwise use backup
            else if (backupObjects[iD] != null)
            {
                objectToUse = backupObjects[iD];
            }
            else
            {
                // If both are unavailable, hide indicator and return
                HideIndicator();
                return;
            }
        
            SetActiveObject(objectToUse);
            ShowIndicator();
        }
    }

    void Update()
    {
        // Check if we have a valid currentObjectId
        if (currentObjectId >= 0)
        {
            // Check if primary object was destroyed or inactive
            if (activeObject == primaryObjects[currentObjectId])
            {
                if (primaryObjects[currentObjectId] == null || !primaryObjects[currentObjectId].activeSelf)
                {
                    if (backupObjects[currentObjectId] != null)
                    {
                        SetActiveObject(backupObjects[currentObjectId]);
                    }
                    else
                    {
                        // No valid backup object
                        activeObject = null;
                        HideIndicator();
                        return;
                    }
                }
            }
        }

        // Only proceed if we have an active object and indicator should be visible
        if (activeObject != null && isIndicatorVisible)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(activeObject.transform.position);

            if (IsObjectVisible(screenPos))
            {
                indicator.gameObject.SetActive(false);
            }
            else
            {
                indicator.gameObject.SetActive(true);
                UpdateIndicatorPosition(screenPos);
            }
        }
    }

    public void ToggleIndicator()
    {
        if (isIndicatorVisible)
        {
            HideIndicator();
        }
        else
        {
            ShowIndicator();
        }
    }
    
    private bool IsObjectVisible(Vector3 screenPos)
    {
        return screenPos.z > 0 &&
               screenPos.x > 0 && screenPos.x < Screen.width &&
               screenPos.y > 0 && screenPos.y < Screen.height;
    }

    private void UpdateIndicatorPosition(Vector3 screenPos)
    {
        Vector3 direction = screenPos - new Vector3(Screen.width / 2, Screen.height / 2, screenPos.z);

        if (screenPos.z < 0)
        {
            direction = -direction;
        }

        direction.z = 0;

        Vector3 clampedPosition = direction.normalized * Mathf.Min(Screen.width / 2 - edgeOffset, Screen.height / 2 - edgeOffset);
        Vector3 finalPosition = new Vector3(Screen.width / 2, Screen.height / 2) + clampedPosition;

        indicator.position = finalPosition;
        indicator.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90);
    }

    public void SetActiveObject(GameObject newActiveObject)
    {
        activeObject = newActiveObject;
    }
}