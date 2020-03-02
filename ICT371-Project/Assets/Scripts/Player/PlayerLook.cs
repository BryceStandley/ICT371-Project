using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public float sensitivity = 100f; //Mouse input sensitivity

    public Transform playerBody; //Reference to main player transform

    private float xRot = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; //Hiding mouse and locking it to the center of the screen
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime; //Getting input from mouse for X and Y
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        xRot -= mouseY; //Reversing Y Input
        xRot = Mathf.Clamp(xRot, -90f, 75f); // Clamping input to stop camera clipping

        transform.localRotation = Quaternion.Euler(xRot, 0f , 0f); //Applying Y Camera rotation
        playerBody.Rotate(Vector3.up * mouseX); //Applying X camera rotation
    }
}
