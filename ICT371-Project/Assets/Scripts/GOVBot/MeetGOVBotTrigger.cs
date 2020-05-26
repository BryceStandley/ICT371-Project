using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeetGOVBotTrigger : MonoBehaviour
{
    public GOVBot govBot;
    public Dialogue meetGovBotDialogue;
    private bool startCheckingEndDialogue = false;
    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            DialogueManager.instance.StartDialogue(meetGovBotDialogue);
            startCheckingEndDialogue = true;
        }
    }

    private void Update()
    {
        if(startCheckingEndDialogue)
        {
            if(!DialogueManager.instance.inDialogue)
            {
                //Dialogue ended, delete this trigger
                govBot.hasMeetGovBot = true;
                PuzzleManager.instance.CheckIfSeenGOVBot();
                Destroy(this.gameObject);
            }
        }
    }
}
