using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputUISwitcher : MonoBehaviour
{
    //This is where the global controll of UI prompts are controlled
    //This will check and toggle what controller or input is being used
    //and set values that each ui will find and adjust automaticly
    public static InputUISwitcher instance;
    public bool xbox = false;
    public bool ps = false;
    public bool gamepad = false;
    public bool pc = true;

    //public PlayerInput input;

    private List<PromptChanger> prompts;

    private void Awake()
    {
        instance = this;
        prompts = new List<PromptChanger>();
        
        //input.onControlsChanged += Input_onControlsChanged;

        //prompts = FindObjectsOfType<PromptChanger>(); // Finds all prompts in the level and adds them to a array
    }


    public void AddToPromptList(PromptChanger prompt)
    {
        prompts.Add(prompt);
    }

    public void OnInputChange(string inputName)// this is triggered when the input device is changed
    {
        if(inputName.ToLower().Contains("gamepad") || inputName.ToLower().Contains("dualshock"))
        {
            //Set all ui elements to gamepad
            //Debug.Log("Gamepad in use...");
            SetGamepad();
            
        }
        else if(!inputName.ToLower().Contains("gamepad") && !inputName.ToLower().Contains("dualshock"))
        {
            //set ui to PC
            SetPC();
            
        }
        TriggerUpdate();
        
    }

    private void SetPC()
    {
        gamepad = false;
        xbox = false;
        ps = false;
        pc = true;
    }

    private void SetGamepad()
    {
        gamepad = true;
        pc = false;
    }

    private void TriggerUpdate()// Tells each prompt in the game to check and update their UI images
    {
        foreach(PromptChanger promptChanger in prompts)
        {
            
            promptChanger.UpdateUI();
        }
    }
}
