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

    public Transform respawnLocation;

    public bool isFull = false;
    private int maxItems = 0;
    private int currentItems = 0;

    public CollectionSounds collectionSounds;

    private void Start() 
    {
        PuzzleManager.instance.AddGarbageBin(this);
        Invoke("FindGarbageItems", 1f);
    }

    private void FindGarbageItems()
    {
        foreach(GameObject go in PuzzleManager.instance.GetGarbageList())
        {
            maxItems++;
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if(!isFull)
        {
            if(other.transform.gameObject.GetComponent<GarbageItem>())
            {
                //Item is a garbage item
                GarbageItem gi = other.transform.gameObject.GetComponent<GarbageItem>();
                if(gi.garbageType == garbageType)
                {
                    Destroy(other.transform.gameObject);
                    numberOfCorrectItemsInBin++;
                    currentItems++;
                    collectionSounds.SetAudioClipAndPlay(true, false);
                }
                else
                {
                    numberOfIncorrectItemsInBin++;
                    Debug.Log("Player placed a item that shouldn't be in this bin, moving to respaw location");
                    Destroy(other.transform.gameObject);
                    currentItems++;
                    numberOfIncorrectItemsInBin++;
                    TrackingController.instance.totalMistakes++;
                    collectionSounds.SetAudioClipAndPlay(false, false);
                }
            }
            else
            {
                numberOfIndisposableItemsInBin++;
                Debug.Log("Player placed a item that shouldn't be disposed into the bin, moving to respawn location");
                ResetLocation(other.transform.gameObject);
                TrackingController.instance.totalMistakes++;
                collectionSounds.SetAudioClipAndPlay(false, true);
            }
        }

        if(currentItems == maxItems)//Checking to see if the current amount of items in the bin is the max, if so bins full
        {
            isFull = true;
            PuzzleManager.instance.CheckGarbageCollectionComplete();
        }
    }

    private void ResetLocation(GameObject item)//Used to reset a indisposible item to a rejected spawn point
    {
        item.transform.position = respawnLocation.position;
    }
}
