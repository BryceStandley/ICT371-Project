using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSocket : MonoBehaviour
{
    public GameObject[] powerCables;
    public GameObject[] unpluggedLocations;
    public bool isUnplugged = false;
    public bool isLamp = false;
    public Light PointLight;
    public bool isDryer;
    public Dryer dryer;
    public bool isWasher;
    public Dialogue unpluggingWasherDialogue;
    public TrackingController.PhantomType[] phantomTypes;
    public int[] itemPercentages;

    private void Start()
    {
        PuzzleManager.instance.AddToPowerOutlets(this);
    }

    private void LateUpdate()
    {
        if(ObjectiveManager.instance.objectiveListType == ObjectiveManager.ObjectiveListType.Main)
        {
            if(ObjectPickUp.instance.lookedAtItem != null)
            {
                if(ObjectPickUp.instance.lookedAtItem == this.gameObject)
                {
                    if(!isUnplugged)
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
        
    }

    public void UnplugItem()
    {
        if(!isWasher)
        {
            for(int i = 0; i < powerCables.Length; i++)
            {
                powerCables[i].transform.position = unpluggedLocations[i].transform.position;
                powerCables[i].transform.rotation = unpluggedLocations[i].transform.rotation;
            }
            isUnplugged = true;
            if (isLamp) 
            {
                PointLight.enabled = false;
            }
            if(isDryer)
            {
                dryer.allowedToUseDryer = false;
            }
            PuzzleManager.instance.CreatePhantomPowerObjective();
            PuzzleManager.instance.CheckIfAllOutletsAreUnplugged();
            foreach(TrackingController.PhantomType pt in phantomTypes)
            {
                TrackingController.instance.AddPhantomPowerSaved(pt);
            }
        }
        else
        {
            DialogueManager.instance.StartDialogue(unpluggingWasherDialogue);
        }
    }
}
