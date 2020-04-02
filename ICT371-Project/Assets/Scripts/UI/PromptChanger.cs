using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PromptChanger : MonoBehaviour
{
    private InputUISwitcher uiMaster;
    public Image promptImage;
    public Sprite xboxImage, psImage, pcImage, defaultGP;
    private void Awake()
    {
        uiMaster = FindObjectOfType<InputUISwitcher>();  
    }

    public void UpdateUI()
    {
        if(uiMaster.gamepad)
        {
            if(uiMaster.xbox)
            {
                promptImage.sprite = xboxImage;
            }
            else if(uiMaster.ps)
            {
                promptImage.sprite = psImage;
            }
            else if(!uiMaster.xbox && !uiMaster.ps)
            {
                promptImage.sprite = defaultGP;
            }
        }
        else
        {
            promptImage.sprite = pcImage;
        }
    }
}
