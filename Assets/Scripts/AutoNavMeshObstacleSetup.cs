using UnityEngine;
using UnityEngine.AI;

public class AutoNavMeshObstacleSetup : MonoBehaviour
{
    void Start()
    {
        Collider col = GetComponent<Collider>();
        if (col == null) return;

        NavMeshObstacle obstacle = GetComponent<NavMeshObstacle>();
        if (obstacle == null)
            obstacle = gameObject.AddComponent<NavMeshObstacle>();

        obstacle.carving = true;

        if (col is BoxCollider box)
        {
            obstacle.shape = NavMeshObstacleShape.Box;
            obstacle.center = box.center;
            obstacle.size = box.size;
        }
        else if (col is CapsuleCollider capsule)
        {
            obstacle.shape = NavMeshObstacleShape.Capsule;
            obstacle.center = capsule.center;
            obstacle.radius = capsule.radius;
            obstacle.height = capsule.height;
        }
    }
}