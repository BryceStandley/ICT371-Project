using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSocket : MonoBehaviour
{
    public GameObject powerCable;
    public GameObject unpluggedLocation;
    public bool isUnplugged = false;
    public bool isLamp = false;
    public Light PointLight;

    private void Start()
    {
        PuzzleManager.instance.AddToPowerOutlets(gameObject);
    }

    private void LateUpdate()
    {
        if(ObjectiveManager.instance.objectiveListType == ObjectiveManager.ObjectiveListType.Main)
        {
            if(ObjectPickUp.instance.lookedAtItem != null)
            {
                if(ObjectPickUp.instance.lookedAtItem.name.Contains("power"))
                {
                    PromptChanger.instance.thirdPurpPrompt = true;
                    PromptChanger.instance.hasCustomName = true;
                    PromptChanger.instance.customName = "Unplug";
                    PromptChanger.instance.customNameAction = PromptChanger.CustomNameAction.Third;
                    PromptChanger.instance.UpdateUI();
                    ActionManger.instance.SetCurrentAction("Unplug");
                }
            }
        }
    }

    public void UnplugItem()
    {
        powerCable.transform.position = unpluggedLocation.transform.position;
        powerCable.transform.rotation = unpluggedLocation.transform.rotation;
        isUnplugged = true;
        if (isLamp) 
        {
            PointLight.enabled = false;
        }
        PuzzleManager.instance.CreatePhantomPowerObjective();
        PuzzleManager.instance.CheckIfAllOutletsAreUnplugged();
    }
}
