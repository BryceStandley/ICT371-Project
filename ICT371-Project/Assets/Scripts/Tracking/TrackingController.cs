using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingController : MonoBehaviour
{
    #region Static Setup and Awake
    public static TrackingController instance;
    private void Awake()
    {
        instance = this;
        co2SavedPerAnnumInKg = 0;
    }
    #endregion

    #region Variables
    //BASE GAME VARIABLES
    public int gameTimeInSeconds = 0; //Game time in seconds
    public int completedObjectives {get; set;} //Total objectives the playe completed
    public int totalObjectives {get; set;} //Total objectives in the game
    public int totalMistakes {get; set;} //How many times the player could of chosen a better option

    //POWER VARIABLES
    public static readonly float s_benchmarkPowerGeneratedInKwh = 0f;//Base power generated
    public float avgDailyPowerGeneratedInKwh = 0f; //Average power generated daily by solar
    public static readonly float s_benchmarkPowerConsumedInKwh = 0f;// Base power consumed
    public float avgDailyPowerConsumedInKwh = 0f; //Average power consumed  daily by the player
    public float avgDailyPowerUsageInKwh = 0f;// Average power usage based on power consumed and generated

    //WATER VARIABLES
    public static readonly float s_benchmarkWaterConsumedInL = 0f;//Base water usage
    public float avgDailyWaterConsumedInL = 0f; //Average water consumed daily

    //GAS VARIABLES
    public static readonly float s_benchmarkNaturalGasConsumedInUnits = 0f;//Base natural gas useage
    public float avgDailyNaturalGasConsumedInUnits = 0f; //Average Daily natural gas usage
    
    //EMMISIONS VARIABLES
    public static readonly float s_benchmarkCarbonFootprintInKg = 0f;//Base carbon footprint in Greenhouse Gas
    public float avgDailyCarbonFootprintInKg  = 0f;
    public float co2SavedPerAnnumInKg {get; set;}//Used for the final scoring
    public float co2SavedPerAnnumByTotalHouseholdsInKg {get; set;}

    //PUZZLE VARIABLES

    public bool playerViewedCCSDoc1 = false;
    public bool playerViewedCCSDoc2 = false;
    public TemperatureUsed temperatureUsedToWashClothes;
    public bool playerUsedDryer = false;
    public FoodBoughtType typeOfFoodThePlayerBought;
    public TransportType typeOfTransportThePlayerUsed;


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
            avgDailyPowerGeneratedInKwh += amount;
        }

        public void DecreasePowerGenerated(float amount)
        {
            avgDailyPowerGeneratedInKwh -= amount;
        }
        #endregion

        #region PowerConsumed
        public void IncreasePowerConsumed(float amount)
        {
            avgDailyPowerConsumedInKwh += amount;
        }
            public void DecreasePowerConsumed(float amount)
        {
            avgDailyPowerConsumedInKwh -= amount;
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
            avgDailyCarbonFootprintInKg += amount;
        }
        public void DecreaseCarbonFootprint(float amount)
        {
            avgDailyCarbonFootprintInKg -= amount;
        }
        #endregion
    #endregion

    #region Puzzle Functions

        #region Tree Planting
        public void AddTreePlantingFootprint()
        {
            co2SavedPerAnnumInKg += 21.7724f * 3;
        }
        #endregion

        #region Laundry Collection
        
        #endregion

        #region Washing Clothes
        public enum TemperatureUsed {Cold, Worm, Hot};
        public void AddWashingClothesFootprint(TemperatureUsed temp)
        {  
            if(temp == TemperatureUsed.Worm)
            {
                co2SavedPerAnnumInKg += 185.64f;
            }
            else if(temp == TemperatureUsed.Cold)
            {
                co2SavedPerAnnumInKg += 458.64f;
            }

        }
        #endregion

        #region Rubbish Collection
        
        #endregion

        #region Light bulb changing
        public void AddOneLightbulbChanged()
        {
            co2SavedPerAnnumInKg += 8.687f;
        }
        #endregion

        #region Trashing Light bulbs
        
        #endregion

        #region Drying Clothes
        public void AddDryWithOutDryer()
        {
            co2SavedPerAnnumInKg += 518.7f;
        }
        #endregion

        #region Phantom Power
        public enum PhantomType {Microwave, LCDTV, PC, WashingMachine, Dryer, DeskLamp, LargeLamp};
        public void AddPhantomPowerSaved(PhantomType type)
        {
            switch(type)
            {
                case PhantomType.Microwave:
                    co2SavedPerAnnumInKg += 1106.2128f;
                    break;
                case PhantomType .LCDTV:
                    co2SavedPerAnnumInKg += 1060.1206f;
                    break;
                case PhantomType.PC:
                    co2SavedPerAnnumInKg += 13827.66f;
                    break;
                case PhantomType .WashingMachine:
                    co2SavedPerAnnumInKg += 1843.688f;
                    break;
                case PhantomType.Dryer:
                    co2SavedPerAnnumInKg += 1198.3972f;
                    break;
                case PhantomType.DeskLamp:
                    co2SavedPerAnnumInKg += 0f;
                    break;
                case PhantomType.LargeLamp:
                    co2SavedPerAnnumInKg += 0f;
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Buying Food
        public enum FoodBoughtType {Beef, Fish, Veggie};
        public void AddBoughtFood(FoodBoughtType type)
        {
            switch(type)
            {
                case FoodBoughtType.Beef:
                    co2SavedPerAnnumInKg += 0;
                    break;
                case FoodBoughtType.Fish:
                    co2SavedPerAnnumInKg += 10037.5f;
                    break;
                case FoodBoughtType.Veggie:
                    co2SavedPerAnnumInKg += 10767.5f;
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Leaving the House
        public enum TransportType {Car, Bike};
        public void AddTransportUsage(TransportType type)
        {
            switch(type)
            {
                case TransportType.Car:
                    co2SavedPerAnnumInKg += 0;
                    break;
                case TransportType.Bike:
                    co2SavedPerAnnumInKg += 3560.94f;
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Solar Activity Ariticle
        
        #endregion

        #region Earth Rotation Article
        
        #endregion

    #endregion

    #region Mist Functions
    public int[] GetGameTime()
    {
        int[] time = new int[3];
        gameTimeInSeconds = Mathf.RoundToInt(Time.time);
        int hours = Mathf.FloorToInt(gameTimeInSeconds / 3600f);
		int minutes = Mathf.FloorToInt((gameTimeInSeconds % 3600)/60);
		int seconds = Mathf.FloorToInt(gameTimeInSeconds % 60);
        time[0] = hours;
        time[1] = minutes;
        time[2] = seconds;
        return time;
    }

    public void CalculateTotalCO2SavedCountryWide()
    {
        co2SavedPerAnnumByTotalHouseholdsInKg = co2SavedPerAnnumInKg * (9204.6f * 1000);
    }

    public int[] GetTotalUnpluggedDevices()
    {
        int[] devices = new int[2];
        foreach(PowerSocket ps in PuzzleManager.instance.powerOutlets)
        {
            if(ps.isUnplugged)
            {
                devices[0]++;
            }
        }
        devices[1] = PuzzleManager.instance.powerOutlets.Count;
        return devices;
    }

    public int GetTotalCCSDocumentsLookedAt()
    {
        int total = 0;
        if(playerViewedCCSDoc1)
        {
            total++;
            if(playerViewedCCSDoc2)
            {
                total++;
            }
        }
        return total;
    }

    public int GetTotalCorrectTrashItems()
    {
        int totalCorrect = 0;
        foreach(GarbageBin gb in PuzzleManager.instance.garbageBins)
        {
            totalCorrect += gb.numberOfCorrectItemsInBin;
        }
        return totalCorrect;
    }

    public int GetTotalOfBulbsTrashed()
    {
        int totalCorrect = 0;
        foreach(GarbageBin gb in PuzzleManager.instance.garbageBins)
        {
            totalCorrect += gb.numberOfBulbsInBin;
        }
        return totalCorrect;
    }

    public int GetTotalCompromisedBins()
    {
        int totalCorrect = 0;
        foreach(GarbageBin gb in PuzzleManager.instance.garbageBins)
        {
            if(gb.isCompromised)
            {
                totalCorrect++;
            }
        }
        return totalCorrect;
    }

    #endregion

}
