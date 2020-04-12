using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //This is an array of actions that store each sprite for each type of input
    public GameActions[] actions;




}

//This is a object type that stores an array of sprites. Each sprite is for the different types of input
// Index 0 = PC
// Index 1 = Xbox
// Index 2 = Playstation/Dualshock
[System.Serializable]
public class GameActions
{
    public string ActionName;
    public Sprite[] controlSprite;
    
    

    public GameActions()
    {
        ActionName = "NoName";
        controlSprite = new Sprite[3];
    }
}

