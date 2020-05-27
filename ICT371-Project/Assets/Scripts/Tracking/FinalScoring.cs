using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

//To Be removed
using UnityEngine.InputSystem;

public class FinalScoring : MonoBehaviour
{
    public static FinalScoring instance;
    public int finalScorePercentage = 0;
    public float finalScore;
    public enum FinalMark {ClimateConscious, Greenthumb, CarbonCutter, EcoChampion, TheModelCitizen};
    [Range(0, 5)]
    public int totalStars = 0;
    public FinalMark finalMark;

    public Objective[] allSideObjectives;

    #region Final UI Variables
    public GameObject FinalScoreUI;
    public TextMeshProUGUI finalGradeText;
    public Image[] starImages;
    public Sprite fullStarSprite;
    public GameObject statsUI;
    public GameObject nextPageButton;
    public TextMeshProUGUI totalCO2Text, totalObjectivesText, gameplayTimeText, totalCO2AusWide;
    #endregion
    private void Awake()
    {
        instance = this;
        FinalScoreUI.SetActive(false);
    }

    public void TriggerFinalScoring()
    {
        CheckIfSideObjectivesAreActive();
        GetFinalPercentageOfWeights();
        finalScore = FindFinalScoreInKgOfCO2Saved();
        FindFinalMark();
    }

    private void GetFinalPercentageOfWeights()
    {
        float tutorialPercentage = GetPercentages(ObjectiveManager.instance.TutorialObjectives);
        float mainPercentages = GetPercentages(ObjectiveManager.instance.MainObjectives);
        float sidePercentages = GetPercentages(ObjectiveManager.instance.SideObjetives);
        float endPercentages = GetPercentages(ObjectiveManager.instance.EndObjectives);
        float combinePercentages = tutorialPercentage + mainPercentages + sidePercentages + endPercentages;
        finalScorePercentage = Mathf.RoundToInt(combinePercentages);
    }

    private float GetPercentages(List<Objective> objList)
    {
        float percentage = 0f;
        foreach(Objective obj in objList)
        {
            float per = (float)obj.puzzleCompletionPercentage / 100f;
            per *= obj.objectiveWeight;
            percentage += per;
        }
        return percentage;
    }

    private List<Objective> toAdd = new List<Objective>();
    private void CheckIfSideObjectivesAreActive()
    {
        foreach(Objective toCheckObj in allSideObjectives)
        {
            toAdd.Add(toCheckObj);
            if(ObjectiveManager.instance.SideObjetives.Count != 0)
            {
                //Debug.Log("theres objectives in the list");
                foreach(Objective obj in ObjectiveManager.instance.SideObjetives)
                {
                    if(toCheckObj == obj)
                    {
                        toAdd.Remove(toCheckObj);
                    }
                }
            }
        }

        foreach(Objective obj in toAdd)
        {
            ObjectiveManager.instance.AddNewSideObjective(obj);
        }
    }

    private float FindFinalScoreInKgOfCO2Saved()
    {
        return finalScorePercentage * TrackingController.instance.co2SavedPerAnnumInKg;
    }

    private void FindFinalMark()
    {
        if(finalScorePercentage <= 20)
        {
            finalMark = FinalMark.ClimateConscious;
            totalStars = 1;
        }
        else if(finalScorePercentage > 20 && finalScorePercentage <= 40)
        {
            finalMark = FinalMark.Greenthumb;
            totalStars = 2;
        }
        else if(finalScorePercentage > 40 && finalScorePercentage <= 60)
        {
            finalMark = FinalMark.CarbonCutter;
            totalStars = 3;
        }
        else if(finalScorePercentage > 60 && finalScorePercentage <= 80)
        {
            finalMark = FinalMark.EcoChampion;
            totalStars = 4;
        }
        else if(finalScorePercentage > 80 && finalScorePercentage <= 100)
        {
            finalMark = FinalMark.TheModelCitizen;
            totalStars = 5;
        }
    }

    public void DisplayFinalScoreUI()
    {
        switch(finalMark)
        {
            case FinalMark.ClimateConscious:
                finalGradeText.text = "Climate Conscious";
                break;
            case FinalMark.Greenthumb:
                finalGradeText.text = "Greenthumb";
                break;
            case FinalMark.CarbonCutter:
                finalGradeText.text = "Carbon Cutter";
                break;
            case FinalMark.EcoChampion:
                finalGradeText.text = "Eco Champion";
                break;
            case FinalMark.TheModelCitizen:
                finalGradeText.text = "The Model Citizen";
                break;
            default:
                finalGradeText.text =  "This is Awkward...";
                break;
        }
        for(int i = 0; i < totalStars; i++)
        {
            starImages[i].sprite = fullStarSprite;
        }
        FinalScoreUI.SetActive(true);
        FinalScoreUI.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, 0.5f);
        PauseMenu.instance.inDialogue = true;
        PlayerInputController.instance.DisablePlayerControls();
    }

    public void QuitGame()
    {
        if(Application.platform != RuntimePlatform.WebGLPlayer)
        {
            Application.Quit();
        }
    }

    private void GetStats()
    {
        int[] time = TrackingController.instance.GetGameTime();
        gameplayTimeText.text = "Gametime: " +time[0] +":" +time[1] +":" +time[2];
        totalCO2Text.text = "Kg of CO2 Saved per year: " +TrackingController.instance.co2SavedPerAnnumInKg;
        TrackingController.instance.CalculateTotalCO2SavedCountryWide();
        totalCO2AusWide.text = "Kg Of CO2 Saved Aus Wide: " +TrackingController.instance.co2SavedPerAnnumByTotalHouseholdsInKg;
        totalObjectivesText.text = "Completed Objectives: " +TrackingController.instance.completedObjectives;
    }
    public void ShowStatsUI()
    {
        GetStats();
        FinalScoreUI.SetActive(false);
        statsUI.SetActive(true);
    }
    int currentStatsPage = 1;
    public void StatsNextPage()
    {
        currentStatsPage++;
        if(currentStatsPage == 2)
        {
            //2nd page of  stats
            gameplayTimeText.text = "Total Mistakes: " +TrackingController.instance.totalMistakes;
            int[] devices = TrackingController.instance.GetTotalUnpluggedDevices();
            totalCO2Text.text = "Unplugged Devices: " +devices[0] +"/" +devices[1];
            totalCO2AusWide.text = "CCS Documents Viewed: " +TrackingController.instance.GetTotalCCSDocumentsLookedAt() +"/2";
            totalObjectivesText.text = "Correct trash items: " +TrackingController.instance.GetTotalCorrectTrashItems() +"/" +PuzzleManager.instance.garbageItems.Count;
        }
        else if(currentStatsPage == 3)
        {
            //3rd page of stats
            gameplayTimeText.text = "Bulbs trashed: " +TrackingController.instance.GetTotalOfBulbsTrashed() +"/" +PuzzleManager.instance.lightHousings.Count;
            totalObjectivesText.text = "Total Compromised Bins: " +TrackingController.instance.GetTotalCompromisedBins() +"/" +PuzzleManager.instance.garbageBins.Count;
            totalCO2AusWide.text = "";
            totalCO2AusWide.text = "";
            nextPageButton.SetActive(false);
        }
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
