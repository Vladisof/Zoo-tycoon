using DefaultNamespace;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class InteractableObject : MonoBehaviour
{
    public string objectName; // Unique object name
    public int iD;
    public int price;
    public TextMeshPro priceText;
    public GameObject[] UnlockedObject;
    public NavMeshSurface navMeshSurface;
    public GameObject nextUpgrade; // This won't control visibility anymore
    public MoneyController moneyController;
    public ActiveObjectIndicator activeObjectIndicator;
    public AudioManager audioManager;
    public GameObject listShop;
    public ExpeditionManager expeditionManager;
    public float preSecond;

    void Start()
    {
        // Check if this specific object is already purchased
        if (PlayerPrefs.GetInt(objectName, 0) == 1)
        {
            for (int i = 0; i < UnlockedObject.Length; i++)
            {
                UnlockedObject[i].SetActive(true);
            }

                expeditionManager.OnHousePurchased(gameObject.name);

            // Update only the area around the unlocked object (adjust radius as needed)
            NavMeshManager.Instance.RequestNavMeshUpdate(navMeshSurface);
            //listShop.SetActive(false);
            Destroy(gameObject);
        }

        priceText.text = price.ToString();
        EventManager.Instance.onInteraction.AddListener(OnInteraction);
    }

    private void OnInteraction(GameObject interactingObject)
    {
        // Only respond if this specific object is being interacted with
        if (interactingObject != this.gameObject && !interactingObject.transform.IsChildOf(this.transform))
        {
            // Not interacting with this specific object, so return without doing anything
            return;
        }

        switch (interactingObject.tag)
        {
            case "MiniGame":
                if (moneyController.SubtractMoney(price))
                {
                    PurchaseObject();
                    moneyController.AddMoneyPerSecond(preSecond);

                        expeditionManager.OnHousePurchased(gameObject.name);
                    
                }
                break;

            case "Upgrade1":
            case "Upgrade2":
            case "Upgrade3":
            case "Upgrade4":
            case "Upgrade5":
                if (moneyController.SubtractMoney(price))
                {
                    PurchaseObject();
                    moneyController.AddMoneyPerSecond(preSecond);

                        expeditionManager.OnHousePurchased(gameObject.name);
                    
                }
                
                break;

            default:
                break;
        }
    }
    // New method to handle the purchase logic in one place
    private void PurchaseObject()
    {
        // Activate the unlocked object
        for (int i = 0; i < UnlockedObject.Length; i++)
        {
            UnlockedObject[i].SetActive(true);
        }
    
        // Add NavMeshObstacle to all child objects with colliders
        Collider[] allColliders = UnlockedObject[0].GetComponentsInChildren<Collider>(true);
        /*foreach (Collider coll in allColliders)
        {
            GameObject childObj = coll.gameObject;
            if (!childObj.GetComponent<NavMeshObstacle>())
            {
                NavMeshObstacle obstacle = childObj.AddComponent<NavMeshObstacle>();
                obstacle.carving = true;
                obstacle.carveOnlyStationary = true;
                obstacle.carvingTimeToStationary = 0.1f;
            
                // Size the obstacle to match the collider
                obstacle.size = coll.bounds.size;
                obstacle.center = coll.bounds.center - childObj.transform.position;
            }
        
            // Make sure the collider is not a trigger
            coll.isTrigger = false;
        }*/
    
        // Also add obstacle to parent if it has a collider
        Collider parentColl = UnlockedObject[0].GetComponent<Collider>();
        if (parentColl != null && !UnlockedObject[0].GetComponent<NavMeshObstacle>())
        {
            NavMeshObstacle obstacle = UnlockedObject[0].AddComponent<NavMeshObstacle>();
            obstacle.carving = true;
            obstacle.carveOnlyStationary = true;
            parentColl.isTrigger = false;
        }

        // Force rebuild NavMeshSurface
        //if (navMeshSurface != null)
       // {
        //    navMeshSurface.BuildNavMesh();
       // }

        // Save purchase state
        PlayerPrefs.SetInt(objectName, 1);
        PlayerPrefs.Save();

        if (activeObjectIndicator != null)
        {
            activeObjectIndicator.NewActiveObject(iD);
        }
        audioManager.PlaySound(3);
        //listShop.SetActive(false);
        Destroy(gameObject);
    }
}