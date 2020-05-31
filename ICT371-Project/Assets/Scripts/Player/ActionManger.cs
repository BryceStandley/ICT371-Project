using System.Collections;
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

    public Dialogue tookBikeDialogue, tookCarDialogue;
    public Collider bikeCollider, carCollider;
    public bool finalTrigger = false;
    private bool endTriggered = false;

    private void Awake()//using clear function to set up empty string
    {
        instance = this;
        
    }
    private void Start()
    {
        ClearCurrentAction();
    }

    private void Update()
    {
        if(endDialogueFinished)
        {
            if(!endTriggered)
            {
                endTriggered = true;
                End();   
            }
        }
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
                    SoundEffectsManager.instance.PlayActionClickClip();
                    ClearCurrentAction();
                }
                else if(actionAvailable.ToLower().Contains(actions[1].ToLower()))
                {
                    UnplugCable();
                    SoundEffectsManager.instance.PlayActionClickClip();
                    ClearCurrentAction();
                }
                else if(actionAvailable.ToLower().Contains(actions[2].ToLower()))
                {
                    BuyFood();
                    SoundEffectsManager.instance.PlayActionClickClip();
                    ClearCurrentAction();
                }
                else if(actionAvailable.ToLower().Contains(actions[3].ToLower()))
                {
                    InspectMagnet();
                    SoundEffectsManager.instance.PlayActionClickClip();
                    ClearCurrentAction();
                }
                else if (actionAvailable.ToLower().Contains(actions[4].ToLower()))
                {
                    InspectDoc();
                    SoundEffectsManager.instance.PlayActionClickClip();
                    ClearCurrentAction();
                }
                else if (actionAvailable.ToLower().Contains(actions[5].ToLower()))
                {
                    finalTrigger = true;
                    tookBike = false;
                    carCollider.enabled = false;
                    ClearCurrentAction();
                    TrackingController.instance.AddTransportUsage(TrackingController.TransportType.Car);
                    TrackingController.instance.typeOfTransportThePlayerUsed = TrackingController.TransportType.Car;
                    PlayerInputController.instance.DisablePlayerControls();
                    DialogueManager.instance.StartDialogue(tookCarDialogue);
                    SoundEffectsManager.instance.PlayActionClickClip();
                    
                }
                else if (actionAvailable.ToLower().Contains(actions[6].ToLower()))
                {
                    finalTrigger = true;
                    tookBike = true;
                    bikeCollider.enabled = false;
                    ClearCurrentAction();
                    TrackingController.instance.AddTransportUsage(TrackingController.TransportType.Bike);
                    TrackingController.instance.typeOfTransportThePlayerUsed = TrackingController.TransportType.Bike;
                    PlayerInputController.instance.DisablePlayerControls();
                    DialogueManager.instance.StartDialogue(tookBikeDialogue);
                    SoundEffectsManager.instance.PlayActionClickClip();
                }
            }
        }
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
                ObjectInformationToolTip.HidePrompt();
                ObjectInformationToolTip.HideTip();
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
                ObjectInformationToolTip.HidePrompt();
                ObjectInformationToolTip.HideTip();
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
                ObjectInformationToolTip.HidePrompt();
                ObjectInformationToolTip.HideTip();
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
                docToInspect.Inspect();
                ObjectInformationToolTip.HidePrompt();
                ObjectInformationToolTip.HideTip();
            }
            else
            {
                Debug.Log("not looking at doc");
                ClearCurrentAction();
            }
        }
    
    }

    public GameObject finalCinematicCamera, playerCamera, playerModel, gamePlayUI, dialogues, objectHoldPoint;
    public bool endDialogueFinished = false;
    public bool tookBike = false;
    private void End()
    { 
        playerCamera.GetComponent<Camera>().enabled = false;
        playerCamera.GetComponent<AudioListener>().enabled = false;
        gamePlayUI.SetActive(false);
        dialogues.SetActive(false);
        objectHoldPoint.SetActive(false);
        finalCinematicCamera.SetActive(true);
        //PuzzleManager.instance.TriggerEndObjective();
        FinalScoring.instance.TriggerFinalScoring();
        FinalScoring.instance.DisplayFinalScoreUI();
    }

}
