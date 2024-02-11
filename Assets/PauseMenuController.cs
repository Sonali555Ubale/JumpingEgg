using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] GameObject RestartGamePanel;
    [SerializeField] GameObject PauseScreen;
    [SerializeField] GameObject ScoreObj;
    [SerializeField] GameObject CoinPanel;
    [SerializeField] GameObject HighScoreObj;
    [SerializeField] GameObject TimerTextObj;
    [SerializeField]
    private TMP_Text scoreText;
    [SerializeField]
    private TMP_Text timerText;

    // Update is called once per frame
    public void Setup(int score, string timer)
    {
     
        RestartGamePanel.SetActive(true);
        PauseScreen.SetActive(false);
        if (SceneManager.GetActiveScene().name == "Copy_TimeAttack 1") TimerTextObj.SetActive(true);
        scoreText.text = "Your Score : " + score.ToString();
        timerText.text = "Time : " + timer.ToString();
      
        ScoreObj.SetActive(false);
        CoinPanel.SetActive(false);
        TimerTextObj.SetActive(false);
        HighScoreObj.SetActive(false);
    }
}
