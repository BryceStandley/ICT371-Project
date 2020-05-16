using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "DialogueItem", order = 0)]

public class Dialogue : ScriptableObject 
{
    public string dialogueTriggerName;
    public string npcName;

    [TextArea(3,10)]
    public string[] sentences;
}
