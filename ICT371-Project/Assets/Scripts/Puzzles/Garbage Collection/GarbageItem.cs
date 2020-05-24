using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageItem : MonoBehaviour
{
    public GarbageBin.GarbageType garbageType;
    public enum ItemType {Normal, Bulb};
    public ItemType itemType = ItemType.Normal;

    private void Start()
    {
        PuzzleManager.instance.AddGarbageItem(this);
    }
}
