using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIManager : MonoBehaviour
{

    public List<GameActions> actions;

}

//This is a class that is used to create a binding tracker for the UI
//This is used in conjunction with the UI Switcher to ensure the players UI
//prompts reflect the same gamepad or input device that their using.
[System.Serializable]
public class GameActions
{
    [SerializeField]
    public string actionName, pcBinding, xboxBinding, dualshockBinding;
  
    public GameActions()
    {
        actionName = "Not Defined";
        pcBinding = "Not Defined";
        xboxBinding = "Not Defined";
        dualshockBinding = "Not Defined";
    }

    public GameActions(string newActionName, string pc, string xbox, string dualshock)
    {
        actionName = newActionName;
        pcBinding = pc;
        xboxBinding = xbox;
        dualshockBinding = dualshock;
    }
}

