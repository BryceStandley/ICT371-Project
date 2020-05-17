using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager instance;
    public ObjectiveManager.ObjectiveListType currentObjectiveListType = ObjectiveManager.ObjectiveListType.Tutorial;


    #region Dialogue Variables
    public Dialogue oneTreePlantedDialogue;
    public Dialogue allTreesPlantedDialogue;
    public Dialogue allRubbishCompletedDialogue;
    public Dialogue twoMistakesRubbishDialogue;
    #endregion
    private void Awake()
    {
        instance = this;
        holesInWorld = new List<GameObject>();
        laundryItems = new List<GameObject>();
        garbageItems = new List<GameObject>();
        garbageBins = new List<GarbageBin>();

    }

    private void UpdateObjectiveListType()
    {
        currentObjectiveListType = ObjectiveManager.instance.objectiveListType;
    }

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


    //Garbage Collection Puzzle
    //Usign Objective ID of 96
    private List<GameObject> garbageItems;
    private List<GarbageBin> garbageBins;

    public void AddGarbageItem(GameObject item)
    {
        garbageItems.Add(item);
    }

    public void AddGarbageBin(GarbageBin bin)
    {
        garbageBins.Add(bin);
    }
    public List<GameObject> GetGarbageList()
    {
        return garbageItems;
    }

    public void CheckGarbageCollectionComplete()
    {
        int total = 0;
        int mistake = 0;
        foreach(GarbageBin bin in garbageBins)
        {
            total += bin.numberOfCorrectItemsInBin + bin.numberOfIncorrectItemsInBin;
            mistake += bin.numberOfIncorrectItemsInBin;
            if(bin.numberOfIncorrectItemsInBin > 0)
            {
                TrackingController.instance.totalMistakes++;
            }
        }

        if(mistake == 2)
        {
            DialogueManager.instance.StartDialogue(twoMistakesRubbishDialogue);
        }

        if(total == garbageItems.Count)
        {
            if(currentObjectiveListType == ObjectiveManager.ObjectiveListType.Main)
            {
                foreach(Objective obj in ObjectiveManager.instance.MainObjectives)
                {
                    if(obj.objectiveID == 96)
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
        }
    }

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
        foreach(GameObject hole in holesInWorld)
        {
            SeedHole h = hole.GetComponent<SeedHole>();
            if(h.hasBeenPlanted)
            {
                count++;
            }

        }
        if(count == 1)
        {
            DialogueManager.instance.StartDialogue(oneTreePlantedDialogue);
        }
        if(count == holesInWorld.Count)
        {
            //get objective manager and update ui
            if(currentObjectiveListType == ObjectiveManager.ObjectiveListType.Main)
            {
                foreach (Objective obj in ObjectiveManager.instance.MainObjectives)
                {
                    if (obj.objectiveID == 99)
                    {
                        if (obj.objective.ToLower().Contains("tree"))//Extra check to enure we dont have duplicate ID's
                        {
                            obj.hasComplete = true;
                            DialogueManager.instance.StartDialogue(allTreesPlantedDialogue);
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
        }
    }
}
