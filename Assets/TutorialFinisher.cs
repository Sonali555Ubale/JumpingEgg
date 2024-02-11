using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialFinisher : MonoBehaviour
{
    private TutorialManager tutorialManager;


    private void Start()
    {
        // Find the TutorialManager in the scene
        tutorialManager = FindObjectOfType<TutorialManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && tutorialManager != null)
        {
            Debug.LogError("Collision Happend");
            FindObjectOfType<AudioManager>().Play("success");
            tutorialManager.FinishTutorial();
        }
    }
}
