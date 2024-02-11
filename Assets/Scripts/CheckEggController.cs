using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckEggController : MonoBehaviour
{
    bool isCalculated = false;
    public bool isStart = false;           
    public GameObject bowlPrefab;
    private int timesVisited = 0;
    private TrajectoryLine tr;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name);
        EggController egg = collision.GetComponent<EggController>();
        if (egg != null)
        {

            //timesVisited += 1;

            if (!isCalculated && !isStart)
            {
                if (gameObject.transform.position.y > egg.transform.position.y) return;
                // Increase the pitch as the player goes higher
                //  AudioManager.instance.sounds[0].pitch += 0.015f;
                //  AudioManager.instance.Play("Bowl");
                GameManager.instance.increaseScore(-1);
                isCalculated = true;


                float _eggX = egg.rb.position.x;
                _eggX /= _eggX;

                float random = Random.Range(0.51f, 0.62f);
                Vector3 newBowlV = new Vector3(_eggX > 0 ? -random : random, egg.rb.position.y + 2.6f, 0f);
                bowlPrefab.GetComponent<BowlController>().bowlType = 0;

                if (GameManager.instance.getGameMode() != GameManager.GameMode.TUTORIAL)
                {
                    Instantiate(bowlPrefab, newBowlV, Quaternion.identity);
                    Debug.Log("!!!!!!BOWL SPAWNED!!!!!!!!!!!");
                }

                //if (egg.rb.rotation > 90 && egg.rb.rotation < 270)
                //{
                //GameManager.instance.displayAchievement("Upside Down!");
                //}

                // Increase the timer
                GameManager.instance.increaseTimer();
                GameManager.instance.UpdateWallBottomPosition(egg.rb.position.y - 2.6f);

            }
            /*else
            {
                GameManager.instance.showPlayerDialog("Ouch!");
            }*/
            egg.insideBowl = true;
            egg.isJumping = false;
            //Debug.Log(score);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        EggController egg = collision.GetComponent<EggController>();

        if (egg != null)
        {
            egg.insideBowl = false;
            // tr.DisableTrajectory(true);

        }
        /*
                // Play the flick animation here
                Debug.Log("");
                Debug.Log("should flick");
                Animator bowlController = collision.gameObject.GetComponentInParent<Animator>();
                if (bowlController != null)
                {
                    Debug.Log("Did flick");
                    bowlController.SetTrigger("shouldFlick");
                }*/
    }
}
