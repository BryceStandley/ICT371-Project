using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCSDoc2 : MonoBehaviour
{

    public GameObject inspectCCS2UI;
    private void Awake()
    {
        inspectCCS2UI.SetActive(false);
    }
    private void LateUpdate()
    {

        if (ObjectPickUp.instance.lookedAtItem != null)
        {
            if (ObjectPickUp.instance.lookedAtItem.name.Contains("Doc 2"))
            {
                PromptChanger.instance.thirdPurpPrompt = true;
                PromptChanger.instance.hasCustomName = true;
                PromptChanger.instance.customName = "Inspect";
                PromptChanger.instance.customNameAction = PromptChanger.CustomNameAction.Third;
                PromptChanger.instance.UpdateUI();
                ActionManger.instance.SetCurrentAction("InspectCCS2");
            }
        }
    }

    public void Inspect()
    {
        inspectCCS2UI.SetActive(true);
        PauseMenu.instance.inDialogue = true;
        PlayerInputController.instance.DisablePlayerControls();
    }

    public void CloseInspect()
    {
        inspectCCS2UI.SetActive(false);
        PauseMenu.instance.inDialogue = false;
        PlayerInputController.instance.EnablePlayerControls();
    }

    public void ProvideLink()
    {
        Application.OpenURL("");
    }
}
