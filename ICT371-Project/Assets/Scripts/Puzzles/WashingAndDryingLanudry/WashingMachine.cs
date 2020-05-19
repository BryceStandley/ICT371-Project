using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WashingMachine : MonoBehaviour
{
    public GameObject washedBasket;
    public GameObject washedBasketLocation;
    public GameObject temperatureUI;
    public TemperatureController temperatureController;
    public int tempWashedAt = 0;
    public bool washComplete = false;

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.GetComponent<WashedBasket>() && washComplete)
        {
            //player is taking washed cloths out of the machine
            //tell washed basket to display full basket mesh
            WashedBasket basket = washedBasket.GetComponent<WashedBasket>();
            basket.SetFullBasket();
            basket.tempWashedAt = tempWashedAt;
        }
    }

    public void AcceptLaundryBasket(GameObject basket)
    {
        if(basket.GetComponent<LaundryBasket>().isFull)
        {
            //player has placed laundry basket into the washing machine
            ObjectPickUp.instance.DropItem(basket);
            //destroy old basket
            Destroy(basket);
            //move new basket ontop of machine
            washedBasket.transform.position = washedBasketLocation.transform.position;
            washedBasket.transform.rotation = washedBasketLocation.transform.rotation;
            washedBasket.GetComponent<Rigidbody>().useGravity = true;
            //display washing machine ui
            temperatureController.MakeWashingMachine();
            temperatureUI.SetActive(true);
            PlayerInputController.instance.DisablePlayerControls();
        }
    }

    public void ResetBasket()
    {
        washedBasket.transform.position = washedBasketLocation.transform.position;
        washedBasket.transform.rotation = washedBasketLocation.transform.rotation;
        washedBasket.GetComponent<Rigidbody>().useGravity = true;
    }
}
