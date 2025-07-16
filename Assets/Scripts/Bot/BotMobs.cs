using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

namespace DefaultNamespace.Bot
{
    public class BotMobs : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private float walkSpeed = 2f;
        [SerializeField] private float runningThreshold = 3f;
        [SerializeField] private float wanderRadius = 5f; // Radius for random movement
        [SerializeField] private float minWanderTime = 2f; // Minimum time between random movements
        [SerializeField] private float maxWanderTime = 6f; // Maximum time between random movements
        [SerializeField] private int minWanderCount = 3; // Minimum number of random movements
        [SerializeField] private int maxWanderCount = 7; // Maximum number of random movements
        [SerializeField] private float minMoneyValue = 0.1f; // Minimum money value
        [SerializeField] private float maxMoneyValue = 1f; // Maximum money value
        [SerializeField] private float wanderCooldown = 5f; // Time between wander cycles
        [SerializeField] private NavMeshAgent agent;

        private MoneyController moneyController;
        private AudioManager audioManager;
        private Vector3 spawnPosition;
        private bool isWandering = false;

        private void Awake()
        {
            if (agent == null) agent = GetComponent<NavMeshAgent>();
            if (animator == null) animator = GetComponent<Animator>();
            moneyController = FindObjectOfType<MoneyController>();
            audioManager = FindObjectOfType<AudioManager>();
            
            // Store the initial position as spawn position
            spawnPosition = transform.position;
        }

        private void Start()
        {
            // Start wandering behavior
            StartCoroutine(WanderRoutine());
        }

        private IEnumerator WanderRoutine()
        {
            while (true)
            {
                // Start wandering if not already wandering
                if (!isWandering)
                {
                    StartCoroutine(RandomWanderBehavior());
                }
                
                // Wait before checking again
                yield return new WaitForSeconds(1f);
            }
        }

        private IEnumerator RandomWanderBehavior()
        {
            isWandering = true;
            
            // Determine how many random movements to make
            int wanderCount = Random.Range(minWanderCount, maxWanderCount + 1);

            // Perform random movements
            for (int i = 0; i < wanderCount; i++)
            {
                // Get a random position within the wander radius
                Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
                randomDirection.y = 0; // Keep on same Y level
                randomDirection += spawnPosition;
                NavMeshHit hit;

                // Find a valid position on the NavMesh
                if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
                {
                    // Move to random position
                    agent.SetDestination(hit.position);

                    // Wait until reaching destination
                    while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
                    {
                        UpdateAnimation();
                        yield return null;
                    }

                    // Idle at destination
                    animator.SetBool("IsitMoving", false);
                    animator.SetBool("IsitFast", false);
                    yield return new WaitForSeconds(Random.Range(minWanderTime, maxWanderTime));
                }
            }

            // Generate random money amount
            int moneyAmount = Mathf.RoundToInt(Random.Range(minMoneyValue, maxMoneyValue));
            audioManager.PlaySound(3);
            moneyController.AddMoney(moneyAmount);

            // Return to exact spawn position
            agent.SetDestination(spawnPosition);

            // Wait until reaching spawn position
            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            {
                UpdateAnimation();
                yield return null;
            }

            // Reset animation
            animator.SetBool("IsitMoving", false);
            animator.SetBool("IsitFast", false);
            
            // Wait before next wander cycle
            yield return new WaitForSeconds(wanderCooldown);
            
            isWandering = false;
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
    }
}