using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuyFood : MonoBehaviour
{
    public GameObject foodBuyUI;
    public FoodOrderer foodOrderer;
    public GameObject oldEsItem, fishButton;

    private void Awake()
    {
        foodBuyUI.SetActive(false);
    }
    public void OpenFoodBuyUI()
    {
        foodBuyUI.SetActive(true);
        PauseMenu.instance.ChangeSelectedItem(fishButton);
        PauseMenu.instance.inDialogue = true;
        PlayerInputController.instance.DisablePlayerControls();
    }

    public void BuyBeef()
    {
        //trigger tracking for beef
        //Debug.Log("buying beef...");
        HideBuyUI();
        foodOrderer.foodBought = true;
        PuzzleManager.instance.TriggerFoodBuyComplete();
        TrackingController.instance.AddBoughtFood(TrackingController.FoodBoughtType.Beef);
        TrackingController.instance.typeOfFoodThePlayerBought = TrackingController.FoodBoughtType.Beef;

    }

    public void BuyFish()
    {
        //trigger tracking for organic
        //Debug.Log("buying fish...");
        HideBuyUI();
        foodOrderer.foodBought = true;
        PuzzleManager.instance.TriggerFoodBuyComplete();
        TrackingController.instance.AddBoughtFood(TrackingController.FoodBoughtType.Fish);
        TrackingController.instance.typeOfFoodThePlayerBought = TrackingController.FoodBoughtType.Fish;
    }

    public void BuyVeggie()
    {
        //trigger tracking for local
        //Debug.Log("buying veggie...");
        HideBuyUI();
        foodOrderer.foodBought = true;
        PuzzleManager.instance.TriggerFoodBuyComplete();
        TrackingController.instance.AddBoughtFood(TrackingController.FoodBoughtType.Veggie);
        TrackingController.instance.typeOfFoodThePlayerBought = TrackingController.FoodBoughtType.Veggie;

    }

    private void HideBuyUI()
    {
        foodBuyUI.SetActive(false);
        PauseMenu.instance.ChangeSelectedItem(PauseMenu.instance.pauseFirstButton);
        PauseMenu.instance.inDialogue = false;
        PlayerInputController.instance.EnablePlayerControls();
        ObjectInformationToolTip.HideTip();
        ObjectInformationToolTip.HidePrompt();
    }
}
