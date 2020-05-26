using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectInformation))]
[RequireComponent(typeof(Rigidbody))]
public class PickUp : MonoBehaviour
{
    public bool pickedUp = false;
    public int pickupCounter = 0;

    private GameObject holdPoint;
    private Rigidbody rb;
    private Collider col;
    private GameObject ground;



    private void Awake()
    {
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        ground = GameObject.FindGameObjectWithTag("Ground");
    }


    public void SetHoldPoint(GameObject point)
    {
        holdPoint = point;
    }

    public void DropItem()//setting fixed values to the rigidbody to stop it from gaining unwanted velocity, only do this once
    {
        col.enabled = true;
        rb.velocity = new Vector3(0, -9.5f * 0.7f, 0);
        rb.angularVelocity = Vector3.zero;
        rb.freezeRotation = false;
        
        //transform.parent = null;
        rb.useGravity = true;
        Invoke("StopMovement", 5f);
        
    }

    private void StopMovement()
    {
        rb.velocity = new Vector3(0, 0, 0); 
        rb.angularVelocity = Vector3.zero;
    }

    private void LateUpdate()
    {
        if(pickedUp)
        {
            
                //constantly move towards a pickup point of the player
                transform.position = holdPoint.transform.position;
                rb.useGravity = false;
                col.enabled = false;
                rb.isKinematic = false;
                //transform.parent = holdPoint.transform;
                rb.freezeRotation = true;

        }
        
    }
}
