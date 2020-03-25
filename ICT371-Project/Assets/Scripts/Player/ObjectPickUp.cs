using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ObjectPickUp : MonoBehaviour
{
    public Image pointer;
    public Sprite handClosed, handOpen, defaultPointer;
    public bool disabledInput = false;

    public float pickupDistance = 5f;
    public float holdDistance = 5f;

    private Ray centerRay;
    private Camera cam;
    private bool holding = false;
    private bool canPickUp = false;

    private GameObject lookedAtItem;
    private GameObject heldItem;

    private float screenWidth, screenHeight;

    public GameObject holdPoint;

    private void Start()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        cam = Camera.main;  
    }

    private void LateUpdate()
    {
        centerRay = cam.ScreenPointToRay(new Vector3(screenWidth / 2, screenHeight / 2, 0));
        RaycastHit hit;
        if (Physics.Raycast(centerRay, out hit, pickupDistance))
        { 
            if(hit.transform.gameObject.CompareTag("PickUp"))
            {
                canPickUp = true; //object can be picked up
                lookedAtItem = hit.transform.gameObject;

                if (!holding)
                {
                    pointer.sprite = handOpen;
                    
                }
            }
            else
            {
                canPickUp = false;
                pointer.sprite = defaultPointer;
            }
            
        }
        else
        {
            pointer.sprite = defaultPointer;
        }
    }

    public void OnObjectPickUp(InputAction.CallbackContext context)
    {
        if(!disabledInput)
        {
            if (canPickUp && !holding)
            {
                //can pick the item up and were not holding a item already
                //pick up the item
                pointer.sprite = handClosed;
                holding = true;
                PickUpItem(lookedAtItem);
            }
            else if (canPickUp && holding)
            {
                //can pick up the item and currently holding a item
                //drop the item
                pointer.sprite = defaultPointer;
                holding = false;
                DropItem(heldItem);
            }
        }
    }

    private void PickUpItem(GameObject item)
    {
        if(heldItem != null)
        {
            Debug.Log("Player cant pick up more that one item at a time...");
        }
        else
        {
            item.GetComponent<PickUp>().pickedUp = true;
            item.GetComponent<PickUp>().SetHoldPoint(holdPoint);
            heldItem = item;
        }
    }

    private void DropItem(GameObject item)
    {
        item.GetComponent<PickUp>().pickedUp = false;
        heldItem = null;
    }
    

}
