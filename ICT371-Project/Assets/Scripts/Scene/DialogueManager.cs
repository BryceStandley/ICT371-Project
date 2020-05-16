using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{   
    public static DialogueManager instance;

    //Reference to allow player input to be turned on and off
    public PlayerInputController playerInputController;

    public TextMeshProUGUI dialogueName;
    public TextMeshProUGUI dialogueSentence;
    public GameObject continueButton;


    public RectTransform dialogueBox;

    private GameObject originalSelectedObject;
    private Queue<string> sentences;
    private EventSystem es;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        sentences = new Queue<string>();
        playerInputController = FindObjectOfType<PlayerInputController>();
        es = FindObjectOfType<EventSystem>();
    }

    public void StartDialogue(Dialogue dialogue)//Method triggered when a dialogue is triggered
	{
        
        dialogueName.text = dialogue.npcName;

        playerInputController.DisablePlayerControls();//Stopping the player from being able to look and move

        sentences.Clear();//Clearing the sentence queue of any old sentences

        foreach(string sentence in dialogue.sentences)//looping though each sentence and adding it to the queue to be displayed
        {
            sentences.Enqueue(sentence);
        }
        dialogueBox.DOAnchorPos(new Vector2(0, 125), 0.5f);//Animates dialogue box into the cameras view
        ChangeSelectedItem();//Sets Selected Item to the continue button
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
        es.SetSelectedGameObject(originalSelectedObject);
        playerInputController.EnablePlayerControls();
        
        
    }

    private void ChangeSelectedItem()
    { 
        originalSelectedObject = es.currentSelectedGameObject;
        es.SetSelectedGameObject(continueButton);
    }
}
