using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodOrderer : MonoBehaviour
{
    public BuyFood buy;
    public bool foodBought = false;
private void LateUpdate()
    {
        if(ObjectiveManager.instance.objectiveListType == ObjectiveManager.ObjectiveListType.Main)
        {
            if(ObjectPickUp.instance.lookedAtItem != null)
            {
                if(ObjectPickUp.instance.lookedAtItem == this.gameObject)
                {
                    if(!foodBought)
                    {
                        PromptChanger.instance.thirdPurpPrompt = true;
                        PromptChanger.instance.hasCustomName = true;
                        PromptChanger.instance.customName = "Order Food";
                        PromptChanger.instance.customNameAction = PromptChanger.CustomNameAction.Third;
                        PromptChanger.instance.UpdateUI();
                        ActionManger.instance.SetCurrentAction("BuyFood");
                    }
                }
            }
        }
    }

    public void BuyFood()
    {
        buy.OpenFoodBuyUI();
    }
}
