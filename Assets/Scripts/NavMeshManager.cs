
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace DefaultNamespace
{
    public class NavMeshManager : MonoBehaviour
    {
        public static NavMeshManager Instance;
        private List<NavMeshSurface> pendingUpdates = new List<NavMeshSurface>();
        private bool updateScheduled = false;
        private bool isProcessing = false;
    
        [SerializeField] private float rebuildCooldown = 1.0f;
        [SerializeField] private int maxUpdatesPerFrame = 1;
    
        private void Awake()
        {
            Instance = this;
        }

        public void RequestLocalizedNavMeshUpdate(NavMeshSurface surface, Vector3 position, float radius)
        {
            StartCoroutine(UpdateNavMeshInAreaAsync(surface, position, radius));
        }

        private IEnumerator UpdateNavMeshInAreaAsync(NavMeshSurface surface, Vector3 position, float radius)
        {
            yield return null;

            if (surface != null && surface.navMeshData != null)
            {
                // Define the update area
                Bounds bounds = new Bounds(position, new Vector3(radius * 2, radius * 2, radius * 2));

                // Get build settings from the surface
                NavMeshBuildSettings buildSettings = surface.GetBuildSettings();

                // Collect sources for this specific surface
                List<NavMeshBuildMarkup> markups = new List<NavMeshBuildMarkup>();
                List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();

                // Add the object's transform as the root to collect only its sources
                NavMeshBuilder.CollectSources(surface.transform, surface.layerMask, 
                    surface.useGeometry, surface.defaultArea, markups, sources);

                // Update only this specific NavMeshSurface
                NavMeshBuilder.UpdateNavMeshData(
                    surface.navMeshData,
                    buildSettings,
                    sources,
                    bounds);
            
                // Notify NavMesh that it was updated
                if (surface.gameObject.activeInHierarchy)
                {
                    surface.AddData();
                }
            }
        }
        public void RequestNavMeshUpdate(NavMeshSurface surface)
        {
            if (!pendingUpdates.Contains(surface))
            {
                pendingUpdates.Add(surface);
            }

            if (!updateScheduled && !isProcessing)
            {
                updateScheduled = true;
                StartCoroutine(ProcessNavMeshUpdates());
            }
        }

        private IEnumerator ProcessNavMeshUpdates()
        {
            isProcessing = true;
            updateScheduled = false;

            // Wait to batch potential multiple update requests
            yield return new WaitForSeconds(rebuildCooldown);

            int processedThisFrame = 0;

            while (pendingUpdates.Count > 0)
            {
                var surface = pendingUpdates[0];
                pendingUpdates.RemoveAt(0);

                if (surface != null)
                {
                    // Build the entire NavMesh but spread the work across frames
                    surface.BuildNavMesh();
                    processedThisFrame++;

                    if (processedThisFrame >= maxUpdatesPerFrame)
                    {
                        processedThisFrame = 0;
                        // Wait for multiple frames to avoid long freezes
                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }

            isProcessing = false;
        }
    }
}