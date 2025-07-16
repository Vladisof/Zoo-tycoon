using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class Bot : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runningThreshold = 3f;
    [SerializeField] private TextMeshPro moneyTextPrefab;
    [SerializeField] private Transform moneyTextSpawnPoint; // Add this to position where the text should appear
    [SerializeField] private float textDisplayDuration = 1f;
    [SerializeField] private Vector2 moneyRange = new Vector2(5, 50);

    private MoneyController moneyController;
    private AudioManager audioManager;
    private Vector3 startPosition;
    private bool isActive = false;
    private BotPool pool;

    private void Awake()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();
        moneyController = FindObjectOfType<MoneyController>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    public void Initialize(BotPool botPool)
    {
        pool = botPool;
    }

    public void Activate(Vector3 spawnPosition, Vector3 targetPosition, Vector3 casirPosition, float minMoneyValue, float maxMoneyValue)
    {
        startPosition = spawnPosition;
        transform.position = spawnPosition;
        gameObject.SetActive(true);
        isActive = true;

        StartCoroutine(BotBehavior(targetPosition, casirPosition, minMoneyValue, maxMoneyValue));
    }

    private IEnumerator BotBehavior(Vector3 targetPosition, Vector3 casirPosition, float minMoneyValue, float maxMoneyValue)
    {
        // Go to target
        agent.SetDestination(targetPosition);

        // Wait until reaching destination or getting close enough
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            UpdateAnimation();
            yield return null;
        }
        animator.SetBool("IsitMoving", false);
        animator.SetBool("IsitFast", false);
        yield return new WaitForSeconds(Random.Range(2f, 10f));

        agent.SetDestination(casirPosition);

        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            UpdateAnimation();
            yield return null;
        }
        animator.SetBool("IsitMoving", false);
        animator.SetBool("IsitFast", false);
        yield return new WaitForSeconds(Random.Range(1, 3f));
        
        // Generate random money amount between 5 and 50
        int moneyAmount = Mathf.RoundToInt(Random.Range(minMoneyValue, maxMoneyValue));
        audioManager.PlaySound(3);
        moneyController.AddMoney(moneyAmount);
        
        // Show money text
        StartCoroutine(ShowMoneyText(moneyAmount));
        
        // Wait for text display to finish
        yield return new WaitForSeconds(textDisplayDuration);
        
        // Return to start
        agent.SetDestination(startPosition);

        // Wait until reaching start position
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            UpdateAnimation();
            yield return null;
        }

        // Deactivate and return to pool
        Deactivate();
    }

    private IEnumerator ShowMoneyText(int amount)
    {
        // Instantiate the money text prefab
        TextMeshPro moneyText = Instantiate(moneyTextPrefab, moneyTextSpawnPoint.position, Quaternion.identity);
        moneyText.transform.SetParent(moneyTextSpawnPoint);
        
        // Set the text to show the money amount
        moneyText.text = "+" + amount.ToString() + "$";
        
        // Optional: animate the text moving upward
        Vector3 startPos = moneyText.transform.localPosition;
        Vector3 endPos = startPos + Vector3.up * 1.5f;
        float elapsedTime = 0;
        
        while (elapsedTime < textDisplayDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / textDisplayDuration;
            
            // Move text upward
            moneyText.transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            
            // Optional: fade out text near the end
            if (t > 0.7f)
            {
                Color textColor = moneyText.color;
                textColor.a = Mathf.Lerp(1, 0, (t - 0.7f) / 0.3f);
                moneyText.color = textColor;
            }
            
            yield return null;
        }
        
        // Destroy the text object
        Destroy(moneyText.gameObject);
    }

    private void UpdateAnimation()
    {
        float currentSpeed = agent.velocity.magnitude;

        if (currentSpeed > 0.3f)
        {
            animator.SetBool("IsitMoving", true);
            animator.SetBool("IsitFast", currentSpeed >= runningThreshold);
        }
        else
        {
            animator.SetBool("IsitMoving", false);
            animator.SetBool("IsitFast", false);
        }
    }

    public void Deactivate()
    {
        isActive = false;
        gameObject.SetActive(false);
        pool.ReturnToPool(this);
    }
}