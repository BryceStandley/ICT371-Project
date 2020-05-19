using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TemperatureController : MonoBehaviour
{
    public TextMeshProUGUI machineName;
    public GameObject temperatureUI;
    public int tempWashedAt = 0;
    public bool isWashingMachine = false;
    public GameObject washingMachine;
    public GameObject dryer;
    public float machineTimersInSeconds = 60f;
    public void MakeWashingMachine()
    {
        machineName.text = "Washing Machine";
        isWashingMachine = true;
    }

    public void MakeDryer()
    {
        machineName.text = "Dryer";
        isWashingMachine = false;
    }

    public void SetTemperature(int temp)
    {   
        tempWashedAt = temp;
        temperatureUI.SetActive(false);
        PlayerInputController.instance.EnablePlayerControls();
        Invoke("EndTimer", machineTimersInSeconds);//Starting a timer that waits 60 seconds before machine is finished;
    }

    private void EndTimer()
    {
        if(isWashingMachine)
        {
            WashingMachine washer = washingMachine.GetComponent<WashingMachine>();
            washer.washComplete = true;
            washer.tempWashedAt = tempWashedAt;
            washer.ResetBasket();
            PuzzleManager.instance.SetWashingClothesObjectiveComplete();
        }
        else
        {
            // do stuff from the dryer
            Dryer dry = dryer.GetComponent<Dryer>();
            dry.tempDryedAt = tempWashedAt;
            dry.dryingComplete = true;
            dry.ResetBasket();
            PuzzleManager.instance.SetDryClothesObjectiveComplete();
        }
    }
}
