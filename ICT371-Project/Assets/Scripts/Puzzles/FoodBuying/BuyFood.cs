using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyFood : MonoBehaviour
{
    public GameObject foodBuyUI;
    public FoodOrderer foodOrderer;

    private void Awake()
    {
        foodBuyUI.SetActive(false);
    }
    public void OpenFoodBuyUI()
    {
        foodBuyUI.SetActive(true);
        PauseMenu.instance.inDialogue = true;
        PlayerInputController.instance.DisablePlayerControls();
        PuzzleManager.instance.AddFoodBuyObjective();
    }

    public void BuyBeef()
    {
        //trigger tracking for beef
        //Debug.Log("buying beef...");
        HideBuyUI();
        foodOrderer.foodBought = true;
        PuzzleManager.instance.TriggerFoodBuyComplete();
        TrackingController.instance.AddBoughtFood(TrackingController.FoodBoughtType.Beef);

    }

    public void BuyFish()
    {
        //trigger tracking for organic
        //Debug.Log("buying fish...");
        HideBuyUI();
        foodOrderer.foodBought = true;
        PuzzleManager.instance.TriggerFoodBuyComplete();
        TrackingController.instance.AddBoughtFood(TrackingController.FoodBoughtType.Fish);
    }

    public void BuyVeggie()
    {
        //trigger tracking for local
        //Debug.Log("buying veggie...");
        HideBuyUI();
        foodOrderer.foodBought = true;
        PuzzleManager.instance.TriggerFoodBuyComplete();
        TrackingController.instance.AddBoughtFood(TrackingController.FoodBoughtType.Veggie);

    }

    private void HideBuyUI()
    {
        foodBuyUI.SetActive(false);
        PauseMenu.instance.inDialogue = false;
        PlayerInputController.instance.EnablePlayerControls();
    }
}
