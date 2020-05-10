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

    public ObjectiveListType objectiveListType;
    public enum ObjectiveListType {Tutorial, Main, End};

    public List<Objective> TutorialObjectives;
    public List<Objective> MainObjectives;
    public List<Objective> EndObjectives;

    public List<GameObject> objectiveUI;

    public AudioClip taskCompleteAudio;
    public AudioSource audioSource;
    

    private void Start()
    {
        SetObjectiveList(TutorialObjectives);
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

    public void SetObjectiveList(List<Objective> objList)
    {
        foreach (Objective obj in objList)
        {
            GameObject objective = Object.Instantiate(objectiveUIElement as GameObject);
            objective.transform.SetParent(objectiveList.transform);
            obj.uiElement = objective;
            objective.transform.localScale = Vector3.one;//resetting the scale to be correct
            objective.GetComponent<ObjectiveUIElement>().UpdateObjective(obj.objective);
            objectiveUI.Add(objective);
        }
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

    public Objective()
    {
        objective = "Not Defined";
        hasComplete = false;
        objectiveID = 0;
        uiElement = null;
        objectiveType = ObjectiveType.Tutorial;
    }
}
