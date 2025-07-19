using UnityEngine;
using System.Collections.Generic;

public class EnvironmentController : MonoBehaviour
{
    [Header("Road Generation")]
    public GameObject roadPrefab;
    public Transform roadParent;
    public float roadSegmentLength = 50f;
    public int roadSegmentsAhead = 5;
    public int roadSegmentsBehind = 2;
    
    [Header("Obstacle Generation")]
    public GameObject[] obstaclePrefabs;
    public float obstacleSpawnChance = 0.3f;
    public float minObstacleDistance = 20f;
    public float maxObstacleDistance = 40f;
    
    [Header("Environment Objects")]
    public GameObject[] environmentPrefabs; // Trees, buildings, etc.
    public float environmentSpawnChance = 0.5f;
    public Vector3 environmentSpawnRange = new Vector3(10f, 0f, 0f);
    
    private Transform playerTransform;
    private List<GameObject> activeRoadSegments = new List<GameObject>();
    private List<GameObject> activeObstacles = new List<GameObject>();
    private List<GameObject> activeEnvironmentObjects = new List<GameObject>();
    private float lastRoadZ = 0f;
    private float lastObstacleZ = 0f;
    
    private void Start()
    {
        // Find the player (motorcycle)
        SimpleMotorcycle motorcycle = FindObjectOfType<SimpleMotorcycle>();
        if (motorcycle != null)
            playerTransform = motorcycle.transform;
        
        // Generate initial road segments
        for (int i = 0; i < roadSegmentsAhead; i++)
        {
            SpawnRoadSegment();
        }
    }
    
    private void Update()
    {
        if (playerTransform == null) return;
        
        UpdateRoadGeneration();
        UpdateObstacleGeneration();
        CleanupOldObjects();
    }
    
    private void UpdateRoadGeneration()
    {
        float playerZ = playerTransform.position.z;
        
        // Spawn new road segments ahead
        while (lastRoadZ < playerZ + (roadSegmentsAhead * roadSegmentLength))
        {
            SpawnRoadSegment();
        }
    }
    
    private void UpdateObstacleGeneration()
    {
        if (obstaclePrefabs.Length == 0) return;
        
        float playerZ = playerTransform.position.z;
        
        // Spawn obstacles ahead
        if (playerZ > lastObstacleZ - 100f) // Check if we need more obstacles
        {
            if (Random.value < obstacleSpawnChance)
            {
                SpawnObstacle();
            }
            lastObstacleZ = playerZ + Random.Range(minObstacleDistance, maxObstacleDistance);
        }
    }
    
    private void SpawnRoadSegment()
    {
        if (roadPrefab == null) return;
        
        Vector3 spawnPosition = new Vector3(0, 0, lastRoadZ);
        GameObject newRoadSegment = Instantiate(roadPrefab, spawnPosition, Quaternion.identity, roadParent);
        activeRoadSegments.Add(newRoadSegment);
        
        // Spawn environment objects along this road segment
        SpawnEnvironmentObjects(spawnPosition);
        
        lastRoadZ += roadSegmentLength;
    }
    
    private void SpawnObstacle()
    {
        if (obstaclePrefabs.Length == 0) return;
        
        GameObject obstaclePrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
        Vector3 spawnPosition = new Vector3(
            Random.Range(-3f, 3f), // Random X position on road
            0f,
            lastObstacleZ
        );
        
        GameObject newObstacle = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity, transform);
        activeObstacles.Add(newObstacle);
    }
    
    private void SpawnEnvironmentObjects(Vector3 roadPosition)
    {
        if (environmentPrefabs.Length == 0) return;
        
        // Spawn objects on both sides of the road
        for (int side = -1; side <= 1; side += 2) // -1 for left, 1 for right
        {
            if (Random.value < environmentSpawnChance)
            {
                GameObject envPrefab = environmentPrefabs[Random.Range(0, environmentPrefabs.Length)];
                Vector3 spawnPosition = roadPosition + new Vector3(
                    side * Random.Range(environmentSpawnRange.x * 0.5f, environmentSpawnRange.x),
                    environmentSpawnRange.y,
                    Random.Range(-roadSegmentLength * 0.4f, roadSegmentLength * 0.4f)
                );
                
                GameObject newEnvObject = Instantiate(envPrefab, spawnPosition, 
                    Quaternion.Euler(0, Random.Range(0, 360), 0), transform);
                activeEnvironmentObjects.Add(newEnvObject);
            }
        }
    }
    
    private void CleanupOldObjects()
    {
        if (playerTransform == null) return;
        
        float playerZ = playerTransform.position.z;
        float cleanupDistance = roadSegmentsBehind * roadSegmentLength;
        
        // Cleanup old road segments
        for (int i = activeRoadSegments.Count - 1; i >= 0; i--)
        {
            if (activeRoadSegments[i] != null && 
                activeRoadSegments[i].transform.position.z < playerZ - cleanupDistance)
            {
                DestroyImmediate(activeRoadSegments[i]);
                activeRoadSegments.RemoveAt(i);
            }
        }
        
        // Cleanup old obstacles
        for (int i = activeObstacles.Count - 1; i >= 0; i--)
        {
            if (activeObstacles[i] != null && 
                activeObstacles[i].transform.position.z < playerZ - cleanupDistance)
            {
                DestroyImmediate(activeObstacles[i]);
                activeObstacles.RemoveAt(i);
            }
        }
        
        // Cleanup old environment objects
        for (int i = activeEnvironmentObjects.Count - 1; i >= 0; i--)
        {
            if (activeEnvironmentObjects[i] != null && 
                activeEnvironmentObjects[i].transform.position.z < playerZ - cleanupDistance)
            {
                DestroyImmediate(activeEnvironmentObjects[i]);
                activeEnvironmentObjects.RemoveAt(i);
            }
        }
    }
    
    public void ResetEnvironment()
    {
        // Clear all active objects
        foreach (GameObject obj in activeRoadSegments)
        {
            if (obj != null) DestroyImmediate(obj);
        }
        foreach (GameObject obj in activeObstacles)
        {
            if (obj != null) DestroyImmediate(obj);
        }
        foreach (GameObject obj in activeEnvironmentObjects)
        {
            if (obj != null) DestroyImmediate(obj);
        }
        
        activeRoadSegments.Clear();
        activeObstacles.Clear();
        activeEnvironmentObjects.Clear();
        
        lastRoadZ = 0f;
        lastObstacleZ = 0f;
        
        // Regenerate initial road
        for (int i = 0; i < roadSegmentsAhead; i++)
        {
            SpawnRoadSegment();
        }
    }
}