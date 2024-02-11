using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTimeMode : MonoBehaviour
{
    private Transform playerTransform;
    public float offset = 0.002f;
    Vector3 temp;
    int lastScore = 0;
    private Vector3 startPosition;
    bool shouldFollow;

    void Start()
    {
        //playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        startPosition = transform.position;
        shouldFollow = true;
    }


    void LateUpdate()
    {
        /*if (Time.deltaTime % 2 )
        {

        }*/
        if (shouldFollow)
        {
            if (lastScore != GameManager.instance.getCurrentScore())
            {
                lastScore = GameManager.instance.getCurrentScore();
                if (lastScore < 90)
                {
                    offset += 0.001f;

                    if (lastScore % 7 == 0)
                    {
                        offset -= 0.0025f;
                    }
                }

            }                                   

            temp = transform.position;
            temp.y = temp.y + (offset * Time.timeScale);      //time is now considered during calculation of the offset. the camera movement with timescale value from TrajectoryLine script
            transform.position = temp;
        }

        //Debug.Log("player " + GameManager.instance.getPlayer().position.y + "       " + transform.position.y);
       /* if (GameManager.instance.getPlayer() != null)
        {
            if (GameManager.instance.getPlayer().position.y > transform.position.y)
            {
                GameManager.instance.playerDead();
            }
        }
        transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y);*/
    }

    public void StopCameraFollow()
    {
        shouldFollow = false;
    }

    public void ResetCameraPosition()
    {
        transform.position = startPosition;
        shouldFollow = true;
    }
}
