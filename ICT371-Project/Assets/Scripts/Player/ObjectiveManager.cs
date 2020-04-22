using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectiveManager : MonoBehaviour
{

    //using a static instance as there will only be one instance of the Objective manager in the scene at a time
    public static ObjectiveManager instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject objectiveUIElement;
    public GameObject objectiveList;

    public List<Objective> objectives;
    

    private void Start()
    {
        foreach(Objective obj in objectives)
        {
            GameObject objective = Object.Instantiate(objectiveUIElement as GameObject);
            objective.transform.SetParent(objectiveList.transform);
            obj.uiElement = objective;
            objective.transform.localScale = Vector3.one;//resetting the scale to be correct
            objective.GetComponent<ObjectiveUIElement>().UpdateObjective(obj.objective);
        }
    }

}

[System.Serializable]
public class Objective
{
    [SerializeField]
    public string objective;
    public int objectiveID;
    public bool hasComplete = false;
    public GameObject uiElement;

    public Objective()
    {
        objective = "Not Defined";
        hasComplete = false;
        objectiveID = 0;
        uiElement = null;
    }
}
