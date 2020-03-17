using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public InputMaster controls;

    public CharacterController controller; //Reference to main player character controller

    public float speed = 6f;

    private float x = 0f;
    private float z = 0f;

    private void Awake()
    {
        controls = new InputMaster();//Creating a new inputMaster component
        controls.Player.MovementX.performed += ctx => MoveX(ctx.ReadValue<float>());// Assigning the MovementX action to the MoveX function
        controls.Player.MovementY.performed += ctx => MoveY(ctx.ReadValue<float>());// Assigning the MovementY action to the MoveY function
        
    }

    private void OnEnable()//Enables movement controls if player is enabled
    {
        controls.Player.Enable();
    }

    private void OnDisable()//Disables movement controls if player is disabled
    {
        controls.Player.Disable();
    }

    void MoveX(float input)//This Function triggers on button down and up
    {
        if(input != x)//Checking if the x/y value has changed i.e. player released button and now we need to stop moving
        {
            x = input;
        }
    }

    void MoveY(float input)//This Function triggers on button down and up
    { 
        if (input != z)//Checking if the x/y value has changed i.e. player released button and now we need to stop moving
        {
            z = input;
        }
    }

    void Update()
    {
        Vector3 move = transform.right * x + transform.forward * z; // Making move vector in the correct local directions of movement
        controller.Move(move * speed * Time.deltaTime);//Applying movement vector with a speed and time delta

    }
}
