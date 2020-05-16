using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingController : MonoBehaviour
{
    #region Static Setup
    public static TrackingController instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion
    #region Variables
    //BASE GAME VARIABLES
    public float gameTimeInSeconds = 0f; //Game time in seconds
    public int completedObjectives {get; set;} //Total objectives the playe completed
    public int totalObjectives {get; set;} //Total objectives in the game
    public int totalMistakes {get; set;} //How many times the player could of chosen a better option

    //POWER VARIABLES
    public static readonly float s_benchmarkPowerGeneratedInKW = 0f;//Base power generated
    public float avgDailyPowerGeneratedInKw = 0f; //Average power generated daily by solar
    public static readonly float s_benchmarkPowerConsumedInKw = 0f;// Base power consumed
    public float avgDailyPowerConsumedInKw = 0f; //Average power consumed  daily by the player
    public float avgDailyPowerUsageInKw = 0f;// Average power usage based on power consumed and generated

    //WATER VARIABLES
    public static readonly float s_benchmarkWaterConsumedInL = 0f;//Base water usage
    public float avgDailyWaterConsumedInL = 0f; //Average water consumed daily

    //GAS VARIABLES
    public static readonly float s_benchmarkNaturalGasConsumedInUnits = 0f;//Base natural gas useage
    public float avgDailyNaturalGasConsumedInUnits = 0f; //Average Daily natural gas usage
    
    //EMMISIONS VARIABLES
    public static readonly float s_benchmarkCarbonFootprintInGHG = 0f;//Base carbon footprint in Greenhouse Gas
    public float avgDailyCarbonFootprintInGHG  = 0f;

    //PUZZLE VARIABLES


    #endregion

    #region Update
     private void Update()
     {
         
     }
    #endregion

    #region Set and Add Functions
    
    #endregion

    #region Tracked Metrics Functions

        #region PowerGenerated
        public void IncreasePowerGenerated(float amount)
        {
            avgDailyPowerGeneratedInKw += amount;
        }

        public void DecreasePowerGenerated(float amount)
        {
            avgDailyPowerGeneratedInKw -= amount;
        }
        #endregion

        #region PowerConsumed
        public void IncreasePowerConsumed(float amount)
        {
            avgDailyPowerConsumedInKw += amount;
        }
            public void DecreasePowerConsumed(float amount)
        {
            avgDailyPowerConsumedInKw -= amount;
        }
        #endregion

        #region Water Usage
        public void IncreaseWaterUsed(float amount)
        {
            avgDailyWaterConsumedInL += amount;
        }
        public void DecreaseWaterUsed(float amount)
        {
            avgDailyWaterConsumedInL -= amount;
        }
        #endregion

        #region Natural Gas Usage
        public void IncreaseNaturalGasUsage(float amount)
        {
            avgDailyNaturalGasConsumedInUnits += amount;
        }

        public void DecreaseNaturalGasUsage(float amount)
        {
            avgDailyNaturalGasConsumedInUnits -= amount;
        }
        #endregion

        #region Global Emmisions
        public void IncreaseCarbonFootprint(float amount)
        {
            avgDailyCarbonFootprintInGHG += amount;
        }
        public void DecreaseCarbonFootprint(float amount)
        {
            avgDailyCarbonFootprintInGHG -= amount;
        }
        #endregion
    #endregion

    #region Puzzle Functions

    #endregion

}
