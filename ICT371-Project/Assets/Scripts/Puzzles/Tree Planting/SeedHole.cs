using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedHole : MonoBehaviour
{
    private void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.name == "Seed")
        {
            Debug.Log("Seed was dropped into the hole!");
        }
        Debug.Log("collision triggered with: " + col.gameObject);
    }
}
