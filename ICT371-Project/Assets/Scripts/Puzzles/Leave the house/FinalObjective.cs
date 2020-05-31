using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalObjective : MonoBehaviour
{
    public bool isCar = false;
    private void LateUpdate()
    {
        if(ObjectiveManager.instance.objectiveListType == ObjectiveManager.ObjectiveListType.End)
        {
            if(ObjectPickUp.instance.lookedAtItem != null)
            {
                if(ObjectPickUp.instance.lookedAtItem == this.gameObject)
                {
                    if(!ActionManger.instance.finalTrigger)
                    {
                        if (isCar)
                        {
                            PromptChanger.instance.thirdPurpPrompt = true;
                            PromptChanger.instance.hasCustomName = true;
                            PromptChanger.instance.customName = "Take Car";
                            PromptChanger.instance.customNameAction = PromptChanger.CustomNameAction.Third;
                            PromptChanger.instance.UpdateUI();
                            ActionManger.instance.SetCurrentAction("TakeCar");
                        }
                        else
                        {
                            PromptChanger.instance.thirdPurpPrompt = true;
                            PromptChanger.instance.hasCustomName = true;
                            PromptChanger.instance.customName = "Take Bike";
                            PromptChanger.instance.customNameAction = PromptChanger.CustomNameAction.Third;
                            PromptChanger.instance.UpdateUI();
                            ActionManger.instance.SetCurrentAction("TakeBike");
                        }
                    }
                }
            }
        }
    }
}
