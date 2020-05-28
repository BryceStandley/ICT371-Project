using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SideObjective", menuName = "Side Objective", order = 1)]
[System.Serializable]
public class Objective : ScriptableObject
{
    [SerializeField]
    public string objective;
    public int objectiveID;
    public bool hasComplete = false;
    public GameObject uiElement;

    public ObjectiveType objectiveType;
    public enum ObjectiveType { Tutorial, Main, End, Side };

    public ObjectiveRequirement objectiveRequirement;
    public enum ObjectiveRequirement { Required, Optional };
    [Range(0, 100)]
    public int puzzleCompletionPercentage = 0;
    [Range(0, 100)]
    public int objectiveWeight = 0; //how mush this objective counts towards the final score

    public Objective()
    {
        objective = "Not Defined";
        hasComplete = false;
        objectiveID = 0;
        uiElement = null;
        objectiveType = ObjectiveType.Tutorial;
        objectiveRequirement = ObjectiveRequirement.Optional;
        puzzleCompletionPercentage = 0;
        objectiveWeight = 0;
    }
    /*public Objective(string obj, int id, GameObject element, ObjectiveType type, ObjectiveRequirement requirement, int percentage, int weight)
    {
        objective = obj;
        hasComplete = false;
        objectiveID = id;
        uiElement = element;
        objectiveType = type;
        objectiveRequirement = requirement;
        puzzleCompletionPercentage = percentage;
        objectiveWeight = weight;
    }*/
}
