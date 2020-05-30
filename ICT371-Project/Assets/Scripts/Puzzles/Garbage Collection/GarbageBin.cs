using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageBin : MonoBehaviour
{
    public enum GarbageType {General, Recycling, Organics};
    public GarbageType garbageType;
    public int numberOfCorrectItemsInBin = 0;
    public int numberOfIncorrectItemsInBin = 0;
    public int numberOfIndisposableItemsInBin = 0;
    public int numberOfBulbsInBin = 0;
    public bool isCompromised = false;
    public float totalCo2Saved;
    public Dialogue indisposableItemDialogue;

    public int binTotal = 0;

    public Transform respawnLocation;

    public bool isFull = false;

    public CollectionSounds collectionSounds;

    private void Start() 
    {
        PuzzleManager.instance.AddGarbageBin(this);
        if(garbageType == GarbageType.General)
        {
            PuzzleManager.instance.AddGeneralWasteBin(this);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if(!isFull)
        {
            if(other.transform.gameObject.GetComponent<GarbageItem>())
            {
                GarbageItem gi = other.transform.gameObject.GetComponent<GarbageItem>();
                if(gi.itemType == GarbageItem.ItemType.Normal)
                { 
                    //Item is a garbage item
                    if(gi.garbageType == garbageType)
                    {
                        Destroy(other.transform.gameObject);
                        //Debug.Log("Player placed Correct item in the bin");
                        numberOfCorrectItemsInBin++;
                        CheckItem(gi);
                        binTotal++;
                        SoundEffectsManager.instance.PlayCorrectBinItemClip();
                        PuzzleManager.instance.CheckGarbageCollectionComplete();

                    }
                    else
                    {
                        Destroy(other.transform.gameObject);
                        numberOfIncorrectItemsInBin++;
                        isCompromised = true;
                        CheckItem(gi);
                        binTotal++;
                        TrackingController.instance.totalMistakes++;
                        SoundEffectsManager.instance.PlayIncorrectBinItemClip();
                        PuzzleManager.instance.CheckGarbageCollectionComplete();

                    }
                }
                else
                {
                    //Item isnt a notmal garbage item so must be a bulb
                    if(gi.garbageType == garbageType)
                    {
                        Destroy(gi.gameObject);
                        numberOfBulbsInBin++;
                        CheckItem(gi);
                        SoundEffectsManager.instance.PlayCorrectBinItemClip();
                        PuzzleManager.instance.CheckBulbCollectionComplete();
                    }
                    else
                    {
                        Destroy(gi.gameObject);
                        numberOfIncorrectItemsInBin++;
                        CheckItem(gi);
                        TrackingController.instance.totalMistakes++;
                        SoundEffectsManager.instance.PlayIncorrectBinItemClip();
                        PuzzleManager.instance.CheckBulbCollectionComplete();

                    }
                }
            }
            else if(!other.gameObject.CompareTag("Player"))
            {
                numberOfIndisposableItemsInBin++;
                DialogueManager.instance.StartDialogue(indisposableItemDialogue);
                //Debug.Log("Player placed a puzzle item in the bin");
                ResetLocation(other.transform.gameObject);
                TrackingController.instance.totalMistakes++;
                SoundEffectsManager.instance.PlayInDisposableBinItemClip();
            }
        }
        else if(!other.gameObject.CompareTag("Player"))
        {
            numberOfIndisposableItemsInBin++;
            //Debug.Log("Player placed a puzzle item in the bin");
            ResetLocation(other.transform.gameObject);
            TrackingController.instance.totalMistakes++;
            SoundEffectsManager.instance.PlayInDisposableBinItemClip();
        }

    }

    private void ResetLocation(GameObject item)//Used to reset a indisposible item to a rejected spawn point
    {
        item.transform.position = respawnLocation.position;
    }

    private void CheckItem(GarbageItem gi)
    {
        switch (gi.item)
        {
            case GarbageItem.Item.Apple:
                if(!isCompromised)
                {
                    TrackingController.instance.AddFoGoWaste(TrackingController.FoGoType.Apple);
                    totalCo2Saved += 618.3216f;
                }
                else
                {
                    TrackingController.instance.RemoveWasteAmount(totalCo2Saved);
                    totalCo2Saved = 0f;
                }
                break;
            case GarbageItem.Item.PizzaBox:
                if(!isCompromised)
                {
                    TrackingController.instance.AddFoGoWaste(TrackingController.FoGoType.PizzaBox);
                    totalCo2Saved += 618.3216f;
                }
                else
                {
                    TrackingController.instance.RemoveWasteAmount(totalCo2Saved);
                    totalCo2Saved = 0f;
                }
                break;
            case GarbageItem.Item.PopStick:
                if(!isCompromised)
                {
                    TrackingController.instance.AddFoGoWaste(TrackingController.FoGoType.PopStick);
                    totalCo2Saved += 1268.9f;
                }
                else
                {
                    TrackingController.instance.RemoveWasteAmount(totalCo2Saved);
                    totalCo2Saved = 0f;
                }
                break;
            case GarbageItem.Item.TpRole:
                if(!isCompromised)
                {
                    TrackingController.instance.AddRecycledWaste(TrackingController.RecycledType.TPRole);
                    totalCo2Saved += 2421.9f;
                }
                else
                {
                    TrackingController.instance.RemoveWasteAmount(totalCo2Saved);
                    totalCo2Saved = 0f;
                }
                break;
            case GarbageItem.Item.ColaCan:
                if(!isCompromised)
                {
                    TrackingController.instance.AddRecycledWaste(TrackingController.RecycledType.ColaCan);
                    totalCo2Saved += 1235.52f;
                }
                else
                {
                    TrackingController.instance.RemoveWasteAmount(totalCo2Saved);
                    totalCo2Saved = 0f;
                }
                break;
            case GarbageItem.Item.ChipPacket:
                if(!isCompromised)
                {
                    TrackingController.instance.AddRecycledWaste(TrackingController.RecycledType.ChipPacket);
                    totalCo2Saved += 4766.58f;
                }
                else
                {
                    TrackingController.instance.RemoveWasteAmount(totalCo2Saved);
                    totalCo2Saved = 0f;
                }
                break;
            case GarbageItem.Item.Toothpaste:
                if(!isCompromised)
                {
                    TrackingController.instance.AddGeneraldWaste(TrackingController.GeneralWasteType.Toothpaste);
                    totalCo2Saved += 0f;
                }
                else
                {
                    TrackingController.instance.RemoveWasteAmount(totalCo2Saved);
                    totalCo2Saved = 0f;
                }
                break;
            case GarbageItem.Item.CoffeeCup:
                if(!isCompromised)
                {
                    TrackingController.instance.AddGeneraldWaste(TrackingController.GeneralWasteType.CoffeeCup);
                    totalCo2Saved += 0f;
                }
                else
                {
                    TrackingController.instance.RemoveWasteAmount(totalCo2Saved);
                    totalCo2Saved = 0f;
                }
                break;
            case GarbageItem.Item.Razor:
                if(!isCompromised)
                {
                    TrackingController.instance.AddGeneraldWaste(TrackingController.GeneralWasteType.RustyRazor);
                    totalCo2Saved += 0f;
                }
                else
                {
                    TrackingController.instance.RemoveWasteAmount(totalCo2Saved);
                    totalCo2Saved = 0f;
                }
                break;
            case GarbageItem.Item.Bulb:
                if(!isCompromised)
                {
                    TrackingController.instance.AddRecycledWaste(TrackingController.RecycledType.LightBulb);
                    totalCo2Saved += 38.61f;
                }
                else
                {
                    TrackingController.instance.RemoveWasteAmount(totalCo2Saved);
                    totalCo2Saved = 0f;
                }
                break;
            
            default:
                break;
        }
    }
}
