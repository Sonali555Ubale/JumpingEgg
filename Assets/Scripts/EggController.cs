using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EggController : MonoBehaviour
{
    public static EggController instance;
    public Rigidbody2D rb;
    bool isClicked = false;
    public bool insideBowl = true;
    public bool isJumping = false;
    //public int score = 0;
    //public int coins = 0;
    Vector2 forceVector = new Vector2(0f, 280f);
    public Animator blinkAnimator;

    private float lastX = 0f;
    Vector2 xcv = new Vector2();
    int screenHeight;
    Vector3 playerScreenY;
    bool shouldChangeEmotion;

    public bool shouldDisableTap = false;
    bool isGameModeTA = true;
   

    private void Start()
    {
        //Debug.Log(rb.position.x +", " +rb.position.y);
        //Debug.Log(rb.centerOfMass.x +", " +rb.centerOfMass.y);
        rb.centerOfMass = new Vector3(rb.centerOfMass.x, rb.centerOfMass.y - 0.045f);
        screenHeight = Screen.height * 5 / 10;
        shouldChangeEmotion = GameManager.instance.getGameMode() == GameManager.GameMode.TIME_ATTACK;

        blinkAnimator = gameObject.GetComponent<Animator>();

        isGameModeTA = GameManager.instance.getGameMode() == GameManager.GameMode.TIME_ATTACK;
    }

    // Update is called once per frame
    void Update()
    {
        //if (shouldDisableTap) return;
        /*if (isGameModeTA)
        {

            return;
        }*/
        GameManager.instance.updatePlayer(rb, playerScreenY);
        if (SceneManager.GetActiveScene().name == "Copy_TimeAttack 1")
        {
            isGameModeTA = true;
            return;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (GameManager.instance.isPauseOrPlayTouched()) return;
            isClicked = true;
            //isJumping = true;
            lastX = rb.position.x;
            xcv.x = lastX;
            xcv.y = rb.position.y;

            if (GameManager.instance.getGameMode() != GameManager.GameMode.TIME_ATTACK)
            {
                playJumpSound();
            }
            //GameManager.instance.OnPlayerJumped();
            //rb.constraints = RigidbodyConstraints2D.FreezePositionX;
        }



    }

    private void playJumpSound()
    {
        if (isClicked) AudioManager.instance?.Play("jump");
    }

    /*
   void OnMouseDrag()
   {
       Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
       Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);

       transform.position = objPosition;
   }*/


    private void FixedUpdate()
    {

        if (isClicked && insideBowl)
        {
            rb.velocity = Vector2.up * 5.9f;
           // tr.DisableTrajectory(insideBowl);
        }
        else if (isJumping)
        {
            //Debug.Log("Jumping");
            rb.position = xcv;
          
        }
        //rb.constraints = RigidbodyConstraints2D.None;

        isClicked = false;

        //Debug.Log(screenHeight);
        Vector3 tempPosY = transform.position;
        tempPosY.x += 0.35f;
        tempPosY.y += 0.25f;
        playerScreenY = Camera.main.WorldToScreenPoint(tempPosY);
        //Debug.Log(playerScreenY.y);

        shouldChangeEmotion = GameManager.instance.getGameMode() == GameManager.GameMode.TIME_ATTACK;
        blinkAnimator.SetBool("is_sad", shouldChangeEmotion && playerScreenY.y < screenHeight);
    }
}
