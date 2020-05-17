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

    public CollectionSounds collectionSounds;

    private void Start() 
    {
        PuzzleManager.instance.AddGarbageBin(this);
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
                    //Debug.Log("Player placed Correct item in the bin");
                    numberOfCorrectItemsInBin++;
                    collectionSounds.SetAudioClipAndPlay(true, false);
                    PuzzleManager.instance.CheckGarbageCollectionComplete();
                }
                else
                {
                    numberOfIncorrectItemsInBin++;
                    //Debug.Log("Player placed Incorrect item in the bin");
                    Destroy(other.transform.gameObject);
                    numberOfIncorrectItemsInBin++;
                    TrackingController.instance.totalMistakes++;
                    collectionSounds.SetAudioClipAndPlay(false, false);
                    PuzzleManager.instance.CheckGarbageCollectionComplete();
                }
            }
            else
            {
                numberOfIndisposableItemsInBin++;
                //Debug.Log("Player placed a puzzle item in the bin");
                ResetLocation(other.transform.gameObject);
                TrackingController.instance.totalMistakes++;
                collectionSounds.SetAudioClipAndPlay(false, true);
                PuzzleManager.instance.CheckGarbageCollectionComplete();
            }
        }

    }

    private void ResetLocation(GameObject item)//Used to reset a indisposible item to a rejected spawn point
    {
        item.transform.position = respawnLocation.position;
    }
}
