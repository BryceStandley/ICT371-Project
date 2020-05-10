using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunRotation : MonoBehaviour
{
    public float rotationTime = 0.01f;
    private void LateUpdate() 
    {
        transform.RotateAround(transform.position, Vector3.left, rotationTime);
    }
}
