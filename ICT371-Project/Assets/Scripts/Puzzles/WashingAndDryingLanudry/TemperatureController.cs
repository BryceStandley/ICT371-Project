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
    public WashingMachine washingMachine;
    public Dryer dryer;
    public float machineTimersInSeconds = 60;
    public TimerUI washingMachineTimerUI;
    public TimerUI dryerTimerUI;

    public Dialogue clothesInWashingMachineDialogue;
    public Dialogue clothesInDryerDialogue;
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
        PauseMenu.instance.inDialogue = false;
        PlayerInputController.instance.EnablePlayerControls();
        if(isWashingMachine)
        {
            washingMachineTimerUI.gameObject.SetActive(true);
            StartCoroutine("UpdateWasherTimer");
            DialogueManager.instance.StartDialogue(clothesInWashingMachineDialogue);
        }
        else
        {
            dryerTimerUI.gameObject.SetActive(true);
            StartCoroutine("UpdateDryerTimer");
            DialogueManager.instance.StartDialogue(clothesInDryerDialogue);
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
            yield return new WaitForSecondsRealtime(1);
        }
    }
    IEnumerator UpdateDryerTimer()
    {
        timer = 0;
        for(int i = 0; i < machineTimersInSeconds; i++)
        {
            timer += 100 / machineTimersInSeconds;
            dryerTimerUI.UpdateSliderVal(timer);
            yield return new WaitForSecondsRealtime(1);
        }
    }

    private void Update()
    {
        if(washingMachine.washComplete)
        {
            washingMachineTimerUI.UpdateSliderVal(100);
        }
        else if(dryer.dryingComplete)
        {
            dryerTimerUI.UpdateSliderVal(100);
        }
    }

    private void EndTimer()
    {
        if(isWashingMachine)
        {
            washingMachine.washComplete = true;
            washingMachine.tempWashedAt = tempWashedAt;
            TrackingController.instance.tempClothesWashedAt = tempWashedAt;
            washingMachine.ResetBasket();
            PuzzleManager.instance.SetWashingClothesObjectiveComplete();
        }
        else
        {
            // do stuff from the dryer
            dryer.tempDryedAt = tempWashedAt;
            TrackingController.instance.tempClothesDriedAt = tempWashedAt;
            TrackingController.instance.driedWithDryer = true;
            dryer.dryingComplete = true;
            dryer.ResetBasket();
            //Add change of percentage based on how the clothes were dried
            PuzzleManager.instance.SetDryClothesObjectiveComplete();
        }
    }
}
