using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHoldingPoint : MonoBehaviour
{
    public float distance = 5f;
    public GameObject playerCamera;
    private void Start()
    {
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
        //Debug.DrawLine(Camera.main.transform.position, transform.position, Color.red, 5f);
        if(Physics.Linecast(Camera.main.transform.position, transform.position, out hit))
        {
            if(!hit.transform.gameObject.CompareTag("DialogueTrigger"))
            {
                Vector3 moveTo = new Vector3(hit.point.x + hit.normal.x * 0.5f, hit.point.y + hit.normal.y * 0.5f, hit.point.z + hit.normal.z * 0.5f);
                transform.position = moveTo;
            }
            if(hit.transform.gameObject.GetComponent<LightHousing>())
            {
                if(ObjectPickUp.instance.heldItem != null && ObjectPickUp.instance.heldItem.GetComponent<Lightbulb>())
                {
                    if(ObjectPickUp.instance.heldItem.GetComponent<Lightbulb>().lightbulbType == Lightbulb.LightbulbType.EnergySaver)
                    {
                        hit.transform.gameObject.GetComponent<LightHousing>().AcceptBulb(ObjectPickUp.instance.heldItem);
                    }
                }
            }
            else if(hit.transform.gameObject.GetComponent<WashingMachine>())
            {
                if(ObjectPickUp.instance.heldItem != null && ObjectPickUp.instance.heldItem.GetComponent<LaundryBasket>())
                {
                    if(ObjectPickUp.instance.heldItem.GetComponent<LaundryBasket>().isFull)
                    {
                        hit.transform.gameObject.GetComponent<WashingMachine>().AcceptLaundryBasket(ObjectPickUp.instance.heldItem);
                    }
                }
                else if(ObjectPickUp.instance.heldItem != null && ObjectPickUp.instance.heldItem.GetComponent<WashedBasket>())
                {
                    if(!ObjectPickUp.instance.heldItem.GetComponent<WashedBasket>().hasClothes)
                    {
                        hit.transform.gameObject.GetComponent<WashingMachine>().TakeWashing(ObjectPickUp.instance.heldItem);
                    }
                }
            }
            else if(hit.transform.gameObject.GetComponent<Dryer>())
            {
                if(ObjectPickUp.instance.heldItem != null && ObjectPickUp.instance.heldItem.GetComponent<WashedBasket>())
                {
                    if(ObjectPickUp.instance.heldItem.GetComponent<WashedBasket>().hasClothes)
                    {
                        hit.transform.gameObject.GetComponent<Dryer>().AcceptWashedBasket(ObjectPickUp.instance.heldItem);
                    }
                }
            }
            else if(hit.transform.gameObject.GetComponent<Clothesline>())
            {
                if(ObjectPickUp.instance.heldItem != null && ObjectPickUp.instance.heldItem.GetComponent<WashedBasket>())
                {
                    if(ObjectPickUp.instance.heldItem.GetComponent<WashedBasket>().hasClothes)
                    {
                        hit.transform.gameObject.GetComponent<Clothesline>().AcceptBasket(ObjectPickUp.instance.heldItem);
                    }
                }
            }
        
        }
    }
}
