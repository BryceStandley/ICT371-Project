using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PromptChanger : MonoBehaviour
{
    public static PromptChanger instance;
    public InputUISwitcher uiMaster;

    public string actionType;
    public TextMeshProUGUI promptText;
    public UIManager uiManager;
    public bool dualPurpPrompt = false;
    public string secondPromptType;
    public bool thirdPurpPrompt = false;
    public string thirdPromptType;
    public bool hasCustomName = false;
    public string customName;
    public CustomNameAction customNameAction = CustomNameAction.First;
    public enum CustomNameAction {First, Second, Third};

    private GameActions m_action, m_secondAction, m_thirdAction;

    private void Awake()
    {
        instance = this;
    }
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
            else if(uiManager.actions[i].actionName.ToLower().Equals(thirdPromptType.ToLower()))
            {
                m_thirdAction = uiManager.actions[i];
            }
        }
        uiMaster.AddToPromptList(this);
        UpdateUI();
        //ObjectInformationToolTip.HidePrompt();
    }

    private string CreatePromptString(bool gp, bool xb)
    {
        string outString;
        string customNameFirst;
        if(hasCustomName && customNameAction == CustomNameAction.First)
        {
            customNameFirst = customName;
        }
        else
        {
            customNameFirst = m_action.actionName;
        }


        if(!gp)
        {
            outString = customNameFirst + " [" + m_action.pcBinding + "] ";
        }
        else
        {
            if(!xb)
            {
                outString = customNameFirst + " [" + m_action.dualshockBinding + "] ";
            }
            else
            {
                outString = customNameFirst + " [" + m_action.xboxBinding + "] ";
            }
        }

        if (dualPurpPrompt)
        {
            string customNameSecond;
            if(hasCustomName && customNameAction == CustomNameAction.Second)
            {
                customNameSecond = customName;
            }
            else
            {
                customNameSecond = m_secondAction.actionName;
            }

            if (!gp)
            {
                outString += " " +customNameSecond + " [" + m_secondAction.pcBinding + "] ";
            }
            else
            {
                if (!xb)
                {
                    outString += " " +customNameSecond + " [" + m_secondAction.dualshockBinding + "] ";
                }
                else
                {
                    outString += " " +customNameSecond + " [" + m_secondAction.xboxBinding + "] ";
                }
            }
        }

        if (thirdPurpPrompt)//Going to assume the third purp of a prompt will be a custom named action
        {
            string customNameThird;
            if(hasCustomName && customNameAction == CustomNameAction.Third)
            {
                customNameThird = customName;
            }
            else
            {
                customNameThird = m_thirdAction.actionName;
            }

            if (!gp)
            {
                outString += " " +customNameThird + " [" + m_thirdAction.pcBinding + "] ";
            }
            else
            {
                if (!xb)
                {
                    outString += " " +customNameThird + " [" + m_thirdAction.dualshockBinding + "] ";
                }
                else
                {
                    outString += " " +customNameThird + " [" + m_thirdAction.xboxBinding + "] ";
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
