using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    #region Variables
    public static PuzzleManager instance;
    public ObjectiveManager.ObjectiveListType currentObjectiveListType = ObjectiveManager.ObjectiveListType.Tutorial;
    #endregion

    #region Dialogue Variables
    public Dialogue oneTreePlantedDialogue;
    public Dialogue allTreesPlantedDialogue;
    public Dialogue allRubbishCompletedDialogue;
    public Dialogue twoMistakesRubbishDialogue;
    public Dialogue unpluggingFirstBulbDialogue;

    #endregion

    #region Awake Function and Setup
    private void Awake()
    {
        instance = this;
        holesInWorld = new List<GameObject>();
        laundryItems = new List<GameObject>();
        garbageItems = new List<GarbageItem>();
        garbageBins = new List<GarbageBin>();
        lightHousings = new List<GameObject>();
        powerOutlets = new List<GameObject>();

    }

    private void UpdateObjectiveListType()
    {
        currentObjectiveListType = ObjectiveManager.instance.objectiveListType;
    }
    #endregion

    #region Light Changing Puzzle
    //Light changing puzzle
    //using ObjectiveID of 95

    private List<GameObject> lightHousings;

    public void AddLighHousing(GameObject item)
    {
        lightHousings.Add(item);
    }

    public void CheckAllLightsChanged()
    {
        int changed = 0;
        int percentage = 0;
        Objective ob = new Objective();

        foreach(Objective obj in ObjectiveManager.instance.MainObjectives)
        {
            if(obj.objectiveID == 95)
            {
                if(obj.objective.ToLower().Contains("light"))
                {
                    ob = obj;
                    break;
                }
            }
        }

        foreach(GameObject lh in lightHousings)
        {
            if(lh.GetComponent<LightHousing>().changedBulb)
            {
                changed++;
                percentage += 100 / lightHousings.Count;
            }
        }
        if(percentage > 95)
        {
            percentage = 100;
        }
        if(ob != null)
        {
            ob.puzzleCompletionPercentage = percentage;
        }
        if(changed == lightHousings.Count)
        {

            if(ob != null)
            {
                ob.hasComplete = true;
                TrackingController.instance.completedObjectives = TrackingController.instance.completedObjectives + 1;
                ObjectiveManager.instance.PlayCompleteObjectiveSound();
                if (ob.uiElement.GetComponent<ObjectiveUIElement>().UpdateObjective(ob.hasComplete))
                {
                    ObjectiveManager.instance.CheckCompletedList();
                    return; //breaking the loop as we have found the objective in the list
                }
            }
        }
    }

    #endregion

    #region Light Garbage Collection Puzzle
    //Light buld Trash Puzzle
    //Using objective id of 94
    public List<GarbageBin> generalWasteBins = new List<GarbageBin>();
    private bool lightBulbGarbageObjectiveCreated = false;
    

    public void AddGeneralWasteBin(GarbageBin go)
    {
        generalWasteBins.Add(go);
    }

    public void CreateLightBulbGarbageObjective()
    {
        if(!lightBulbGarbageObjectiveCreated)
        {
            ObjectiveManager.instance.AddNewMainObjective("Throw away the bulbs.", 94, Objective.ObjectiveType.Main, Objective.ObjectiveRequirement.Optional, 0, 10);
            lightBulbGarbageObjectiveCreated = true;
            DialogueManager.instance.StartDialogue(unpluggingFirstBulbDialogue);
        }
    }

    public void CheckBulbCollectionComplete()
    {
        int count = 0;
        int percentage = 0;
        Objective ob = new Objective();

        foreach(Objective obj in ObjectiveManager.instance.MainObjectives)
        {
            if(obj.objectiveID == 94)
            {
                if(obj.objective.ToLower().Contains("bulb"))
                {
                    ob = obj;
                    break; //breaking the loop, found the objective
                }
            }
        }

        foreach(GarbageBin go in generalWasteBins)
        {

            count += go.numberOfBulbsInBin;
            percentage += (100 / lightHousings.Count) * go.numberOfBulbsInBin;
            
        }
        
        if(percentage > 95)
        {
            percentage = 100;
        }
        if(ob != null)
        {
            ob.puzzleCompletionPercentage = percentage;
        }
        if(count == lightHousings.Count)
        {
            if(ob != null)
            {
                ob.hasComplete = true;
                TrackingController.instance.completedObjectives = TrackingController.instance.completedObjectives + 1;
                ObjectiveManager.instance.PlayCompleteObjectiveSound();
                if(ob.uiElement.GetComponent<ObjectiveUIElement>().UpdateObjective(ob.hasComplete))
                {
                    ObjectiveManager.instance.CheckCompletedList();
                }
            }
        }

    }
    #endregion

    #region Laundry Collection Puzzle
    //Laundry Puzzle
    //Using ObjectID of 98
    private List<GameObject> laundryItems;
    public void AddLaundryItem(GameObject item)
    {
        laundryItems.Add(item);
    }

    public void SetLaundryCollectionComplete()
    {
        foreach (Objective obj in ObjectiveManager.instance.TutorialObjectives)
        {
            if (obj.objectiveID == 98)
            {
                if (obj.objective.ToLower().Contains("laundry"))//Extra check to enure we dont have duplicate ID's
                {
                    obj.hasComplete = true;
                    ObjectiveManager.instance.bedroomDoor.Play();
                    TrackingController.instance.completedObjectives = TrackingController.instance.completedObjectives + 1;
                    ObjectiveManager.instance.PlayCompleteObjectiveSound();
                    if (obj.uiElement.GetComponent<ObjectiveUIElement>().UpdateObjective(obj.hasComplete))
                    {
                        //Debug.Log("Objective Updated Correctly");
                        ObjectiveManager.instance.CheckCompletedList();
                    }
                }
            }
        }
    }

    #endregion
    
    #region Washing Clothes Puzzle
    //Washing Clothes Puzzle
    //using Objective ID of 97

    public void SetWashingClothesObjectiveComplete()
    {
        foreach (Objective obj in ObjectiveManager.instance.MainObjectives)
        {
            if (obj.objectiveID == 97)
            {
                if (obj.objective.ToLower().Contains("wash"))//Extra check to enure we dont have duplicate ID's
                {
                    obj.hasComplete = true;
                    TrackingController.instance.completedObjectives = TrackingController.instance.completedObjectives + 1;
                    ObjectiveManager.instance.PlayCompleteObjectiveSound();
                    if (obj.uiElement.GetComponent<ObjectiveUIElement>().UpdateObjective(obj.hasComplete))
                    {
                        ObjectiveManager.instance.CheckCompletedList();
                        AddDryObjective();
                        return;
                    }
                }
            }
        }
    }

    private void AddDryObjective()
    {
        ObjectiveManager.instance.AddNewMainObjective("Dry your clothes.", 93, Objective.ObjectiveType.Main, Objective.ObjectiveRequirement.Optional, 0, 20);

    }

    #endregion
   
    #region Drying Clothes Puzzle
    //Dry Clothes Puzzle
    //using objective ID of 93
    public void SetDryClothesObjectiveComplete()
    {
        foreach (Objective obj in ObjectiveManager.instance.MainObjectives)
        {
            if (obj.objectiveID == 93)
            {
                if (obj.objective.ToLower().Contains("dry"))//Extra check to enure we dont have duplicate ID's
                {
                    obj.hasComplete = true;
                    TrackingController.instance.completedObjectives = TrackingController.instance.completedObjectives + 1;
                    ObjectiveManager.instance.PlayCompleteObjectiveSound();
                    if (obj.uiElement.GetComponent<ObjectiveUIElement>().UpdateObjective(obj.hasComplete))
                    {
                        //Debug.Log("Objective Updated Correctly");
                        ObjectiveManager.instance.CheckCompletedList();
                    }
                }
            }
        }
    }
    #endregion

    #region Garbage Collection Puzzle
    //Garbage Collection Puzzle
    //Usign Objective ID of 96
    private List<GarbageItem> garbageItems;
    private List<GarbageBin> garbageBins;
    private bool hasSeenMistakeMessage = false;

    public void AddGarbageItem(GarbageItem item)
    {
        garbageItems.Add(item);
    }

    public void AddGarbageBin(GarbageBin bin)
    {
        garbageBins.Add(bin);
    }
    public List<GarbageItem> GetGarbageList()
    {
        return garbageItems;
    }

    public void CheckGarbageCollectionComplete()
    {
        Objective obj = new Objective();
        if(currentObjectiveListType == ObjectiveManager.ObjectiveListType.Main)
            {
                foreach(Objective ob in ObjectiveManager.instance.MainObjectives)
                {
                    if(ob.objectiveID == 96)
                    {
                        if(ob.objective.Contains("trash"))
                        {
                            obj = ob;
                            break;
                        }
                    }
                }
            }


        List<GarbageItem> toRemove = new List<GarbageItem>();//Checking the garbage items and removing any bulbs that add them selfs
        foreach(GarbageItem gi in garbageItems)
        {
            if(gi.itemType == GarbageItem.ItemType.Bulb)
            {
                toRemove.Add(gi);
            }
        }
        foreach(GarbageItem gi in toRemove)
        {
            garbageItems.Remove(gi);
        }

        int total = 0;
        int mistake = 0;
        float percentage = 0f;
        foreach(GarbageBin bin in garbageBins)
        {
            total += bin.binTotal;
            mistake += bin.numberOfIncorrectItemsInBin;
            percentage += (float) total / garbageItems.Count;
            //removed mistake counter as mistakes are added to the tracker in the bin script
        }
        if(percentage >= 90)
        {
            percentage = 100f;
            obj.puzzleCompletionPercentage = (int)percentage;
        }

        if(mistake == 2 && !hasSeenMistakeMessage)
        {
            DialogueManager.instance.StartDialogue(twoMistakesRubbishDialogue);
            hasSeenMistakeMessage = true;
        }

        if(obj != null)
        {
            if (total == garbageItems.Count)
            {

                obj.hasComplete = true;
                DialogueManager.instance.StartDialogue(allRubbishCompletedDialogue);
                TrackingController.instance.completedObjectives = TrackingController.instance.completedObjectives + 1;
                ObjectiveManager.instance.PlayCompleteObjectiveSound();
                if (obj.uiElement.GetComponent<ObjectiveUIElement>().UpdateObjective(obj.hasComplete))
                {
                    ObjectiveManager.instance.CheckCompletedList();
                }
            
            }
        }
    }
    #endregion

    #region Tree Planting Puzzle
    //Tree Puzzle
    //Using Objective ID of 99 as the tree puzzle Objective ID
    private List<GameObject> holesInWorld;

    public void AddHole(GameObject hole)
    {
        holesInWorld.Add(hole);
    }

    public void CheckTreePuzzleComplete()//This is a temp method to track the tree puzzle, a more dynamic method will be used later
    {
        int count = 0;
        float percentage = 0f;
        Objective ob = new Objective();

        if(currentObjectiveListType == ObjectiveManager.ObjectiveListType.Main)
        {
            foreach (Objective obj in ObjectiveManager.instance.MainObjectives)
            {
                if (obj.objectiveID == 99)
                {
                    if (obj.objective.ToLower().Contains("tree"))//Extra check to enure we dont have duplicate ID's
                    {
                        ob = obj;
                        break;
                    }
                }
            }
        }

        foreach(GameObject hole in holesInWorld)
        {
            SeedHole h = hole.GetComponent<SeedHole>();
            if(h.hasBeenPlanted)
            {
                count++;
                percentage += (float)count / holesInWorld.Count;
            }

        }
        if(count == 1)
        {
            DialogueManager.instance.StartDialogue(oneTreePlantedDialogue);
        }
        if(percentage >= 90)
        {
            percentage = 100f;
        }
        if(count == holesInWorld.Count)
        {
            //get objective manager and update ui
            ob.hasComplete = true;
            ob.puzzleCompletionPercentage = (int)percentage;
            DialogueManager.instance.StartDialogue(allTreesPlantedDialogue);
            TrackingController.instance.completedObjectives = TrackingController.instance.completedObjectives + 1;
            ObjectiveManager.instance.PlayCompleteObjectiveSound();
            if (ob.uiElement.GetComponent<ObjectiveUIElement>().UpdateObjective(ob.hasComplete))
            {
                //Debug.Log("Objective Updated Correctly");
                ObjectiveManager.instance.CheckCompletedList();
                return;
            }
            
        }
    }
    #endregion

    #region  Phantom Power Unplugging
    //Using Objective ID of 92
    public List<GameObject> powerOutlets;
    public Dialogue allPlugsUnplugged, oneDeivceUnplugged;
    private bool ppMade = false;

    public void AddToPowerOutlets(GameObject outlet)
    {
        powerOutlets.Add(outlet);
    }

    public void CreatePhantomPowerObjective()
    {
        if(!ppMade)
        {
            ObjectiveManager.instance.AddNewMainObjective("Unplug devices", 92, Objective.ObjectiveType.Main, Objective.ObjectiveRequirement.Optional, 0, 10);
            ppMade = true;
        }
    }
    public void CheckIfAllOutletsAreUnplugged()
    {
        Objective objective = new Objective();
        int count = 0;
        float percentage = 0;
        foreach(GameObject go in powerOutlets)
        {
            PowerSocket ps = go.GetComponent<PowerSocket>();
            if(ps.isUnplugged)
            {
                count++;
                percentage += 1f;
            }
        }
        foreach(Objective obj in ObjectiveManager.instance.MainObjectives)
        {
            if(obj.objectiveType == Objective.ObjectiveType.Main)
            {
                if(obj.objectiveID == 92)
                {
                    objective = obj;
                    break;
                }
            }
        }
        if(objective != null)
        {
            float per = (float)percentage / powerOutlets.Count;
            per *= 100;
            if(per >= 95)
            {
                per = 100f;
                objective.puzzleCompletionPercentage = (int)per;
            }
            else
            {
                objective.puzzleCompletionPercentage = (int)per;
            }
        }

        if(count == powerOutlets.Count)
        {
            if(objective != null)
            {
                objective.hasComplete = true;
                DialogueManager.instance.StartDialogue(allPlugsUnplugged);
                TrackingController.instance.completedObjectives = TrackingController.instance.completedObjectives + 1;
                ObjectiveManager.instance.PlayCompleteObjectiveSound();
                if (objective.uiElement.GetComponent<ObjectiveUIElement>().UpdateObjective(objective.hasComplete))
                {
                    ObjectiveManager.instance.CheckCompletedList();
                }
            }
        }
        else if(count == 1)
        {
            DialogueManager.instance.StartDialogue(oneDeivceUnplugged);
        }
    }
    #endregion

    #region Food Buying
    //using Objective ID of 91

    public void AddFoodBuyObjective()
    {
        ObjectiveManager.instance.AddNewMainObjective("Buy some food", 91, Objective.ObjectiveType.Main, Objective.ObjectiveRequirement.Optional, 0, 5);
    }

    public void TriggerFoodBuyComplete()
    {
        Objective obj = new Objective();
        foreach(Objective ob in ObjectiveManager.instance.MainObjectives)
        {
            if(ob.objectiveID == 91)
            {
                if(ob.objective.Contains("food"))
                {
                    obj = ob;
                    break;
                }
            }
        }

        if(obj != null)
        {
            obj.hasComplete = true;
            obj.puzzleCompletionPercentage = 100;
            //DialogueManager.instance.StartDialogue(allPlugsUnplugged);
            TrackingController.instance.completedObjectives = TrackingController.instance.completedObjectives + 1;
            ObjectiveManager.instance.PlayCompleteObjectiveSound();
            if (obj.uiElement.GetComponent<ObjectiveUIElement>().UpdateObjective(obj.hasComplete))
            {
                ObjectiveManager.instance.CheckCompletedList();
            }
        }
    }
    #endregion

    #region Meet GOV Bot
    //Using Objective ID of 100
    public void CheckIfSeenGOVBot()
    {
        Objective obj = new Objective();
        foreach(Objective ob in ObjectiveManager.instance.TutorialObjectives)
        {
            if(ob.objectiveID == 100)
            {
                if(ob.objective.Contains("GOV"))
                {
                    obj = ob;
                    break;
                }
            }
        }

        if(obj != null)
        {
            obj.hasComplete = true;
            obj.puzzleCompletionPercentage = 100;
            //DialogueManager.instance.StartDialogue(allPlugsUnplugged);
            TrackingController.instance.completedObjectives = TrackingController.instance.completedObjectives + 1;
            ObjectiveManager.instance.PlayCompleteObjectiveSound();
            if (obj.uiElement.GetComponent<ObjectiveUIElement>().UpdateObjective(obj.hasComplete))
            {
                ObjectiveManager.instance.CheckCompletedList();
            }
        }
    }
    #endregion
}