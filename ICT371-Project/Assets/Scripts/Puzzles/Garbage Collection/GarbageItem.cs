using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageItem : MonoBehaviour
{
    public GarbageBin.GarbageType garbageType;

    private void Start()
    {
        PuzzleManager.instance.AddGarbageItem(this.gameObject);
    }
}
