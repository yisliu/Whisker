using System.Collections.Generic;
using UnityEngine;

public class ThrowableSpawner : MonoBehaviour
{
    public GameObject[] throwablePrefabs;
    public float spawnRadius = 20f;
    public int maxObjects = 10;
    public float respawnInterval = 15f;

    private List<GameObject> spawnedObjects = new List<GameObject>();

    void Start()
    {
        if (throwablePrefabs == null || throwablePrefabs.Length == 0)
        {
            Debug.LogWarning("[ThrowableSpawner] No prefabs assigned! Add prefabs in the Inspector.");
            return;
        }

        SpawnObjects();
        InvokeRepeating(nameof(SpawnObjects), respawnInterval, respawnInterval);
    }

    void SpawnObjects()
    {
        spawnedObjects.RemoveAll(o => o == null);

        int toSpawn = maxObjects - spawnedObjects.Count;
        int spawned = 0;

        for (int i = 0; i < toSpawn * 3 && spawned < toSpawn; i++)
        {
            Vector2 circle = Random.insideUnitCircle * spawnRadius;
            Vector3 origin = transform.position + new Vector3(circle.x, 50f, circle.y);

            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 200f))
            {
                Vector3 spawnPos = hit.point + Vector3.up * 0.3f;
                GameObject prefab = throwablePrefabs[Random.Range(0, throwablePrefabs.Length)];
                GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity);
                spawnedObjects.Add(obj);
                spawned++;
            }
        }

        Debug.Log($"[ThrowableSpawner] Spawned {spawned} objects ({spawnedObjects.Count} total active).");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}