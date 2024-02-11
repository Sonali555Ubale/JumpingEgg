using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    // public GameObject coinPrefab; // Assign this in the Inspector
    [SerializeField] List<GameObject> coinPrefab;
    public Transform player; // Assign the player's transform here
    public float spawnRate = 1f; // Time in seconds between each spawn
    public float spawnDistanceAbovePlayer = 5f; // Distance above the player to spawn coins

    private float lastSpawnTime = 0f;
    private float playerLastY;

    void Start()
    {
        if (player != null)
        {
            // Initialize player's last Y position
            playerLastY = player.position.y;
        }
       
       // InvokeRepeating("SpawnCoin", spawnRate, playerLastY);
     }

    void Update()
    {
        // Check if it's time to spawn a new coin
        if (player != null && Time.time - lastSpawnTime >= spawnRate && player.position.y > playerLastY)
        {
            //   SpawnCoin();
            Invoke("SpawnCoin", .5f);
            lastSpawnTime = Time.time;
            playerLastY = player.position.y + spawnDistanceAbovePlayer;
        }
    }

    private void SpawnCoin()
    {
        // Spawn the coin directly above the player's last highest position, but always in the center
        Vector3 spawnPosition = new Vector3(0, playerLastY + spawnDistanceAbovePlayer, 0); // X is set to 0 for center
        //if(coinPrefab!=null)
        foreach (GameObject t in coinPrefab)
        {
                if(t!=null)
            Instantiate(t, spawnPosition, Quaternion.identity);
        }
    }
}
