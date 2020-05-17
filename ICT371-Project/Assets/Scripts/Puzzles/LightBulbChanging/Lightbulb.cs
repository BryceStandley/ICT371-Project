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
                    GetComponent<GarbageItem>().garbageType = GarbageBin.GarbageType.General;
                    PuzzleManager.instance.AddGarbageItem(this.gameObject);
                    lightHousing.currentBulb = null;
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
