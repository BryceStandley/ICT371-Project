using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public bool pickedUp = false;

    private GameObject holdPoint;

    public void SetHoldPoint(GameObject point)
    {
        holdPoint = point;
    }

    private void Update()
    {
        if(pickedUp)
        {
            //constantly move towards a pickup point of the player
            transform.position = holdPoint.transform.position;
            GetComponent<Rigidbody>().freezeRotation = true;
        }
        else
        {
            GetComponent<Rigidbody>().freezeRotation = false;
        }
    }
}
