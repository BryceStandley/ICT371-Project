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

    public GameObject lookedAtItem;
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

    public void UpdateScreenSize()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
    }

    private void LateUpdate()// Called after the update method
    {
        centerRay = cam.ScreenPointToRay(new Vector3(screenWidth / 2, screenHeight / 2, 0));
        RaycastHit hit;
        if (Physics.Raycast(centerRay, out hit, pickupDistance) || heldItem != null)//Casting ray from center of screen and checking if the player is holding a object
        {
            if(hit.transform != null)
            {

                if (heldItem != null && holding)//Check the players holding a item and the bool is set to be true 
                {
                    pointer.sprite = handClosed;//Showing the closed hand pointer
                }
                else if(hit.transform.gameObject.CompareTag("PickUp"))//Checking if the player looked at a item that can be picked up
                {
                    
                    canPickUp = true; //object can be picked up
                    lookedAtItem = hit.transform.gameObject;// tracking the game object the player is looking at

                    if (!holding && !detailsDisplaying)//Check if the player isn'y holding a object and the details aren't showing
                    {
                        pointer.sprite = handOpen;
                        ObjectInformationToolTip.GetPromptObject().GetComponent<PromptChanger>().dualPurpPrompt = true;
                        ObjectInformationToolTip.GetPromptObject().GetComponent<PromptChanger>().UpdateUI();
                        ObjectInformationToolTip.ShowPrompt();
                    }
                }
                else if(hit.transform.gameObject.CompareTag("Information"))//Checking if the player looked at a item that can be picked up
                {
                    canPickUp = false; //object can NOT be picked up
                    lookedAtItem = hit.transform.gameObject;// tracking the game object the player is looking at

                    if (!holding && !detailsDisplaying)//Check if the player isn'y holding a object and the details aren't showing
                    {
                        //pointer.sprite = handOpen; //We dont need to show a different cursor because you cant pick up this item
                        //ObjectInformationToolTip.instance.Get
                        ObjectInformationToolTip.GetPromptObject().GetComponent<PromptChanger>().dualPurpPrompt = false;
                        ObjectInformationToolTip.GetPromptObject().GetComponent<PromptChanger>().UpdateUI();
                        ObjectInformationToolTip.ShowPrompt();
                    }
                }
                else// All checks failed, item the ray is hitting can't be picked up
                {
                    canPickUp = false;
                    pointer.sprite = defaultPointer;
                    ObjectInformationToolTip.HidePrompt();
                    ObjectInformationToolTip.HideTip();
                    detailsDisplaying = false;
                    lookedAtItem = null;
                }
                
                
            }
            else// Ray wasn't cast and player isn't holding item, setting pointer to default and hiding all prompts from the player
            {
                pointer.sprite = defaultPointer;
                ObjectInformationToolTip.HidePrompt();
                ObjectInformationToolTip.HideTip();
                detailsDisplaying = false;
            }
            
        }
        else// Ray wasn't cast and player isn't holding item, setting pointer to default and hiding all prompts from the player
            {
                pointer.sprite = defaultPointer;
                ObjectInformationToolTip.HidePrompt();
                ObjectInformationToolTip.HideTip();
                detailsDisplaying = false;
            }

    }

    public void OnObjectDetailsDisplayed(InputAction.CallbackContext context)// Triggered when player presses details input
    {
        if(context.performed && !detailsDisplaying && !holding)// Check if button is pressed, details currently isn't displaying and the player isnt holding a object.
        {

            if(lookedAtItem != null)//Check to see if the player has looked at a item or not and if details should be displayed.
            {
                detailsDisplaying = true;
                if (lookedAtItem.GetComponent<ObjectInformation>() != null)
                {
                    ObjectInformationToolTip.ShowTip(lookedAtItem.GetComponent<ObjectInformation>().itemName, lookedAtItem.GetComponent<ObjectInformation>().itemStats);
                }
                else
                {
                    Debug.LogError("ObjectPickUp::Error at Line 89:: Could not find component 'ObjectInformation'");
                }
                ObjectInformationToolTip.HidePrompt();
            }
            else
            {
                //Debug.LogError("ObjectPickUp::Error with action:: lookedAtItem is null");
            }

        }
        else if (context.performed && detailsDisplaying && !holding)
        {
            ObjectInformationToolTip.HideTip();
            ObjectInformationToolTip.ShowPrompt();
            detailsDisplaying = false;
        }
    }

    public void OnObjectPickUp(InputAction.CallbackContext context)// Triggered when player presses the pick up input
    {
        if (!disabledInput && context.performed)// Making sure input is allowed and button press has been performed
        {
            if (canPickUp && !holding)
            {
                //can pick the item up and were not holding a item already
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

    public void PickUpItem(GameObject item)// Used to pick up a object
    {
        if(heldItem != null) // Make sure player isn't already holding an object
        {
            Debug.Log("Player cant pick up more that one item at a time...");
        }
        else// Updates the pick up script to move to the follow point and forces ToolTip to hide
        {
            item.GetComponent<PickUp>().pickedUp = true;
            item.GetComponent<PickUp>().pickupCounter++;
            item.GetComponent<PickUp>().SetHoldPoint(holdPoint);
            heldItem = item;
            holding = true;
            ObjectInformationToolTip.HidePrompt();
            ObjectInformationToolTip.HideTip();
            detailsDisplaying = false;
        }
    }

    public void DropItem(GameObject item)// Used to drop a item from the hold point
    {
        item.GetComponent<PickUp>().pickedUp = false;
        item.GetComponent<PickUp>().DropItem();
        heldItem = null;
        ObjectInformationToolTip.HideTip();
        lookedAtItem = null;
    }
    

}
