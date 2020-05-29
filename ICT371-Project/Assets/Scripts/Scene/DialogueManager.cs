using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    #region  Variables   
    public static DialogueManager instance;
    public GameObject dialogueUIElement;
    public TextMeshProUGUI dialogueName;
    public TextMeshProUGUI dialogueSentence;
    public GameObject continueButton;
    public RectTransform dialogueBox;
    private GameObject originalSelectedObject;
    [SerializeField]
    private Queue<string> sentences;
    public EventSystem es;
    public PauseMenu pauseMenu;
    public List<string> tempList;

    public bool inDialogue = false;
    public Dialogue tookBikeDialogue, tookCarDialogue;
    public Dialogue currentDialogue;
    #endregion
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        sentences = new Queue<string>();
        tempList = new List<string>();
        dialogueUIElement.SetActive(false);

    }

    public void StartDialogue(Dialogue dialogue)//Method triggered when a dialogue is triggered
	{
        currentDialogue = dialogue;
        if(inDialogue)
        {
            if(dialogue != null)
            {
                foreach(string sentence in dialogue.sentences)
                {
                    sentences.Enqueue(sentence);
                    tempList.Add(sentence);
                }
            }
        }
        else
        {
            inDialogue = true;
            InitDialogue(dialogue);
        }
	}

    private void InitDialogue(Dialogue dialogue)//Supporting 2 dialogues at once
    {
        pauseMenu.inDialogue = true;
        dialogueUIElement.SetActive(true);
        PauseMenu.instance.ChangeSelectedItem(continueButton);
        dialogueName.text = dialogue.npcName;

        PlayerInputController.instance.DisablePlayerControls();//Stopping the player from being able to look and move

        sentences.Clear();//Clearing the sentence queue of any old sentences

        foreach(string sentence in dialogue.sentences)//looping though each sentence and adding it to the queue to be displayed
        {
            sentences.Enqueue(sentence);
            tempList.Add(sentence);
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
        inDialogue = false;
        tempList.Clear();
        PlayerInputController.instance.EnablePlayerControls();
        ObjectInformationToolTip.HidePrompt();
        ObjectInformationToolTip.HideTip();
        Invoke("DisableDialogue",0.6f);
        if(currentDialogue == tookCarDialogue || currentDialogue == tookBikeDialogue)
        {
            ActionManger.instance.endDialogueFinished = true;
        }
        else
        {
            currentDialogue = null;
        }
        
        
        
    }

    private void DisableDialogue()
    {
        dialogueUIElement.SetActive(false);
        pauseMenu.inDialogue = false;
        PauseMenu.instance.ChangeSelectedItem(PauseMenu.instance.pauseFirstButton);

    }

    private void ChangeSelectedItem()
    { 
        originalSelectedObject = es.currentSelectedGameObject;
        es.SetSelectedGameObject(continueButton);
    }
}
