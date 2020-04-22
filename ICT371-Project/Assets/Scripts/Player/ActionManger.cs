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

    private string actionAvailable;

    private string[] actions = { "TakeSeed" };

    public GameObject seed;

    private void Awake()//using clear function to set up empty string
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
                if (actionAvailable.ToLower().Equals(actions[0].ToLower()))//convert strings to lower and check if they match a given action
                {
                    TakeSeed();
                    ClearCurrentAction();
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
        actionAvailable = "";
    }

    private void TakeSeed()
    {
        ObjectPickUp objectPickUp = FindObjectOfType<ObjectPickUp>();
        objectPickUp.DropItem(objectPickUp.heldItem);
        seed.GetComponent<Rigidbody>().useGravity = true;
        objectPickUp.PickUpItem(seed);
    }
}
