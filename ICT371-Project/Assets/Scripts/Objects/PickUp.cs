using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectInformation))]
[RequireComponent(typeof(Rigidbody))]
public class PickUp : MonoBehaviour
{
    public bool pickedUp = false;

    private GameObject holdPoint;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }


    public void SetHoldPoint(GameObject point)
    {
        holdPoint = point;
    }

    public void DropItem()//setting fixed values to the rigidbody to stop it from gaining unwanted velocity, only do this once
    {
        rb.velocity = new Vector3(0, -9.5f * 0.7f, 0);
        rb.angularVelocity = Vector3.zero;
        rb.freezeRotation = false;
    }

    private void Update()
    {
        if(pickedUp)
        {
            //constantly move towards a pickup point of the player
            transform.position = holdPoint.transform.position;
            rb.freezeRotation = true;
     
        }
    }
}
