using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    public InputMaster controls;
    public InputAction inputAction;

    private Vector2 input;
    private bool onInput = false;

    private Quaternion headOriginOrientation, bodyOriginOrientation;// References to the original Rotation Origins
    private float currentYaw = 0f, currentPitch = 0f; //Base X and Y values

    public float sensitivity; // Sensitivity used for calc

    public Transform playerBody; //Reference to main player transform


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
        
    }

    private void OnDisable()//Disables camera controls if camera is disabled
    {
        controls.Player.Disable();
        
    }

    private void Update() // Generating rotation values based on input
    {
        //This input alows for smoth movement with mouse and controller
        //A mouse is always updating its valuse but a controller doesnt so we need to make sure we keep reading the value
        //of the input when its triggered.
        if(inputAction.triggered || onInput)//Checking if the input action was triggered this frame or were still poling the input
        {
            onInput = true;
            //Debug.Log("Action Triggered");
            input = inputAction.ReadValue<Vector2>();
        }
        else
        {
            onInput = false;
        }

        input.x *= Time.deltaTime * sensitivity;
        input.y *= Time.deltaTime * sensitivity;

        currentYaw += input.x;
        currentPitch += input.y;
        currentPitch = Mathf.Clamp(currentPitch, -85f, 75f);
    }

    private void FixedUpdate()//Applying Rotation maths to the camera and body of the plyer
    {
        var bodyRotation = Quaternion.AngleAxis(currentYaw, Vector3.up);
        var headRotation = Quaternion.AngleAxis(-currentPitch, Vector3.right);

        transform.localRotation = headRotation * headOriginOrientation;
        playerBody.localRotation = bodyRotation * bodyOriginOrientation;
        
    }

}
