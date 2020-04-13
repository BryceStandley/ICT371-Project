using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionManger : MonoBehaviour
{
    private string actionAvailable;

    private string[] actions = { "TakeSeed" };

    public GameObject seed;

    public void OnActionInput(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            Debug.Log("action button pressed");
            if (actionAvailable != "")
            {
                Debug.Log("Action is availabe");
                if (actionAvailable.ToLower().Equals(actions[0].ToLower()))
                {
                    TakeSeed();
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
