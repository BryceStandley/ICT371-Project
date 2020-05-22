using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ObjectiveUIElement : MonoBehaviour
{
    public Image hasCompletedTick;
    public Sprite tickSprite;
    public TextMeshProUGUI objectiveText;



    public void UpdateObjective(string obj)//Overload so we can use the same function name ot update the text and if the objective is complete
    {
        objectiveText.text = obj;
        hasCompletedTick.color = Color.clear;
    }

    public bool UpdateObjective(bool complete)
    {
        if(complete)
        {
            hasCompletedTick.sprite = tickSprite;
            hasCompletedTick.color = Color.white;
            objectiveText.fontStyle = FontStyles.Strikethrough;
            return true;
        }
        else
        {
            return false;
        }
    }
}
