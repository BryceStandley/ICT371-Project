using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clothesline : MonoBehaviour
{
    public GameObject[] clothes;

    public Dialogue clothesOnClothesLineDialogue;
    public void AcceptBasket(GameObject other)
    {
        WashedBasket basket = other.GetComponent<WashedBasket>();
        if(basket != null)
        {
            if(basket.hasClothes)
            {
                DryClothes(basket.gameObject);
            }
        }
    }

    private void DryClothes(GameObject basket)
    {
        ObjectPickUp.instance.DropItem(basket);
        basket.GetComponent<Rigidbody>().useGravity = false;
        basket.transform.position = new Vector3(0,-100,0);//Move basket under world with no gravity
        foreach(GameObject go in clothes)
        {
            go.SetActive(true);
        }
        PuzzleManager.instance.SetDryClothesObjectiveComplete();
        TrackingController.instance.tempClothesDriedAt = 30;
        DialogueManager.instance.StartDialogue(clothesOnClothesLineDialogue);

    }
}
