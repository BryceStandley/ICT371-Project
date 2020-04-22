using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueTest : MonoBehaviour
{
    public InputMaster controls;

    private DialogueTrigger dialogueTrigger;

    private void Awake()
    {
        dialogueTrigger = FindObjectOfType<DialogueTrigger>();
    }

    public void Trigger(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            dialogueTrigger.TriggerDialogue();
        }
    }
}
