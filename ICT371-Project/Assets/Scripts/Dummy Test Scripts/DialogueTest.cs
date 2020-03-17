using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTest : MonoBehaviour
{
    public InputMaster controls;

    private DialogueTrigger dialogueTrigger;

    private void Awake()
    {
        dialogueTrigger = this.gameObject.GetComponent<DialogueTrigger>();
        controls = new InputMaster();//Creating a new inputMaster component
        controls.TestKeys.DialogueTrigger.performed += ctx => Trigger();// Assigning the trigger key to the trigger script


    }

    private void OnEnable()
    {
        controls.TestKeys.Enable();
    }

    private void OnDisable()
    {
        controls.TestKeys.Disable();
    }

    private void Trigger()
    {
        //Debug.Log("triggering dialogue...");
        dialogueTrigger.TriggerDialogue();
    }
}
