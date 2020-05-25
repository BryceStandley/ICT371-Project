using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCSDoc1 : MonoBehaviour
{

    public GameObject inspectCCS1UI;
    private void Awake()
    {
        inspectCCS1UI.SetActive(false);
    }
    private void LateUpdate()
    {
        
            if (ObjectPickUp.instance.lookedAtItem != null)
            {
                if (ObjectPickUp.instance.lookedAtItem.name.Contains("Doc 1"))
                {
                    PromptChanger.instance.thirdPurpPrompt = true;
                    PromptChanger.instance.hasCustomName = true;
                    PromptChanger.instance.customName = "Inspect";
                    PromptChanger.instance.customNameAction = PromptChanger.CustomNameAction.Third;
                    PromptChanger.instance.UpdateUI();
                    ActionManger.instance.SetCurrentAction("InspectCCS1");
                }
            }
    }
   
    public void Inspect()
    {
        inspectCCS1UI.SetActive(true);
        PauseMenu.instance.inDialogue = true;
        PlayerInputController.instance.DisablePlayerControls();
    }

    public void CloseInspect()
    {
        inspectCCS1UI.SetActive(false);
        PauseMenu.instance.inDialogue = false;
        PlayerInputController.instance.EnablePlayerControls();
    }

    public void ProvideLink() 
    {
        Application.OpenURL("https://climate.nasa.gov/faq/14/is-the-sun-causing-global-warming/");
    }
}
