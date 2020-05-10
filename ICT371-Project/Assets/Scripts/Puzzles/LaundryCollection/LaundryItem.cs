using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaundryItem : MonoBehaviour
{
    //This is a id script used to ensure a laundry item is dropped in the basket
    
    private void Start()
    {
        PuzzleManager.instance.AddLaundryItem(this.gameObject);
    }
}
