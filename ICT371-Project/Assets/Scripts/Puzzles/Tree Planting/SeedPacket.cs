using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedPacket : MonoBehaviour
{
    private ObjectPickUp objectPickUp;
    public ActionManger actionManger;

    public GameObject prompt;

    private void Awake()
    {
        objectPickUp = FindObjectOfType<ObjectPickUp>();
    }
    private void LateUpdate()
    {
        if(objectPickUp.heldItem != this.gameObject)
        {
            //hide prompt and dont allow input
            prompt.SetActive(false);
        }
        else if(objectPickUp.heldItem == this.gameObject)
        {
            //display prompt
            prompt.SetActive(true);
            //tell action manager that picking up the seed is the current action available
            actionManger.SetCurrentAction("TakeSeed");
        }
    }

}
