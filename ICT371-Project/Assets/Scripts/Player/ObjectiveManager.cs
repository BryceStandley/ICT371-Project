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
        audioSource = GetComponent<AudioSource>();
    }

    public GameObject objectiveUIElement;
    public GameObject objectiveList;
    public GameObject optionalObjectiveList;

    public ObjectiveListType objectiveListType;
    public enum ObjectiveListType {Tutorial, Main, End};

    public List<Objective> TutorialObjectives;
    public Animation bedroomDoor;
    public Animation garrageInsideDoor;
    public Animation garrageOutsideDoor;
    public List<Objective> MainObjectives;
    public List<Objective> EndObjectives;

    public List<GameObject> objectiveUI;

    public AudioClip taskCompleteAudio;
    public AudioSource audioSource;
    

    private void Start()
    {
        SetObjectiveList(TutorialObjectives);
        TrackingController.instance.totalObjectives = TutorialObjectives.Count + MainObjectives.Count + EndObjectives.Count;
    }

    private int check = 0;
    public void CheckCompletedList()
    {
        check = 0;
        if(objectiveListType == ObjectiveListType.Tutorial)
        {
            foreach(Objective obj in TutorialObjectives)
            {
                if(obj.hasComplete)
                {
                    check++;
                }
            }
            if(check == TutorialObjectives.Count)
            {
                //Tutorials complete, change list
                ClearUI();
                SetObjectiveList(MainObjectives);
                objectiveListType = ObjectiveListType.Main;
                PuzzleManager.instance.currentObjectiveListType = ObjectiveListType.Main; 
                //player has completed tutorial, open garrage inside door
                garrageInsideDoor.Play();

            }
        }
        else if (objectiveListType == ObjectiveListType.Main)
        {
            foreach (Objective obj in MainObjectives)
            {
                if (obj.hasComplete)
                {
                    check++;
                }
            }
            if (check == MainObjectives.Count)
            {
                //Mains complete, change list
                ClearUI();
                SetObjectiveList(EndObjectives);
                objectiveListType = ObjectiveListType.End;
                PuzzleManager.instance.currentObjectiveListType = ObjectiveListType.End;
                garrageOutsideDoor.Play();
            }
        }
        else if (objectiveListType == ObjectiveListType.End)
        {
            foreach (Objective obj in EndObjectives)
            {
                if (obj.hasComplete)
                {
                    check++;
                }
            }
            if (check == EndObjectives.Count)
            {
                //End complete, end game
                ClearUI();
            }
        }
    }

    public void AddNewMainObjective(string objective, int objID, Objective.ObjectiveType objType, Objective.ObjectiveRequirement objRequirement, int percentage, int weight)
    {
        if(objectiveListType == ObjectiveListType.Main)
        {   
            GameObject uiElement = CreateNewObjectiveUIElement(objRequirement);
            Objective obj = new Objective(objective, objID, uiElement, objType, objRequirement, percentage, weight);
            uiElement.GetComponent<ObjectiveUIElement>().UpdateObjective(obj.objective);
            MainObjectives.Add(obj);
        }
    }

    public void SetObjectiveList(List<Objective> objList)
    {
        foreach (Objective obj in objList)
        {
            GameObject objective = Object.Instantiate(objectiveUIElement as GameObject);
            if(obj.objectiveRequirement == Objective.ObjectiveRequirement.Required)
            {
                objective.transform.SetParent(objectiveList.transform);
            }
            else
            {
                objective.transform.SetParent(optionalObjectiveList.transform);
            }
            obj.uiElement = objective;
            objective.transform.localScale = Vector3.one;//resetting the scale to be correct
            objective.GetComponent<ObjectiveUIElement>().UpdateObjective(obj.objective);
            objectiveUI.Add(objective);
        }
    }
    public GameObject CreateNewObjectiveUIElement(Objective.ObjectiveRequirement requirement)
    {
        GameObject uiElement = Object.Instantiate(objectiveUIElement as GameObject);
        uiElement.transform.localScale = Vector3.one;
        uiElement.transform.position = Vector3.zero;
        uiElement.transform.localRotation = new Quaternion(0,0,0,0); //resetiing rotation of object incase it didnt spawn with the correct values
        uiElement.transform.SetParent(objectiveList.transform);
        if(requirement == Objective.ObjectiveRequirement.Required)
        {
            uiElement.transform.SetParent(objectiveList.transform);
        }
        else
        {
            uiElement.transform.SetParent(optionalObjectiveList.transform);
        }
        objectiveUI.Add(uiElement);
        return uiElement;
    }

    private void ClearUI()
    {
        foreach(GameObject ui in objectiveUI)
        {
            Destroy(ui.gameObject);
        }
        objectiveUI.Clear();
    }

    public void PlayCompleteObjectiveSound()
    {
        audioSource.clip = taskCompleteAudio;
        audioSource.Play();
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

    public ObjectiveType objectiveType;
    public enum ObjectiveType {Tutorial, Main, End};

    public ObjectiveRequirement objectiveRequirement;
    public enum ObjectiveRequirement {Required, Optional};
    [Range(0, 100)]
    public int puzzleCompletionPercentage = 0;
    [Range(0,100)]
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
        public Objective(string obj, int id, GameObject element, ObjectiveType type, ObjectiveRequirement requirement, int percentage, int weight)
    {
        objective = obj;
        hasComplete = false;
        objectiveID = id;
        uiElement = element;
        objectiveType = type;
        objectiveRequirement = requirement;
        puzzleCompletionPercentage = percentage;
        objectiveWeight = weight;
    }
}
