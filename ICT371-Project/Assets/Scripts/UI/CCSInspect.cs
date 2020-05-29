using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCSInspect : MonoBehaviour
{

    public GameObject inspectUI;
    public GameObject backButton;
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
        PauseMenu.instance.ChangeSelectedItem(backButton);
        PauseMenu.instance.inDialogue = true;
        PlayerInputController.instance.DisablePlayerControls();
        if(this.gameObject.name.Contains("1"))
        {
            //this is  ccs doc 1
            TrackingController.instance.playerViewedCCSDoc1 = true;
            thisDoc = 1;
        }
        else if(this.gameObject.name.Contains("2"))
        {
            //this is  ccs doc 2
            TrackingController.instance.playerViewedCCSDoc2 = true;
            thisDoc = 2;
        }
    }
    int thisDoc = 0;
    public void CloseInspect()
    {
        inspectUI.SetActive(false);
        PauseMenu.instance.ChangeSelectedItem(PauseMenu.instance.pauseFirstButton);
        PauseMenu.instance.inDialogue = false;
        PlayerInputController.instance.EnablePlayerControls();
        ObjectInformationToolTip.HideTip();
        ObjectInformationToolTip.HidePrompt();
        PuzzleManager.instance.CheckCCSDocumentDialogues(thisDoc);
    }

    public void ProvideLink() 
    {
        Application.OpenURL(documentLink);
    }
}
