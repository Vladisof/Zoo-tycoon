using System.Collections.Generic;
using DefaultNamespace.Bot;
using UnityEngine;

public class BotPool : MonoBehaviour
{
    [SerializeField] private List<Bot> botPrefabs; // List of different bot prefabs
    [SerializeField] private int initialPoolSize = 10;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform targetPoint;
    [SerializeField] private Transform casirPoint;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int maxActiveBots = 5;
    [SerializeField] private float minMoneyValue = 0.1f;
    [SerializeField] private float maxMoneyValue = 1f;

    private Queue<Bot> pooledBots = new Queue<Bot>();
    private List<Bot> activeBots = new List<Bot>();
    private float nextSpawnTime;

    private void Start()
    {
        // Validate that we have at least one prefab
        if (botPrefabs == null || botPrefabs.Count == 0)
        {
            Debug.LogError("No bot prefabs assigned to the BotPool!");
            return;
        }

        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewBot();
        }

        nextSpawnTime = Time.time + spawnInterval;
    }

    private void Update()
    {
        if (Time.time >= nextSpawnTime && activeBots.Count < maxActiveBots)
        {
            SpawnBot();
            nextSpawnTime = Time.time + Random.Range(spawnInterval * 0.5f, spawnInterval * 1.5f);
        }
    }

    private void CreateNewBot()
    {
        // Get a random bot prefab from the list
        Bot randomPrefab = botPrefabs[Random.Range(0, botPrefabs.Count)];
        
        Bot newBot = Instantiate(randomPrefab);
        newBot.Initialize(this);
        newBot.gameObject.SetActive(false);
        pooledBots.Enqueue(newBot);
    }

    private void SpawnBot()
    {
        if (pooledBots.Count == 0)
        {
            CreateNewBot();
        }

        Bot bot = pooledBots.Dequeue();
        activeBots.Add(bot);

        Vector3 spawnOffset = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        Vector3 targetOffset = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

        bot.Activate(spawnPoint.position + spawnOffset, targetPoint.position + targetOffset, casirPoint.position, minMoneyValue, maxMoneyValue);
    }

    public void ReturnToPool(Bot bot)
    {
        activeBots.Remove(bot);
        pooledBots.Enqueue(bot);
    }
}