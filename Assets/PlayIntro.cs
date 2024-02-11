using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayIntro : MonoBehaviour
{
    private float waitingTime = 10f;

    private void Start()
    {
        StartCoroutine(waitForIntro(waitingTime));
    }

    IEnumerator waitForIntro(float waitinTime)
    {
        yield return new WaitForSeconds(waitinTime);
        SceneManager.LoadScene(1);
    }
}
