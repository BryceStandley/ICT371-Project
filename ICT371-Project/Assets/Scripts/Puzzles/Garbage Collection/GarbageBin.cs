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
                        binTotal++;
                        collectionSounds.SetAudioClipAndPlay(true, false);
                        PuzzleManager.instance.CheckGarbageCollectionComplete();

                    }
                    else
                    {
                        Destroy(other.transform.gameObject);
                        numberOfIncorrectItemsInBin++;
                        binTotal++;
                        TrackingController.instance.totalMistakes++;
                        collectionSounds.SetAudioClipAndPlay(false, false);
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
                        collectionSounds.SetAudioClipAndPlay(true, false);
                        PuzzleManager.instance.CheckBulbCollectionComplete();
                    }
                    else
                    {
                        Destroy(gi.gameObject);
                        numberOfIncorrectItemsInBin++;
                        TrackingController.instance.totalMistakes++;
                        collectionSounds.SetAudioClipAndPlay(false, false);
                        PuzzleManager.instance.CheckBulbCollectionComplete();

                    }
                }
            }
            else if(!other.gameObject.CompareTag("Player"))
            {
                numberOfIndisposableItemsInBin++;
                //Debug.Log("Player placed a puzzle item in the bin");
                ResetLocation(other.transform.gameObject);
                TrackingController.instance.totalMistakes++;
                collectionSounds.SetAudioClipAndPlay(false, true);
            }
        }
        else if(!other.gameObject.CompareTag("Player"))
        {
            numberOfIndisposableItemsInBin++;
            //Debug.Log("Player placed a puzzle item in the bin");
            ResetLocation(other.transform.gameObject);
            TrackingController.instance.totalMistakes++;
            collectionSounds.SetAudioClipAndPlay(false, true);
        }

    }

    private void ResetLocation(GameObject item)//Used to reset a indisposible item to a rejected spawn point
    {
        item.transform.position = respawnLocation.position;
    }
}
