using UnityEngine;

public class AnimalFeedPoint : MonoBehaviour
{
    public AnimalFeedingManager feedingManager;

    private bool playerInRange = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log(other.name);
        }
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
            feedingManager.TryFeedAnimal(transform);
            
            // Optionally, show feedback to the player
        }
    }
}