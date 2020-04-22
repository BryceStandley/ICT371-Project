using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowTree : MonoBehaviour
{
    public int growTime = 30;

    private float treeScale = 0.3f;

    private void Start()
    {
        treeScale = transform.localScale.x;
    }

    private float t = 0;
    private void Update()//Using the time between frames to calculate how much to grow the tree over time
    {
        t += Time.deltaTime / growTime;
        float i = Mathf.Lerp(treeScale, 1.5f, t);
        //Debug.Log(t + " :: " + i);

        if(transform.localScale.x < 1.5f)
        {
            transform.localScale = new Vector3(i, i, i);
        }
        else
        {
            this.enabled = false;
        }
    }
}
