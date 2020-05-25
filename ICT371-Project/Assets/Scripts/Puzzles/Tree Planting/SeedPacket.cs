using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedPacket : MonoBehaviour
{
    public GameObject secondaryPrompt;
    private void LateUpdate()
    {
        if(ObjectPickUp.instance.heldItem != this.gameObject)
        {
            //hide prompt and dont allow input
            secondaryPrompt.SetActive(false);
        }
        else if(ObjectPickUp.instance.heldItem == this.gameObject)
        {
            //display prompt
            secondaryPrompt.SetActive(true);
            //tell action manager that picking up the seed is the current action available
            ActionManger.instance.SetCurrentAction("TakeSeed");
        }
        
        if(ObjectPickUp.instance.lookedAtItem != null)
        {
            if(ObjectPickUp.instance.lookedAtItem.name.ToLower() == "seed packet")
            {
                PromptChanger.instance.thirdPurpPrompt = true;
                PromptChanger.instance.hasCustomName = true;
                PromptChanger.instance.customName = "Take Seed";
                PromptChanger.instance.customNameAction = PromptChanger.CustomNameAction.Third;
                PromptChanger.instance.UpdateUI();
                ActionManger.instance.SetCurrentAction("TakeSeed");
            }
        }
    }

}
