using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    public static PlayerLook instance;
    public InputMaster controls;
    public InputAction inputAction;
    public PlayerInput playerInput;

    private Vector2 input;
    private Vector2 lastInput;
    private bool onInput = false;
    private bool gamepad = false;

    private Quaternion headOriginOrientation, bodyOriginOrientation;// References to the original Rotation Origins
    private float currentYaw = 0f, currentPitch = 0f; //Base X and Y values

    public float sensitivity; // Sensitivity used for calc

    public Transform playerBody; //Reference to main player transform
    private bool allowedInput = false;
    private bool isFirstInput = true;
    private bool zeroed = false;


    private void Awake()
    {
        instance = this;
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
        InputSystem.onEvent += InputSystem_onEvent;
        input = Vector2.zero;
        Invoke("AllowInput", 0.01f); // Adding a small delay to allow the mouse to recenter before getting input data
    }

    private void InputSystem_onEvent(UnityEngine.InputSystem.LowLevel.InputEventPtr arg1, InputDevice arg2)
    {
        //throw new System.NotImplementedException();
        if(playerInput.currentControlScheme.ToLower().Contains("gamepad"))
        {
            //Debug.LogError("Gamepad in use...");
            //Debug.LogError(playerInput.currentControlScheme);
            gamepad = true;
        }
        else
        {
            //Debug.LogError("No gamepad...");
            //Debug.LogError(playerInput.currentControlScheme);
            gamepad = false;
        }
        
    }

    private void OnDisable()//Disables camera controls if camera is disabled
    {
        allowedInput = false;
        zeroed = false;
        isFirstInput = true;
        InputSystem.onEvent -= InputSystem_onEvent;
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

            if(isFirstInput)
            {
                if(!gamepad)
                {
                    //Debug.Log("Main input: " +input);
                    if (input.x > 2.5f || input.x < -2.5f)
                    {
                        if (currentPitch == 0f && currentYaw == 0)
                        {
                            currentYaw = 0f;
                            currentPitch = 0f;
                            input = Vector2.zero;
                        }
                        else
                        {
                            input = Vector2.zero;
                        }
                        isFirstInput = true;
                        zeroed = true;
                        //Debug.Log("Zeroed Input: " +input);
                    }
                    else if (zeroed)
                    {
                        isFirstInput = false;

                    }
                }
                else
                {
                    isFirstInput = false;
                }

            }
            else
            {
                lastInput = input;
                input.x *= Time.deltaTime * sensitivity;
                input.y *= Time.deltaTime * sensitivity;

                currentYaw += input.x;
                currentPitch += input.y;
                currentPitch = Mathf.Clamp(currentPitch, -85f, 75f);
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

    public void UpdateSensitivity(float slider)
    {
        sensitivity = slider;
        PlayerPrefs.SetFloat("GameCameraSensitivity", slider);
    }

}
