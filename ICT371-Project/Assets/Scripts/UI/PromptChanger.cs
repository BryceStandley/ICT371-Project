using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PromptChanger : MonoBehaviour
{
    public InputUISwitcher uiMaster;
    public string ActionType;
    public Image promptImage;
    public UIManager uiManager;
    private GameActions m_action;
    private void Awake()
    {
        
        for(int i = 0; i < uiManager.actions.Length; i++)
        {
            //Searching though the actions to allocate this to the correct action
            //setting both strings to lower case and checking if they equal each other
            if(uiManager.actions[i].ActionName.ToLower().Equals(ActionType.ToLower()))
            {
                m_action = uiManager.actions[i];
            }
        }
    }

    //0 = PC
    //1 = Xbox
    //2 = PS
    public void UpdateUI()
    {
        if(uiMaster.gamepad)
        {
            if(uiMaster.xbox)
            {
                promptImage.sprite = m_action.controlSprite[1];
            }
            else if(uiMaster.ps)
            {
                promptImage.sprite = m_action.controlSprite[2];
            }
            else if(!uiMaster.xbox && !uiMaster.ps)
            {
                promptImage.sprite = m_action.controlSprite[0];
            }
        }
        else
        {
            promptImage.sprite = m_action.controlSprite[0];
        }
    }
}
