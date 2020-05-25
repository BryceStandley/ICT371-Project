using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FridgeMagnet : MonoBehaviour
{

    public GameObject inspectMagnetUI;
    private void Awake()
    {
        inspectMagnetUI.SetActive(false);
    }
    private void LateUpdate()
    {
        if(ObjectiveManager.instance.objectiveListType == ObjectiveManager.ObjectiveListType.Main)
        {
            if(ObjectPickUp.instance.lookedAtItem != null)
            {
                if(ObjectPickUp.instance.lookedAtItem.name.Contains("magnet"))
                {
                    PromptChanger.instance.thirdPurpPrompt = true;
                    PromptChanger.instance.hasCustomName = true;
                    PromptChanger.instance.customName = "Inspect";
                    PromptChanger.instance.customNameAction = PromptChanger.CustomNameAction.Third;
                    PromptChanger.instance.UpdateUI();
                    ActionManger.instance.SetCurrentAction("InspectMagnet");
                }
            }
        }
    }

    public void Inspect()
    {
        inspectMagnetUI.SetActive(true);
        PauseMenu.instance.inDialogue = true;
        PlayerInputController.instance.DisablePlayerControls();
    }

    public void CloseInspect()
    {
        inspectMagnetUI.SetActive(false);
        PauseMenu.instance.inDialogue = false;
        PlayerInputController.instance.EnablePlayerControls();
    }

}
