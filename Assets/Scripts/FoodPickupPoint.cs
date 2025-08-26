using UnityEngine;

public class FoodPickupPoint : MonoBehaviour
{
    public AnimalFeedingManager feedingManager;

    private bool playerInRange = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    void Update()
    {
        if (playerInRange)
        {
            feedingManager.PickupFood();
            playerInRange = false; // Prevent multiple pickups
            // Optionally, show feedback to the player
        }
    }
}