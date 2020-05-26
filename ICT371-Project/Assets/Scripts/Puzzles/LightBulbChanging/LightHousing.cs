using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightHousing : MonoBehaviour
{
    public GameObject bulbLocation;
    public GameObject currentBulb;

    public bool changedBulb = false;

    private void Start()
    {
        currentBulb.GetComponent<Lightbulb>().lightHousing = this;
        PuzzleManager.instance.AddLighHousing(this.gameObject);
    }

    public void AcceptBulb(GameObject other)
    {
        if(currentBulb == null)
        {
            Lightbulb lb = other.GetComponent<Lightbulb>();
            if(lb != null)
            {
                if(lb.lightbulbType == Lightbulb.LightbulbType.EnergySaver)
                {
                    ObjectPickUp.instance.DropItem(other.gameObject);
                    //ObjectPickUp.instance.holding = false;
                    Destroy(other.GetComponent<PickUp>());
                    Destroy(other.GetComponent<Rigidbody>());
                    other.transform.position = bulbLocation.transform.position;
                    other.transform.rotation = new Quaternion(0,0,0,0);
                    other.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    other.tag = "Information";
                    lb.tagChanged = true;
                    currentBulb = other;
                    changedBulb = true;
                    lb.lightHousing = this;
                    PuzzleManager.instance.CheckAllLightsChanged();
                }
            }
        }
    }
}
