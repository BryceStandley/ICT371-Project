using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedPacket : MonoBehaviour
{
    private void LateUpdate()
    {
        if(ObjectiveManager.instance.objectiveListType == ObjectiveManager.ObjectiveListType.Main)
        {
             if(ObjectPickUp.instance.lookedAtItem != null)
             {
                 if(ObjectPickUp.instance.lookedAtItem == this.gameObject)
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
}
