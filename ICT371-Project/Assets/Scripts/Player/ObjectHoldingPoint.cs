using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHoldingPoint : MonoBehaviour
{
    public float distance = 5f;

    private ObjectPickUp objPickUp;

    private void Start()
    {
        objPickUp = FindObjectOfType<ObjectPickUp>();
        transform.position = Camera.main.transform.position + Camera.main.transform.forward * distance;
    }

    private void LateUpdate()
    {
        transform.position = Camera.main.transform.position + Camera.main.transform.forward * distance;

        CheckGround();
    }

    private void CheckGround()
    {
        RaycastHit hit = new RaycastHit();
        Debug.DrawLine(Camera.main.transform.position, transform.position, Color.red, 5f);
        if(Physics.Linecast(Camera.main.transform.position, transform.position, out hit))
        {
            Vector3 moveTo = new Vector3(hit.point.x + hit.normal.x * 0.5f, hit.point.y + hit.normal.y * 0.5f, hit.point.z + hit.normal.z * 0.5f);
            transform.position = moveTo;
        }
    }
}
