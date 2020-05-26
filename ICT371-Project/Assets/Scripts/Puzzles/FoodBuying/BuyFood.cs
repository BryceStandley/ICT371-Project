﻿using System.Collections;
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
        Debug.Log("buying beef...");
        HideBuyUI();
        foodOrderer.foodBought = true;
        PuzzleManager.instance.TriggerFoodBuyComplete();

    }

    public void BuyOrganic()
    {
        //trigger tracking for organic
        Debug.Log("buying organic...");
        HideBuyUI();
        foodOrderer.foodBought = true;
        PuzzleManager.instance.TriggerFoodBuyComplete();
    }

    public void BuyLocal()
    {
        //trigger tracking for local
        Debug.Log("buying local...");
        HideBuyUI();
        foodOrderer.foodBought = true;
        PuzzleManager.instance.TriggerFoodBuyComplete();

    }

    private void HideBuyUI()
    {
        foodBuyUI.SetActive(false);
        PauseMenu.instance.inDialogue = false;
        PlayerInputController.instance.EnablePlayerControls();
    }
}