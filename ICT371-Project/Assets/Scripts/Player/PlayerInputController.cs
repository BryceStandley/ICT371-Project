using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    public PlayerController playerController;
    public PlayerLook playerLook;
    public ObjectPickUp objPickUp;

    public void DisablePlayerControls()//Disables player input to look and move, also shows the cursor
    {
        playerController.enabled = false;
        playerLook.enabled = false;
        objPickUp.disabledInput = true;
        objPickUp.enabled = false; 
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void EnablePlayerControls()//Inverts Disable controls method
    {
        playerController.enabled = true;
        playerLook.enabled = true;
        objPickUp.disabledInput = false;
        objPickUp.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
