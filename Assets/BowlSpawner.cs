using UnityEngine;
using System.Collections.Generic; // Add this for using List

public class BowlSpawner : MonoBehaviour
{
    public GameObject bowlPrefab;
    public GameObject player; // Reference to the player object
    public float spawnRate = 3f; // Time between spawns
    private float nextSpawnTime = 1f;
    private float verticalSpawnOffset = 4f; // Vertical offset from the player's position to spawn bowls
    private float spawnHeightBuffer = 10f; // Additional height to ensure bowls spawn off-screen and move into view
    float playerHeight = 2.0f; 

    public int maxBowlsPerSpawn = 3; // Maximum number of bowls to spawn at each interval

    private List<Vector3> lastSpawnPositions = new List<Vector3>(); // List to track last few spawn positions
    private int maxLastSpawnPositions = 5; // Maximum number of last spawn positions to remember

    float minVerticalDistance = 3f;
    float minHorizontalDistance = 5f;

    void Update()
    {
        if ((player != null && Time.time >= nextSpawnTime) && (player.transform.position.y > player.transform.position.y + verticalSpawnOffset))
        {
            SpawnBowls();
            nextSpawnTime = Time.time + spawnRate;
        }
    }

    void SpawnBowls()
    {
        int dynamicMaxBowlsPerSpawn = GameManager.CalculateMaxBowlsToSpawn(player.transform.position.y);

        while (dynamicMaxBowlsPerSpawn > -1.9f)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            // Check if the new position is not too close to any of the last few spawn positions
            bool isTooClose = false;
            foreach (var lastPos in lastSpawnPositions)
            {
                if (Vector3.Distance(spawnPosition, lastPos) < Mathf.Max(minHorizontalDistance, minVerticalDistance))
                {
                    isTooClose = true;
                    break;
                }
            }

            if (!isTooClose)
            {
                Instantiate(bowlPrefab, spawnPosition, Quaternion.identity);
                // Update the list of last spawn positions
                lastSpawnPositions.Add(spawnPosition);
                if (lastSpawnPositions.Count > maxLastSpawnPositions)
                {
                    lastSpawnPositions.RemoveAt(0); // Keep the list size within the limit
                }
                dynamicMaxBowlsPerSpawn--;
            }
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        Camera cam = Camera.main;
        float screenHeight = 2f * cam.orthographicSize;
        float screenWidth = screenHeight * cam.aspect;

        float randomX, spawnY;
        bool validPositionFound = false;
        do
        {
            randomX = Random.Range(-screenWidth / 2, screenWidth / 2);
            spawnY = player.transform.position.y + verticalSpawnOffset + Random.Range(0f, spawnHeightBuffer);

            validPositionFound = true;
            // Check against all last spawned positions to ensure the minimum vertical distance
            foreach (Vector3 lastPos in lastSpawnPositions)
            {
                if (Mathf.Abs(spawnY - lastPos.y) < playerHeight)
                {
                    validPositionFound = false;
                    break;
                }
            }
        } while (!validPositionFound);

        // Update lastSpawnPositions to include this new position
        Vector3 newPosition = new Vector3(randomX, spawnY, 0f);
        lastSpawnPositions.Add(newPosition);
        // Optionally, limit the size of lastSpawnPositions to avoid indefinite growth
        if (lastSpawnPositions.Count > maxLastSpawnPositions)
        {
            lastSpawnPositions.RemoveAt(0);
        }

        return newPosition;
    }


}
