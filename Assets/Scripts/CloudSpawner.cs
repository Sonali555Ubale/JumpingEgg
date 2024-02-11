using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> cloudPrefabs;
    public Transform player;

    public float verticalOffset = 3f; // Vertical distance between each cloud
    public float horizontalDistance = 3f; // Horizontal distance from the player to spawn clouds
    private int cloudCount = 0;
    private float lastY;
    private bool spawnLeft = true; // Toggle spawning side

   /* private void Start()
    {
        lastY = player.position.y + verticalOffset;
       // SpawnClouds();
    }*/

    private void Update()
    { if(player!=null)
        if (cloudCount < 3 || player.position.y > lastY - verticalOffset && player.position.y > 1.5f)
        {
            SpawnClouds();
        }
    }

    private void SpawnClouds()
    {
        // Determine the side to spawn the cloud
        Vector3 spawnPosition = spawnLeft
            ? new Vector3(player.position.x - horizontalDistance, lastY, 0)
            : new Vector3(player.position.x + horizontalDistance, lastY, 0);

        // Instantiate the cloud
        int prefabIndex = Random.Range(0, cloudPrefabs.Count);
        Instantiate(cloudPrefabs[prefabIndex], spawnPosition, Quaternion.identity);

        // Update for next cloud
        lastY += verticalOffset;
        spawnLeft = !spawnLeft; // Switch side
        cloudCount++;
    }
}
