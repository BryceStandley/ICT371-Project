using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WashingMachine : MonoBehaviour
{
    public GameObject fullBasket;
    public GameObject washedBasket;
    public GameObject washedBasketLocation;
    public GameObject temperatureUI;
    public TemperatureController temperatureController;
    public int tempWashedAt = 0;
    public bool washComplete = false;
    public PowerSocket powerSocket;
    public bool delayTakingWashing = false;
    public bool showedUI = false;

    public void TakeWashing(GameObject other)
    {
        if(other.gameObject.GetComponent<WashedBasket>() && washComplete)
        {
            //player is taking washed cloths out of the machine
            //tell washed basket to display full basket mesh
            WashedBasket basket = washedBasket.GetComponent<WashedBasket>();
            basket.SetFullBasket();
            basket.tempWashedAt = tempWashedAt;
            washComplete=false;
        }
    }

    public void AcceptLaundryBasket(GameObject basket)
    {
        if(basket.GetComponent<LaundryBasket>().isFull)
        {
            if(!delayTakingWashing)
            {
                if(!showedUI)
                {
                    //player has placed laundry basket into the washing machine
                    fullBasket = basket;
                    Invoke("SetUpBasket", 0.1f);
                }
            }
        }
    }
    
    private void SetUpBasket()
    {
        //display washing machine ui
        temperatureController.MakeWashingMachine();
        temperatureUI.SetActive(true);
        ObjectInformationToolTip.HideTip();
        ObjectInformationToolTip.HidePrompt();
        showedUI = true;
        PauseMenu.instance.inDialogue = true;
        PlayerInputController.instance.DisablePlayerControls();
    }

    public void ResetBasket()
    {
        washedBasket.transform.position = washedBasketLocation.transform.position;
        washedBasket.transform.rotation = washedBasketLocation.transform.rotation;
        washedBasket.GetComponent<Rigidbody>().useGravity = true;
    }
}
