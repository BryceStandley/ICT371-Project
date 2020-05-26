using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    public static PlayerInputController instance;
    public PlayerController playerController;
    public PlayerLook playerLook;
    public ObjectPickUp objPickUp;

    private bool gamepad = false;

    private void Awake()
    {
        instance = this;
    }

    public void OnInputChange(PlayerInput input)
    {
        if(input.currentControlScheme == "Gamepad")
        {
            gamepad = true;
        }
        else
        {
            gamepad = false;
        }
        FindObjectOfType<InputUISwitcher>().OnInputChange(input);
        //Debug.LogError("Test for controller change");
    }

    public void DisablePlayerControls()//Disables player input to look and move, also shows the cursor
    {

        playerController.enabled = false;
        playerLook.enabled = false;
        objPickUp.disabledInput = true;
        objPickUp.enabled = false; 
        Cursor.lockState = CursorLockMode.None;
        if(!gamepad)
        {
            Cursor.visible = true;
        }
    }

    public void EnablePlayerControls()//Inverts Disable controls method
    {
        Cursor.lockState = CursorLockMode.Locked;//Locking cursor to the center of the screen and hiding it
        Cursor.visible = false; 
        playerController.enabled = true;
        playerLook.enabled = true;
        objPickUp.disabledInput = false;
        objPickUp.enabled = true;

    }
}
