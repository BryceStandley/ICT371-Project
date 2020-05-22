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
    public float machineTimersInSeconds = 60;
    public TimerUI washingMachineTimerUI;
    public TimerUI dryerTimerUI;
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
        if(isWashingMachine)
        {
            washingMachineTimerUI.gameObject.SetActive(true);
            StartCoroutine("UpdateWasherTimer");
        }
        else
        {
            dryerTimerUI.gameObject.SetActive(true);
            StartCoroutine("UpdateDryerTimer");
        }
        Invoke("EndTimer", machineTimersInSeconds);//Starting a timer that waits x seconds before machine is finished;
    }

    float timer = 0;
    IEnumerator UpdateWasherTimer()
    {
        for(int i = 0; i < machineTimersInSeconds; i++)
        {
            timer += 100 / machineTimersInSeconds;
            washingMachineTimerUI.UpdateSliderVal(timer);
            yield return new WaitForSeconds(1);
        }
    }
    IEnumerator UpdateDryerTimer()
    {
        for(int i = 0; i < machineTimersInSeconds; i++)
        {
            timer += 100 / machineTimersInSeconds;
            dryerTimerUI.UpdateSliderVal(timer);
            yield return new WaitForSeconds(1);
        }
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
            //Add change of percentage based on how the clothes were dried
            PuzzleManager.instance.SetDryClothesObjectiveComplete();
        }
    }
}
