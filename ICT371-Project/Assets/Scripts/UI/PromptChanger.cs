using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PromptChanger : MonoBehaviour
{
    public InputUISwitcher uiMaster;
    public string actionType;
    public TextMeshProUGUI promptText;
    public UIManager uiManager;
    public bool dualPurpPrompt = false;
    public string secondPromptType;
    public bool hasCustomName = false;
    public string customName;

    private GameActions m_action, m_secondAction;
    private void Start()
    { 
        for(int i = 0; i < uiManager.actions.Count; i++)
        {
            //Searching though the actions to allocate this to the correct action
            //setting both strings to lower case and checking if they equal each other
            if(uiManager.actions[i].actionName.ToLower().Equals(actionType.ToLower()))
            {
                m_action = uiManager.actions[i];
            }
            else if(uiManager.actions[i].actionName.ToLower().Equals(secondPromptType.ToLower()))
            {
                m_secondAction = uiManager.actions[i];
            }
        }
        uiMaster.AddToPromptList(this);
        UpdateUI();
        //ObjectInformationToolTip.HidePrompt();
    }

    private string CreatePromptString(bool gp, bool xb)
    {
        string outString;

        if(!hasCustomName)
        {
            customName = m_action.actionName;   
        }


        if(!gp)
        {
            outString = customName + " [" + m_action.pcBinding + "] ";
        }
        else
        {
            if(!xb)
            {
                outString = customName + " [" + m_action.dualshockBinding + "] ";
            }
            else
            {
                outString = customName + " [" + m_action.xboxBinding + "] ";
            }
        }

        if (dualPurpPrompt)
        {
            if (!gp)
            {
                outString += " " +m_secondAction.actionName + " [" + m_secondAction.pcBinding + "] ";
            }
            else
            {
                if (!xb)
                {
                    outString += " " +m_secondAction.actionName + " [" + m_secondAction.dualshockBinding + "] ";
                }
                else
                {
                    outString += " " +m_secondAction.actionName + " [" + m_secondAction.xboxBinding + "] ";
                }
            }
        }

        return outString;

    }


    public void UpdateUI()
    {
        //Debug.Log("Update ui triggered");
        if(uiMaster.gamepad)
        {
            if(uiMaster.xbox)
            {
                promptText.text = CreatePromptString(true, true);
            }
            else if(uiMaster.ps)
            {
                promptText.text = CreatePromptString(true, false);
            }
            else if(!uiMaster.xbox && !uiMaster.ps)
            {
                promptText.text = CreatePromptString(true, true);
            }
        }
        else
        {
            promptText.text = CreatePromptString(false, false);
        }
    }
}
