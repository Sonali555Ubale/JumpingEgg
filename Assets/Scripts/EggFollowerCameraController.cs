using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggFollowerCameraController : MonoBehaviour
{
    private Transform playerTransform;
    public float offset = 1.2f;
    Vector3 temp;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (playerTransform == null) return;
        temp = transform.position;
        float multiplier = playerTransform.position.y * offset;
        temp.y = playerTransform.position.y + multiplier;
        transform.position = temp;
        /*transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y);*/
    }
}
