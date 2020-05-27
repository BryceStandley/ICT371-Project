using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dryer : MonoBehaviour
{
    public GameObject washedBasket;
    public GameObject washedBasketLocation;
    public GameObject temperatureUI;
    public TemperatureController temperatureController;
    public int tempDryedAt = 0;
    public bool dryingComplete;
    public bool allowedToUseDryer = true;
    private bool hasShownDialogue = false;
    public Dialogue dryerUnpluggedDialogue;

    public void AcceptWashedBasket(GameObject basket)
    {
        if(basket.GetComponent<WashedBasket>().hasClothes)
        {
            if(allowedToUseDryer)
            {
                ObjectPickUp.instance.DropItem(basket);
                //move new basket ontop of machine
                washedBasket.transform.position = washedBasketLocation.transform.position;
                washedBasket.transform.rotation = washedBasketLocation.transform.rotation;
                washedBasket.GetComponent<WashedBasket>().SetEmptyBasket();
                //display washing machine ui
                temperatureController.MakeDryer();
            }
            else if(!hasShownDialogue)
            {
                DialogueManager.instance.StartDialogue(dryerUnpluggedDialogue);
                hasShownDialogue = true;
            }
        }
    }

    public void ResetBasket()
    {
        washedBasket.transform.position = washedBasketLocation.transform.position;
        washedBasket.transform.rotation = washedBasketLocation.transform.rotation;
        washedBasket.GetComponent<Rigidbody>().useGravity = true;
    }
}
