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
    public float machineTimersInSeconds = 60f;
    public float delayTakingWashing = 10f;
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
        dryerTimerUI.gameObject.SetActive(true);
        StartCoroutine("UpdateDryerTimer");
        DialogueManager.instance.StartDialogue(clothesInDryerDialogue);
        isWashingMachine = false;
        Invoke("EndTimer", machineTimersInSeconds);
    }

    public void SetTemperature(int temp)
    {   
        tempWashedAt = temp;
        temperatureUI.SetActive(false);
        PauseMenu.instance.ChangeSelectedItem(PauseMenu.instance.pauseFirstButton);
        ObjectInformationToolTip.HideTip();
        ObjectInformationToolTip.HidePrompt();
        PauseMenu.instance.inDialogue = false;
        PlayerInputController.instance.EnablePlayerControls();
        if(isWashingMachine)
        {
            washingMachineTimerUI.gameObject.SetActive(true);
            StartCoroutine("UpdateWasherTimer");
            DialogueManager.instance.StartDialogue(clothesInWashingMachineDialogue);
            ObjectPickUp.instance.DropItem(washingMachine.fullBasket);
            //destroy old basket
            Destroy(washingMachine.fullBasket);
            //move new basket ontop of machine
            washingMachine.washedBasket.transform.position = washingMachine.washedBasketLocation.transform.position;
            washingMachine.washedBasket.transform.rotation = washingMachine.washedBasketLocation.transform.rotation;
            washingMachine.washedBasket.GetComponent<Rigidbody>().useGravity = true;
        }
        Invoke("EndTimer", machineTimersInSeconds);//Starting a timer that waits x seconds before machine is finished;
    }

    public void QuitUI()
    {
        temperatureUI.SetActive(false);
        PauseMenu.instance.inDialogue = false;
        PlayerInputController.instance.EnablePlayerControls();
        washingMachine.delayTakingWashing = true;
        Invoke("DelayTakingNewBasket", delayTakingWashing);
    }
    private void DelayTakingNewBasket()
    {
        washingMachine.delayTakingWashing = false;
        washingMachine.showedUI = false;
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

    private void EndTimer()
    {
        if(isWashingMachine)
        {
            washingMachine.washComplete = true;
            washingMachine.tempWashedAt = tempWashedAt;
            washingMachine.ResetBasket();
            PuzzleManager.instance.SetWashingClothesObjectiveComplete();
            washingMachineTimerUI.UpdateSliderVal(100);
            washingMachineTimerUI.gameObject.SetActive(false);
            washingMachine.powerSocket.isWasher = false;
            if(tempWashedAt == 30)
            {
                TrackingController.instance.AddWashingClothesFootprint(TrackingController.TemperatureUsed.Cold);
                TrackingController.instance.temperatureUsedToWashClothes = TrackingController.TemperatureUsed.Cold;
            }
            else if(tempWashedAt == 40)
            {
                TrackingController.instance.AddWashingClothesFootprint(TrackingController.TemperatureUsed.Warm);
                TrackingController.instance.temperatureUsedToWashClothes = TrackingController.TemperatureUsed.Warm;
            }
            else
            {
                TrackingController.instance.AddWashingClothesFootprint(TrackingController.TemperatureUsed.Hot);
                TrackingController.instance.temperatureUsedToWashClothes = TrackingController.TemperatureUsed.Hot;
            }
        }
        else
        {
            // do stuff from the dryer
            dryer.dryingComplete = true;
            dryer.ResetBasket();
            TrackingController.instance.playerUsedDryer = true;
            PuzzleManager.instance.SetDryClothesObjectiveComplete();
            dryerTimerUI.UpdateSliderVal(100);
            dryerTimerUI.gameObject.SetActive(false);
        }
    }
}
