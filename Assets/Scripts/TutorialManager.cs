using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject introMessage;
    private TMPro.TextMeshProUGUI textIntroMessages;
    private Animator animatorIntroMessages;
    private string[] introMessageArr = { 
        "Hey there! 0", 
        "Welcome to the Eggs land! 1", 
        "Allow me\nto guide you through... 2",
        "Let's begin 3",
        "Tap anywhere on the screen 4",
        "Tap now 5",
        "Tap now 6",
        "One More jump! 7",
        "Good Job! 7",
        "Let's roll\nYou're ready now! 8"
        //"Great job!\n You're ready now!"
    };
    private int introMessagesIndex = 0;

    // Animation Constants
    private string SHOW_INTRO = "show_new_message";

    public EggController player;
    double timeAfterIntro = 2f;
    bool shouldCloseTextPanel = false;
    float lastc2x = 0f;

    // Bowl Controllers
    public BowlController bowlController1;
    public BowlController bowlController2;
    public BowlController bowlController3;
   public GameObject endBasket; //end basket
    //

    bool isTutorialComplete = false;
    private bool gameIsPaused;
   
    private bool onLastJump = false;
    private MainMenu mainMenu;

    void Start()
    {
        mainMenu = FindObjectOfType<MainMenu>();
        isTutorialComplete = GameManager.instance.getIsTutorialComplete();

        FindObjectOfType<AudioManager>().Play("Bg");

        textIntroMessages = introMessage.GetComponent<TMPro.TextMeshProUGUI>();
        animatorIntroMessages = introMessage.GetComponent<Animator>();
        player = GameObject.FindWithTag("Player").GetComponent<EggController>();
      
        StartCoroutine(StartInstructionsEnumerator());
        DisablePlayerInput(true);
    }

    IEnumerator StartInstructionsEnumerator()
    {
        yield return new WaitForSeconds(2f);
        ShowIntroTextAndAnimate();
       
    }

    private void Update()
    {
        if (!isTutorialComplete)
        {
            HandleTutorialProgress();
        }
    }

    void HandleTutorialProgress()
    {
        if (shouldCloseTextPanel)
        {
            CloseTextPanel();
        }

        if (introMessagesIndex <= 3)
        {
         
            HandleIntroTap();
        }
        else if (introMessagesIndex == 4)
        {
            HandleFirstJump();
        }
       
        else if (introMessagesIndex > 4 && introMessagesIndex < introMessageArr.Length)
        {
            HandleMovingBowls();
        }
    }

   

    void HandleIntroTap()
    {
        if (Input.GetMouseButtonUp(0))
        {
            introMessagesIndex++;
            ShowIntroTextAndAnimate();
          
        }
    }

    void HandleFirstJump()
    {
       DisablePlayerInput(false);
        if (Input.GetMouseButtonUp(0))
        {
            CloseTextPanel();
            DisablePlayerInput(true);
            introMessagesIndex++;
        }
      }


    void HandleMovingBowls()
    {
        timeAfterIntro -= Time.deltaTime;
        if (timeAfterIntro > 0) return;

        float c1x, c2x;
        Debug.LogError("Intro msg index:"+introMessagesIndex);
        if (introMessagesIndex >6) // Handling the last jump
        {
            c1x = bowlController3.transform.position.x;
            c2x = endBasket.transform.position.x;
            HandleLastJump(c1x, c2x);
        }
       /* else if (introMessagesIndex == 7)
        {
            c1x = bowlController2.transform.position.x;
            c2x = bowlController3.transform.position.x;
            HandleRegularJump(c1x, c2x);
        }*/
        else if (introMessagesIndex == 6 )
        {
            c1x = bowlController2.transform.position.x;
            c2x = bowlController3.transform.position.x;
            HandleRegularJump(c1x, c2x);
        }
        else
        {
            c1x = bowlController1.transform.position.x;
            c2x = bowlController2.transform.position.x;
            HandleRegularJump(c1x, c2x);
        }
    }

    void HandleRegularJump(float c1x, float c2x)
    {
        bool isGoingLeft = c2x < lastc2x;
        lastc2x = c2x;
        float diff = Mathf.Abs(c1x - c2x);

        if ((diff >= 0.4 && diff <= 0.5) && isGoingLeft && !gameIsPaused && c1x < c2x)
        {
            PauseGame(true);
            DisablePlayerInput(false);
            ShowIntroTextAndAnimate();
        }
        else if (gameIsPaused)
        {
            if (Input.GetMouseButtonUp(0))
            {
                PauseGame(false);
                CloseTextPanel();
                DisablePlayerInput(true);
                introMessagesIndex++;
            }
        }
    }

    void HandleLastJump(float c1x, float c2x)
    {
       // 
        if (introMessagesIndex ==7)
        {
            DisablePlayerInput(false);
           
        }


        else
        {
            HandleRegularJump(c1x, c2x);
        }
    }
 /*   private bool PlayerReachedEndBasket()
    {
        Debug.LogError("player and last Basket disatnce::" + player.transform.position +"::"+endBasket.transform.position+"distance="+ (player.transform.position- endBasket.transform.position));
        float distanceThreshold = -0.2f; // You can adjust this threshold based on your game's scale
        return Vector3.Distance(player.transform.position, endBasket.transform.position) < distanceThreshold;

    }*/
   public void FinishTutorial()
    {
        introMessagesIndex++;
        isTutorialComplete = true;
        ShowIntroTextAndAnimate();
        StartCoroutine(PlayTutorialCompleteTransition());
        MainMenu.MarkTutorialAsCompleted();

    }

    IEnumerator PlayTutorialCompleteTransition()
    {
        yield return new WaitForSeconds(3f);
        GameManager.instance.MakeSceneTransition(GameManager.GameMode.MENU);
    }

    void DisablePlayerInput(bool value)
    {
        GameManager.instance.isGamePaused = value;
        player.shouldDisableTap = value;
    }

    void PauseGame(bool pause)
    {
        gameIsPaused = pause;
        Time.timeScale = pause ? 0f : 1f;
    }

    void CloseTextPanel()
    {
        shouldCloseTextPanel = false;
        introMessage.transform.parent.gameObject.SetActive(false);
    }

    void ShowIntroTextAndAnimate()
    {
        textIntroMessages.text = introMessageArr[introMessagesIndex];
        introMessage.transform.parent.gameObject.SetActive(true);
        animatorIntroMessages.SetTrigger(SHOW_INTRO);
        FindObjectOfType<AudioManager>().Play("Messages");
        
    }
}
