using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputMaster controls;

    public CharacterController controller; //Reference to main player character controller

    public float speed = 6f;
    public float jumpHeight = 5f;
    public float gravity = 20f;


    private Vector2 input;
    private bool jumping = false;
    private Animator animator;

    private Vector3 movementDirection = Vector3.zero;
    private CharacterController character;

    private void Awake()
    {
        controls = new InputMaster();//Creating a new inputMaster component  
    }

    private void Start()
    {
        character = GetComponent<CharacterController>();
        animator = GetComponent<Animator>(); //Disabled For now, Will add animations later and trigger states here
    }

    private void OnEnable()//Enables movement controls if player is enabled
    {
        controls.Player.Enable();
    }

    private void OnDisable()//Disables movement controls if player is disabled
    {
        controls.Player.Disable();
    }

    public void OnMoveInput(InputAction.CallbackContext context)//Input for character movement
    {
        input = context.ReadValue<Vector2>(); 
    }
    

    public void OnJumpInput(InputAction.CallbackContext context)//Input for Jumping
    {
        if(character.isGrounded)
        {
            jumping = true;
        }
    }

    private void FixedUpdate()//Applying movement and applying jumping mechanics
    {
        var localDirection = new Vector3(input.x, 0, input.y) * speed;
        var worldSpaceDirection = transform.TransformDirection(localDirection);
        if(controller.isGrounded)
        {
            animator.SetFloat("velocity", input.magnitude);
            if(input.magnitude < 0.2f)
            {
                animator.SetFloat("velocity", 0);
            }
            if(jumping)
            {
                jumping = false;
                worldSpaceDirection.y = jumpHeight;
                if(localDirection.magnitude > 0)
                {
                    animator.SetFloat("velocity", 0);
                }
            }
            else
            {
                worldSpaceDirection.y = 0;
            }
            movementDirection = worldSpaceDirection;
        }
        movementDirection.y -= gravity * Time.deltaTime;

        character.Move(movementDirection * Time.deltaTime);

    }
}
