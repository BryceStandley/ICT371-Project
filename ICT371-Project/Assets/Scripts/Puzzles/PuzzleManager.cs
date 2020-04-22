using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager instance;

    private void Awake()
    {
        instance = this;
        holesInWorld = new List<GameObject>();
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
        if(count == holesInWorld.Count)
        {
            //get objective manager and update ui
            foreach(Objective obj in ObjectiveManager.instance.objectives)
            {
                if(obj.objectiveID == 99)
                {
                    if(obj.objective.ToLower().Contains("tree"))//Extra check to enure we dont have duplicate ID's
                    {
                        obj.hasComplete = true;
                        if(obj.uiElement.GetComponent<ObjectiveUIElement>().UpdateObjective(obj.hasComplete))
                        {
                            Debug.Log("Objective Updated Correctly");
                        }
                    }
                }
            }
        }
    }
}
