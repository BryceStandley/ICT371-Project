using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowTree : MonoBehaviour
{
    public float growRate = 1.001f;
    private void Update()
    { 
        if(transform.localScale.x < 1.5f)
        {
            transform.localScale *= growRate;
        }
        else
        {
            this.enabled = false;
        }
    }
}
