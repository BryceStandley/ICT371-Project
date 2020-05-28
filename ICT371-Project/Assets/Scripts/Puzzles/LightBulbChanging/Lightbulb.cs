using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightbulb : MonoBehaviour
{
    public enum LightbulbType {Halogen, EnergySaver};
    public LightbulbType lightbulbType = LightbulbType.Halogen;
    public LightHousing lightHousing;
    public bool isGarbage = false;
    public bool tagChanged = false;

    private void Update()
    {
        if(PuzzleManager.instance.currentObjectiveListType == ObjectiveManager.ObjectiveListType.Tutorial || tagChanged)
        {
            gameObject.tag = "Information";
        }
        else{
            gameObject.tag = "PickUp";
        }
        if(lightbulbType == LightbulbType.Halogen)
        {
            if(GetComponent<PickUp>().pickedUp)
            {
                if(!isGarbage)
                {
                    if(AddGarbage())
                    {
                        lightHousing.currentBulb = null;
                        transform.localScale = new Vector3(62f, 62f, 62f); //resetting scale of the bulb
                        PuzzleManager.instance.CreateLightBulbGarbageObjective();
                    }
                }
            }
        }
    }

    private bool AddGarbage()
    {
        gameObject.AddComponent(typeof(GarbageItem));
        isGarbage = true;
        GarbageItem gi = GetComponent<GarbageItem>();
        gi.garbageType = GarbageBin.GarbageType.General;
        gi.itemType = GarbageItem.ItemType.Bulb;
        gi.item = GarbageItem.Item.Bulb;
        return true;
    }
}
