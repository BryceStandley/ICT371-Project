﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    public InputMaster controls;
    public InputAction inputAction;

    private Vector2 input;
    private Vector2 lastInput;
    private bool onInput = false;

    private Quaternion headOriginOrientation, bodyOriginOrientation;// References to the original Rotation Origins
    private float currentYaw = 0f, currentPitch = 0f; //Base X and Y values

    public float sensitivity; // Sensitivity used for calc

    public Transform playerBody; //Reference to main player transform
    private bool allowedInput = false;
    private bool isFirstInput = true;


    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;//Locking cursor to the center of the screen and hiding it
        Cursor.visible = false; 
        controls = new InputMaster();//Creating a new inputMaster component
        headOriginOrientation = transform.localRotation;
        bodyOriginOrientation = playerBody.localRotation;
        inputAction = controls.Player.Camera;
        
    }

    private void OnEnable()//Enables camera controls if camera is enabled
    {
        controls.Player.Enable();
        input = Vector2.zero;
        Invoke("AllowInput", 0.5f); // Adding a small delay to allow the mouse to recenter before getting input data
        
    }

    private void OnDisable()//Disables camera controls if camera is disabled
    {
        allowedInput = false;
        controls.Player.Disable();
        
    }

    private void AllowInput()
    {
        allowedInput = true;
    }

    private void Update() // Generating rotation values based on input
    {
        //This input alows for smoth movement with mouse and controller
        //A mouse is always updating its valuse but a controller doesnt so we need to make sure we keep reading the value
        //of the input when its triggered.
        if(allowedInput)
        {

            if(inputAction.triggered || onInput)//Checking if the input action was triggered this frame or were still poling the input
            {
                onInput = true;
                input = inputAction.ReadValue<Vector2>();
                //Debug.Log("x: " +input.x +" ------- y: " +input.y);
            }
            else
            {
                onInput = false;
            }
            lastInput = input;
            input.x *= Time.deltaTime * sensitivity;
            input.y *= Time.deltaTime * sensitivity;

            currentYaw += input.x;
            currentPitch += input.y;
            currentPitch = Mathf.Clamp(currentPitch, -85f, 75f);
            if(isFirstInput)
            {
                currentYaw = 0f;
                currentPitch = 0f;
                input = Vector2.zero;
                isFirstInput = false;
            }
        }
        
        
    }

    private void FixedUpdate()//Applying Rotation maths to the camera and body of the plyer
    {
        var bodyRotation = Quaternion.AngleAxis(currentYaw, Vector3.up);
        var headRotation = Quaternion.AngleAxis(-currentPitch, Vector3.right);

        transform.localRotation = headRotation * headOriginOrientation;
        playerBody.localRotation = bodyRotation * bodyOriginOrientation;
        
    }

}
