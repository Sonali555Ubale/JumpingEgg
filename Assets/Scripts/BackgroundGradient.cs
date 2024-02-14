using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
//using UnityEngine.Experimental.Rendering.LWRP;

public class BackgroundGradient : MonoBehaviour
{
    [SerializeField] GameObject sun;
    [SerializeField] Light2D globalLight;
  

    Color colorStartDusk = new Color32(115, 0, 212, 255);
    Color colorEndDusk = new Color32(254, 155, 0, 255);

    Color colorStartDay = new Color32(255, 246, 103, 255);
    Color colorEndDay = new Color32(255, 237, 165, 255);

    Color colorStartDawn = new Color32(137, 243, 255, 255);
    Color colorEndDawn = new Color32(255, 76, 110, 255);

    Color colorStartNight = new Color32(8, 13, 63, 255);
    Color colorEndNight = new Color32(23, 23, 23, 255);

    UIGradient gradient;

    int changeInterval = 0;
    float totalZ = 10f; // say it takes said seconds to complete the activity
    float currentZ = 0;   // the amount of time that has elapsed so far

    private void Start()
    {
        gradient = GetComponent<UIGradient>();
    }

    void Update()
    {
        if (changeInterval > 4) { currentZ = 0f; changeInterval = 0; }
        if (currentZ >= totalZ) { currentZ = 0f; changeInterval++; }

        currentZ += Time.deltaTime; // add time each frame
        float percentComplete = currentZ / totalZ; // value between 0 - 1
        percentComplete = Mathf.Clamp01(percentComplete); // this prevents it exceeding 1
        //Debug.LogError(percentComplete);
        
        if (changeInterval == 0)
        {
            TransitionGradient(colorStartDusk, colorStartDay, colorEndDusk, colorEndDay, percentComplete);
        }
        if (changeInterval == 1)
        {
            TransitionGradient(colorStartDay, colorStartDawn, colorEndDay, colorEndDawn, percentComplete);
        }
        if (changeInterval == 2)
        {
            TransitionGradient(colorStartDawn, colorStartNight, colorEndDawn, colorEndNight, percentComplete);
            globalLight.intensity = Mathf.Clamp01(1 - percentComplete);
        }
        if (changeInterval == 3)
        {
            TransitionGradient(colorStartNight, colorStartDusk, colorEndNight, colorEndDusk, percentComplete);
            globalLight.intensity = Mathf.Clamp01(percentComplete);
        }

    }  

    void TransitionGradient(Color colorStart1, Color colorStart2, Color colorEnd1, Color colorEnd2, float interval)
    {
        gradient.gameObject.SetActive(false);
        gradient.m_color1 = Color.Lerp(colorStart1, colorStart2, interval);
        gradient.m_color2 = Color.Lerp(colorEnd1, colorEnd2, interval);
        gradient.gameObject.SetActive(true);
    }
}
