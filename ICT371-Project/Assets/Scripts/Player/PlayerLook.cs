using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    public InputMaster controls;

    private float x = 0f;
    private float y = 0f;

    public float sensitivity = 100f; //Mouse input sensitivity

    public Transform playerBody; //Reference to main player transform

    private float xRot = 0f;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;//Locking cursor to the center of the screen and hiding it
        controls = new InputMaster();//Creating a new inputMaster component
        controls.Player.Camera.performed += ctx => Look(ctx.ReadValue<Vector2>());// Assigning the camera action to the Look function
        
    }

    private void OnEnable()//Enables camera controls if camera is enabled
    {
        controls.Player.Enable();
    }

    private void OnDisable()//Disables camera controls if camera is disabled
    {
        controls.Player.Disable();
    }


    void Look(Vector2 input)// Maps the delta of the mouse between -1 to 1 for both mouse X and Y
    { 
        x = Mathf.Clamp(input.x, -1, 1);
        y = Mathf.Clamp(input.y, -1, 1);
    }

    void Update()
    {
        float mouseX = x * sensitivity * Time.deltaTime; //Converting input from mouse
        x = 0f;
        float mouseY = y * sensitivity * Time.deltaTime;
        y = 0f;

        
        xRot -= mouseY; //Reversing Y Input so input direction is correct
        xRot = Mathf.Clamp(xRot, -90f, 75f); // Clamping input to stop camera clipping



        transform.localRotation = Quaternion.Euler(xRot, 0f , 0f); //Applying Y Camera rotation
        playerBody.Rotate(Vector3.up * mouseX); //Applying X camera rotation
    }

}
