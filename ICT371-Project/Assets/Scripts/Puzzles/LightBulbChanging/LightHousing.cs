using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightHousing : MonoBehaviour
{
    public GameObject bulbLocation;
    public GameObject currentBulb;

    private void Start()
    {
        currentBulb.GetComponent<Lightbulb>().lightHousing = this;
    }

    public void AcceptBulb(GameObject other)
    {
        if(other.GetComponent<Lightbulb>())
        {
            if(other.GetComponent<Lightbulb>().lightbulbType == Lightbulb.LightbulbType.EnergySaver)
            {
                ObjectPickUp.instance.DropItem(other.gameObject);
                //ObjectPickUp.instance.holding = false;
                other.tag = "Information";
                Destroy(other.GetComponent<PickUp>());
                Destroy(other.GetComponent<Rigidbody>());
                other.transform.position = bulbLocation.transform.position;
                other.transform.rotation = new Quaternion(0,0,0,0);
                other.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                currentBulb = other;
            }
        }
    }
}
