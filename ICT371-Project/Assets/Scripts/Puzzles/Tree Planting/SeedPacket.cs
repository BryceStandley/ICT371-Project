using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedPacket : MonoBehaviour
{
    private ObjectPickUp objectPickUp;
    public ActionManger actionManger;
    public PromptChanger mainPromptChanger;

    public GameObject secondaryPrompt;

    private void Awake()
    {
        objectPickUp = FindObjectOfType<ObjectPickUp>();
    }
    private void LateUpdate()
    {
        if(objectPickUp.heldItem != this.gameObject)
        {
            //hide prompt and dont allow input
            secondaryPrompt.SetActive(false);
        }
        else if(objectPickUp.heldItem == this.gameObject)
        {
            //display prompt
            secondaryPrompt.SetActive(true);
            //tell action manager that picking up the seed is the current action available
            actionManger.SetCurrentAction("TakeSeed");
        }
        
        if(objectPickUp.lookedAtItem != null)
        {
            if(objectPickUp.lookedAtItem.name.ToLower() == "seed packet")
            {
                mainPromptChanger.thirdPurpPrompt = true;
                mainPromptChanger.hasCustomName = true;
                mainPromptChanger.customName = "Take Seed";
                mainPromptChanger.customNameAction = PromptChanger.CustomNameAction.Third;
                mainPromptChanger.UpdateUI();
                actionManger.SetCurrentAction("TakeSeed");
            }
            else
            {
                mainPromptChanger.thirdPurpPrompt = false;
                mainPromptChanger.hasCustomName = false;
                mainPromptChanger.UpdateUI();
            }
        }
    }

}
