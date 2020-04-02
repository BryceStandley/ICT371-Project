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
    public GameObject heldItem;

    private float screenWidth, screenHeight;

    public GameObject holdPoint;

    private bool detailsDisplaying = false;

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

                if (!holding && !detailsDisplaying)
                {
                    pointer.sprite = handOpen;
                    ObjectInformationToolTip.ShowPrompt();
                }
            }
            else
            {
                canPickUp = false;
                pointer.sprite = defaultPointer;
                ObjectInformationToolTip.HidePrompt();
                ObjectInformationToolTip.HideTip();
                detailsDisplaying = false;
            }
            
        }
        else
        {
            pointer.sprite = defaultPointer;
            ObjectInformationToolTip.HidePrompt();
            ObjectInformationToolTip.HideTip();
            detailsDisplaying = false;
        }
    }

    public void OnObjectDetailsDisplayed(InputAction.CallbackContext context)
    {
        if(context.performed && !detailsDisplaying && !holding)
        {

            detailsDisplaying = true;
            ObjectInformationToolTip.ShowTip(lookedAtItem.GetComponent<ObjectInformation>().itemName, lookedAtItem.GetComponent<ObjectInformation>().itemStats);
            ObjectInformationToolTip.HidePrompt();

        }
        else if (context.performed && detailsDisplaying && !holding)
        {
            ObjectInformationToolTip.HideTip();
            ObjectInformationToolTip.ShowPrompt();
            detailsDisplaying = false;
        }
    }

    public void OnObjectPickUp(InputAction.CallbackContext context)
    {
        if (!disabledInput && context.performed)
        {
            if (canPickUp && !holding)
            {
                //can pick the item up and were not holding a item already
                //pick up the item
                pointer.sprite = handClosed;
                holding = true;
                PickUpItem(lookedAtItem);
                ObjectInformationToolTip.HidePrompt();
                ObjectInformationToolTip.HideTip();
                detailsDisplaying = false;
            }
            else if (canPickUp && holding)
            {
                //can pick up the item and currently holding a item
                //drop the item
                pointer.sprite = defaultPointer;
                holding = false;
                DropItem(heldItem);
            }
            else if(holding && heldItem != null)
            {
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
            ObjectInformationToolTip.HidePrompt();
            ObjectInformationToolTip.HideTip();
            detailsDisplaying = false;
        }
    }

    private void DropItem(GameObject item)
    {
        item.GetComponent<PickUp>().pickedUp = false;
        item.GetComponent<PickUp>().DropItem();
        heldItem = null;
        ObjectInformationToolTip.HideTip();
    }
    

}
