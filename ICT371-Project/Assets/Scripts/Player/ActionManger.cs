﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionManger : MonoBehaviour
{
    // Using a string as a rewritable buffer for a action instruction
    // This could be converted into a queue or list but a string works just as well
    // Doing this allows us to check if the given action is the same as a action we have in our array of actions
    // this is a check to ensure that puzzle only actions can be triggered when needed.

    public static ActionManger instance;
    private string actionAvailable;

    private string[] actions = { "TakeSeed", "Unplug" , "BuyFood", "InspectMagnet", "InspectDoc", "TakeCar", "TakeBike"};

    public GameObject seed;
    public CCSInspect docToInspect;

    private void Awake()//using clear function to set up empty string
    {
        instance = this;
        
    }
    private void Start()
    {
        ClearCurrentAction();
    }

   

    public void OnActionInput(InputAction.CallbackContext context)//This function is called whrn the action button is pressed
    {
        if(context.performed)// checking if the button press was performed
        {
            //Debug.Log("action button pressed");
            if (!actionAvailable.Equals(""))//check if there is a action available, could change this to a list or queue over a string check
            {
                //Debug.Log("Action is availabe");
                if (actionAvailable.ToLower().Contains(actions[0].ToLower()))//convert strings to lower and check if they match a given action
                {
                    TakeSeed();
                    ClearCurrentAction();
                }
                else if(actionAvailable.ToLower().Contains(actions[1].ToLower()))
                {
                    UnplugCable();
                    ClearCurrentAction();
                }
                else if(actionAvailable.ToLower().Contains(actions[2].ToLower()))
                {
                    BuyFood();
                    ClearCurrentAction();
                }
                else if(actionAvailable.ToLower().Contains(actions[3].ToLower()))
                {
                    InspectMagnet();
                    ClearCurrentAction();
                }
                else if (actionAvailable.ToLower().Contains(actions[4].ToLower()))
                {
                    InspectDoc();
                    ClearCurrentAction();
                }
                else if (actionAvailable.ToLower().Contains(actions[5].ToLower()))
                {
                    TakeCar();
                    ClearCurrentAction();
                }
                else if (actionAvailable.ToLower().Contains(actions[6].ToLower()))
                {
                    TakeBike();
                    ClearCurrentAction();
                }
            }
        }
    }

    private void LateUpdate()
    {   
    }

    public void SetCurrentAction(string action)
    {
        actionAvailable = action;
    }

    public void ClearCurrentAction()
    {
        //Debug.Log("Action Cleared");
        actionAvailable = "";
        docToInspect = null;
        PromptChanger.instance.thirdPurpPrompt = false;
        PromptChanger.instance.hasCustomName = false;
        PromptChanger.instance.UpdateUI();
    }

    private void TakeSeed()
    {
        if(ObjectPickUp.instance.heldItem != null)
        {
            ObjectPickUp.instance.DropItem(ObjectPickUp.instance.heldItem);
        }
        seed.GetComponent<Rigidbody>().useGravity = true;
        ObjectPickUp.instance.PickUpItem(seed);
    }

    private void UnplugCable()
    {   
        if(ObjectPickUp.instance.lookedAtItem != null)
        {
            PowerSocket ps = ObjectPickUp.instance.lookedAtItem.GetComponent<PowerSocket>();
            if(ps != null)
            {
                ps.UnplugItem();
            }
            else
            {
                Debug.Log("not looking at a power socket");
                ClearCurrentAction();
            }
        }

    }

    private void BuyFood()
    {
        if(ObjectPickUp.instance.lookedAtItem != null)
        {
            FoodOrderer foodOrderer = ObjectPickUp.instance.lookedAtItem.GetComponent<FoodOrderer>();
            if(foodOrderer != null)
            {
                foodOrderer.BuyFood();
            }
            else
            {
                Debug.Log("not looking at a food orderer");
                ClearCurrentAction();
            }
        }
    }

    private void InspectMagnet()
    {
        if(ObjectPickUp.instance.lookedAtItem != null)
        {
            FridgeMagnet magnet = ObjectPickUp.instance.lookedAtItem.GetComponent<FridgeMagnet>();
            if(magnet != null)
            {
                magnet.Inspect();
            }
            else
            {
                Debug.Log("not looking at a fridge magnet");
                ClearCurrentAction();
            }
        }
    }

    private void InspectDoc()
    {
        if (ObjectPickUp.instance.lookedAtItem != null)
        {
            if (docToInspect != null)
            {
                Debug.Log("inspect Doc");
                docToInspect.Inspect();
            }
            else
            {
                Debug.Log("not looking at doc");
                ClearCurrentAction();
            }
        }
    
    }

    private void TakeCar()
    {
        PuzzleManager.instance.TriggerEndObjective();
        TrackingController.instance.AddTransportUsage(TrackingController.TransportType.Car);
        //trigger end cinematic
        //trigger final objective
        //trigger final scoring
        //Show Scoring
    }

    private void TakeBike()
    {
        PuzzleManager.instance.TriggerEndObjective();
        TrackingController.instance.AddTransportUsage(TrackingController.TransportType.Bike);
        //trigger end cinematic
        //trigger final objective
        //trigger final scoring
        //Show Scoring
        
    }
}
