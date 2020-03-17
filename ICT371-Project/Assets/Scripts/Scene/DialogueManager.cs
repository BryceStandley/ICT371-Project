using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class DialogueManager : MonoBehaviour
{
    //References to allow player input to be turned on and off
    public PlayerController playerController;
    public PlayerLook playerLook;

    public TextMeshProUGUI dialogueName;
    public TextMeshProUGUI dialogueSentence;

    public RectTransform dialogueBox;

    private Queue<string> sentences;

    void Start()
    {
        sentences = new Queue<string>();
        playerLook = FindObjectOfType<PlayerLook>();
        playerController = FindObjectOfType<PlayerController>();
    }

    public void StartDialogue(Dialogue dialogue)//Method triggered when a dialogue is triggered
	{
        
        dialogueName.text = dialogue.name;

        DisablePlayerControls();//Stopping the player from being able to look and move

        sentences.Clear();//Clearing the sentence queue of any old sentences

        foreach(string sentence in dialogue.sentences)//looping though each sentence and adding it to the queue to be displayed
        {
            sentences.Enqueue(sentence);
        }
        dialogueBox.DOAnchorPos(new Vector2(0, 125), 0.5f);//Animates dialogue box into the cameras view
        DisplayNextSentence();//Triggeres display method
	}

    public void DisplayNextSentence()//Displays the next sentence in the dialogue queue
    {
        if(sentences.Count == 0)//Check if there are any more sentences to display
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();//storing the next sentence and getting ready to display
        dialogueSentence.text = sentence;
    }

    public void EndDialogue()//Cleans up and disables dialogue box
    {
        //Debug.Log("Dialogue Ended...");
        dialogueBox.DOAnchorPos(new Vector2(0, -125), 0.5f);//Animates dialogue box out of cameras view
        EnablePlayerControls();
        
    }

    public void DisablePlayerControls()//Disables player input to look and move, also shows the cursor
    {
        playerController.enabled = false;
        playerLook.enabled = false;
        Cursor.lockState = CursorLockMode.None;
    }

    public void EnablePlayerControls()//Inverts Disable controls method
    {
        playerController.enabled = true;
        playerLook.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
