using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FridgeMagnet : MonoBehaviour
{

    public GameObject inspectMagnetUI;
    public GameObject closeButton;

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
        PauseMenu.instance.ChangeSelectedItem(closeButton);
        PauseMenu.instance.inDialogue = true;
        PlayerInputController.instance.DisablePlayerControls();
        
    }

    public void CloseInspect()
    {
        inspectMagnetUI.SetActive(false);
        PauseMenu.instance.inDialogue = false;
        PauseMenu.instance.ChangeSelectedItem(PauseMenu.instance.pauseFirstButton);
        PlayerInputController.instance.EnablePlayerControls();
        ObjectInformationToolTip.HideTip();
        ObjectInformationToolTip.HidePrompt();
        
    }

}
