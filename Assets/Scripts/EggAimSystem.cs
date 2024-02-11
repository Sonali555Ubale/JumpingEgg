using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggAimSystem : MonoBehaviour
{
    private bool isPressed;
    public Rigidbody2D rb;
    private SpringJoint2D springJ;

    private float releaseDelay;
    private float maxDragDistance;
    public Rigidbody2D slingRigidBody;


    private void Awake()
    {
       rb = GetComponent<Rigidbody2D>();
        springJ = GetComponent<SpringJoint2D>();
        slingRigidBody = springJ.connectedBody;
        releaseDelay = 1 / (springJ.frequency * 4);
    }


    // Update is called once per frame
    void Update()
    {
        if (isPressed) DragEgg();
    }

    private void DragEgg()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        float distance = Vector2.Distance(mousePosition, slingRigidBody.position);
        if(distance> maxDragDistance)
        {
            Vector2 direction = (mousePosition - slingRigidBody.position).normalized;
            rb.position = mousePosition;
        }
        else
        {
            rb.position = mousePosition;
        }

    }

    private void OnMouseDown()
    {
        isPressed = true;
        rb.isKinematic = true;
        Debug.LogError("OnMouseDown");
    }
    private void OnMouseUp()
    {
        isPressed = false;
        rb.isKinematic = false;
        StartCoroutine(Release());
    }
    private IEnumerator Release()
    {
        yield return new WaitForSeconds(releaseDelay);
        springJ.enabled = false;

    }

}
