using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    public InputMaster controls;

    private float x = 0f;
    private float y = 0f;

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
    }

    private void OnEnable()//Enables camera controls if camera is enabled
    {
        controls.Player.Camera.performed += Look;// Assigning the camera action to the Look function
        controls.Player.Enable();
        
    }

    private void OnDisable()//Disables camera controls if camera is disabled
    {
        controls.Player.Camera.performed -= Look;// Assigning the camera action to the Look function
        controls.Player.Disable();
        
    }

    void Look(InputAction.CallbackContext context)// Maps the delta of the mouse between -1 to 1 for both mouse X and Y
    {

        var delta = context.ReadValue<Vector2>();


        x = delta.x;
        y = delta.y;
    }

    private void Update() // Generating rotation values based on input
    {
        x *= Time.deltaTime * sensitivity;
        y *= Time.deltaTime * sensitivity;

        currentYaw += x;
        currentPitch += y;
        currentPitch = Mathf.Clamp(currentPitch, -90f, 75f);
    }

    private void FixedUpdate()//Applying Rotation maths to the camera and body of the plyer
    {
        var bodyRotation = Quaternion.AngleAxis(currentYaw, Vector3.up);
        var headRotation = Quaternion.AngleAxis(-currentPitch, Vector3.right);

        transform.localRotation = headRotation * headOriginOrientation;
        playerBody.localRotation = bodyRotation * bodyOriginOrientation;
        
    }

}
