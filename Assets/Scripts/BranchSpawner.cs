using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchSpawner : MonoBehaviour
{

    [SerializeField] List<GameObject> types;

    private void Start()
    {

        Vector3 tempPos = Camera.main.WorldToScreenPoint(transform.position);

        tempPos.x = Mathf.Clamp(tempPos.x, 0, Screen.width);
    }

    public void AddNewBranches(float offset)
    {
        Transform playerTransform = GameObject.FindWithTag("Player").GetComponent<EggController>().transform;
        float lastY = playerTransform.position.y /*+ Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height)).y*/ + (offset > 0f ? Random.Range(4f, 6f) : 0);

        foreach (GameObject t in types) {
            //GameObject obj = types[Random.Range(0, types.Count - 1)];
            GameObject obj = Instantiate(t, new Vector3(0, lastY, 0), Quaternion.identity);
            Rotate(obj);
            FlipAndChangePosition(obj, lastY);
          //  Debug.LogError("Start");
           // Debug.LogError(Screen.width);
           // Debug.LogError(transform.position);
           // Debug.LogError(Camera.main.WorldToScreenPoint(transform.position));
           // Debug.LogError(Camera.main.ScreenToWorldPoint(transform.position));
           // Debug.LogError(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0)));
          //  Debug.LogError("End");
            lastY += Random.Range(1.3f, 1.8f);
        }
    }

    private void Rotate(GameObject obj)
    {
        obj.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-40f, 40f));
    }

    private void FlipAndChangePosition(GameObject obj, float lastY)
    {
        int multiplier = Random.Range(0f, 1.0f) <= 0.5 ? -1 : 1;

        if (multiplier < 0)
        {
            obj.transform.position = new Vector3(-1, lastY);
            obj.transform.localScale = new Vector3(1 , obj.transform.localScale.y);
        } else
        {
            obj.transform.position = new Vector3(1, lastY);
            obj.transform.localScale = new Vector3(-1 , obj.transform.localScale.y);
        }
    }
}
