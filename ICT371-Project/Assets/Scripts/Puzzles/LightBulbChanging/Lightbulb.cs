using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightbulb : MonoBehaviour
{
    public enum LightbulbType {Halogen, EnergySaver};
    public LightbulbType lightbulbType = LightbulbType.Halogen;
    public LightHousing lightHousing;
    public bool isGarbage = false;
    private void Update()
    {
        if(lightbulbType == LightbulbType.Halogen)
        {
            if(GetComponent<PickUp>().pickedUp)
            {
                if(!isGarbage)
                {
                    AddGarbage();
                }
                if(GetComponent<GarbageItem>())
                {
                    GarbageItem gi = GetComponent<GarbageItem>();
                    gi.garbageType = GarbageBin.GarbageType.General;
                    gi.itemType = GarbageItem.ItemType.Bulb;
                    lightHousing.currentBulb = null;
                    transform.localScale = new Vector3(0.4f, 0.4f ,0.4f); //resetting scale of the bulb
                    
                }
            }
        }
    }

    private void AddGarbage()
    {
        gameObject.AddComponent(typeof(GarbageItem));
        isGarbage = true;
    }
}
