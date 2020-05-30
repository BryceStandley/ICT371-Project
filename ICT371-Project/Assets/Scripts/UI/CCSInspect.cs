using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCSInspect : MonoBehaviour
{

    public GameObject inspectUI;
    public GameObject backButton;
    public string documentLink;
    public GameObject[] pages;
    private int currentIndex = 0;
    public int docNumber = 0;
    public bool viewed = false;

    private void Start()
    {
        PuzzleManager.instance.AddCCSDocument(this);
    }

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
        viewed = true;
        if(docNumber == 1)
        {
            //this is  ccs doc 1
            TrackingController.instance.playerViewedCCSDoc1 = true;
        }
        else if(docNumber == 2)
        {
            //this is  ccs doc 2
            TrackingController.instance.playerViewedCCSDoc2 = true;
        }
    }

    public void CloseInspect()
    {
        inspectUI.SetActive(false);
        PauseMenu.instance.ChangeSelectedItem(PauseMenu.instance.pauseFirstButton);
        PauseMenu.instance.inDialogue = false;
        PlayerInputController.instance.EnablePlayerControls();
        ObjectInformationToolTip.HideTip();
        ObjectInformationToolTip.HidePrompt();
        PuzzleManager.instance.CheckCCSDocumentDialogues();
    }

    public void ProvideLink() 
    {
        Application.OpenURL(documentLink);
    }

    public void NextPage()
    {
        if(currentIndex < pages.Length - 1)
        {
            
            pages[currentIndex].SetActive(false);
            currentIndex++;
            pages[currentIndex].SetActive(true);
            

        }
    }

    public void PreviousPage()
    {
        if(currentIndex > 0)
        {
            pages[currentIndex].SetActive(false);
            currentIndex--;
            pages[currentIndex].SetActive(true);
        }
    }
}
