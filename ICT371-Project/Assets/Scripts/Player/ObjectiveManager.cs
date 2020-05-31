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
    public GameObject optionalObjectiveList;

    public ObjectiveListType objectiveListType;
    public enum ObjectiveListType {Tutorial, Main, End, Side};

    public List<Objective> TutorialObjectives;
    public Animation bedroomDoor;
    public Animation garrageInsideDoor;
    public Animation garrageOutsideDoor;
    public Dialogue allMainObjectivesCompleteDialogue, allCoreAndOptionalObjectivesComplete, allSideObjectivesCompleteDialogue;
    public List<Objective> MainObjectives;
    public List<Objective> EndObjectives;
    public List<Objective> SideObjetives;
    public List<Objective> allObjectives;

    public List<GameObject> objectiveUI;

    private void Start()
    {
        Invoke("DelayedStart", 1f);
        TutorialObjectives.Clear();
        MainObjectives.Clear();
        EndObjectives.Clear();
        SideObjetives.Clear();
    }

    private void DelayedStart()
    {
        foreach (Objective obj in allObjectives)
        {
            if(obj != null)
			{
                //Debug.LogError(obj.objective);
                obj.hasComplete = false;
                obj.puzzleCompletionPercentage = 0;//Setting to 50% complete for testing
                switch(obj.objectiveType)
                {
                    case Objective.ObjectiveType.Tutorial:
                        TutorialObjectives.Add(obj);
                        break;
                    case Objective.ObjectiveType.Main:
                        MainObjectives.Add(obj);
                        break;
                    case Objective.ObjectiveType.End:
                        EndObjectives.Add(obj);
                        break;
                    default:
                        break;
                }
            }
        }
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
                SetObjectiveList(SideObjetives);
                CheckCompletedDialogue();
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
                SetObjectiveList(SideObjetives);
                CheckCompletedDialogue();
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

    public void AddNewSideObjective(Objective obj)
    {
        if(objectiveListType == ObjectiveListType.Main || objectiveListType == ObjectiveListType.End)
        {   
            GameObject uiElement = CreateNewObjectiveUIElement(obj.objectiveRequirement);
            obj.uiElement = uiElement;
            Objective objective = obj;
            uiElement.GetComponent<ObjectiveUIElement>().UpdateObjective(objective.objective);
            SideObjetives.Add(objective);
            TrackingController.instance.totalObjectives++;
        }
    }
    


    public void SetObjectiveList(List<Objective> objList)
    {
        foreach (Objective obj in objList)
        {
            GameObject objective = CreateNewObjectiveUIElement(obj.objectiveRequirement);// Object.Instantiate(objectiveUIElement as GameObject);
            obj.uiElement = objective;
            ObjectiveUIElement uIElement = objective.GetComponent<ObjectiveUIElement>();
            uIElement.UpdateObjective(obj.objective);
            if(obj.hasComplete)
            {
                uIElement.UpdateObjective(obj.hasComplete);
            }
            
            //objectiveUI.Add(objective);
        }
    }
    public GameObject CreateNewObjectiveUIElement(Objective.ObjectiveRequirement requirement)
    {
        GameObject uiElement = Object.Instantiate(objectiveUIElement as GameObject);
        if(requirement == Objective.ObjectiveRequirement.Required)
        {
            uiElement.transform.SetParent(objectiveList.transform);

        }
        else
        {
            uiElement.transform.SetParent(optionalObjectiveList.transform);
        }
        uiElement.transform.localScale = Vector3.one;
        uiElement.transform.position = Vector3.zero;
        uiElement.transform.localRotation = new Quaternion(0, 0, 0, 0); //resetiing rotation of object incase it didnt spawn with the correct values
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

    private int mainObj;
    private int sideObj;
    private bool hasSeenCoreAndOptional = false;
    private bool hasSeenOption = false;
    private bool hasSeenCore = false;
    public void CheckCompletedDialogue()
    {
        mainObj = 0;
        sideObj = 0;
        foreach(Objective obj in allObjectives)
        {
            if(obj.hasComplete)
            {
                switch (obj.objectiveType)
                {
                    case Objective.ObjectiveType.Main:
                        mainObj++;
                        break;
                    case Objective.ObjectiveType.Side:
                        sideObj++;
                        break;
                    default:
                        break;
                }
            }
        }

        if(sideObj == 3 && mainObj == 5)
        {
            if(!hasSeenCoreAndOptional)
            {
                DialogueManager.instance.StartDialogue(allCoreAndOptionalObjectivesComplete);
                hasSeenCoreAndOptional = true;
            }
        }
        else if(mainObj == 5 && sideObj != 3)
        {
            if(!hasSeenCore)
            {
                DialogueManager.instance.StartDialogue(allMainObjectivesCompleteDialogue);
                hasSeenCore = true;
            }
        }
        else if(sideObj == 3 && mainObj != 5)
        {
            if(!hasSeenOption)
            {
                DialogueManager.instance.StartDialogue(allSideObjectivesCompleteDialogue);
                hasSeenOption = true;
            }
        }
    }


}