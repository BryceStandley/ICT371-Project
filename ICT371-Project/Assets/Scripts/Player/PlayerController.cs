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
        controls.Player.Movement.performed += ctx => Move(ctx.ReadValue<Vector2>());// Assigning the camera action to the Look function

    }

    private void OnEnable()//Enables movement controls if player is enabled
    {
        controls.Player.Enable();
    }

    private void OnDisable()//Disables movement controls if player is disabled
    {
        controls.Player.Disable();
    }

    void Move(Vector2 input)//This Function triggers on button down and up
    {
        if(input.x != x)//Checking if the x/y value has changed i.e. player released button and now we need to stop moving
        {
            x = input.x;
        }
        if(input.y != z)
        {
            z = input.y;
        }
        
    }

    void Update()
    {
        Vector3 move = transform.right * x + transform.forward * z; // Making move vector in the correct local directions of movement
        controller.Move(move * speed * Time.deltaTime);//Applying movement vector with a speed and time delta

    }
}
