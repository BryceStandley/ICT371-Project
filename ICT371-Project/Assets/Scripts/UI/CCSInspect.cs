using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCSInspect : MonoBehaviour
{

    public GameObject inspectUI;
    public string documentLink;
    private void LateUpdate()
    {
        
            if (ObjectPickUp.instance.lookedAtItem != null)
            {
                if (ObjectPickUp.instance.lookedAtItem == this.gameObject)
                {
                    PromptChanger.instance.thirdPurpPrompt = true;
                    PromptChanger.instance.hasCustomName = true;
                    PromptChanger.instance.customName = "Inspect";
                    PromptChanger.instance.customNameAction = PromptChanger.CustomNameAction.Third;
                    PromptChanger.instance.UpdateUI();
                    ActionManger.instance.docToInspect = this;
                    ActionManger.instance.SetCurrentAction("InspectDoc");
                }
            }
    }
   
    public void Inspect()
    {
        inspectUI.SetActive(true);
        PauseMenu.instance.inDialogue = true;
        PlayerInputController.instance.DisablePlayerControls();
    }

    public void CloseInspect()
    {
        inspectUI.SetActive(false);
        PauseMenu.instance.inDialogue = false;
        PlayerInputController.instance.EnablePlayerControls();
    }

    public void ProvideLink() 
    {
        Application.OpenURL(documentLink);
    }
}
