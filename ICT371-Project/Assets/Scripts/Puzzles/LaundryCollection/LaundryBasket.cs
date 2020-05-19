using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaundryBasket : MonoBehaviour
{
    public Mesh[] basketMeshes;

    private MeshFilter meshFilter;
    private Rigidbody rb;
    private int percentageFull = 0;

    public bool isFull = false;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        rb = GetComponent<Rigidbody>();
    }


    private void OnCollisionEnter(Collision other) 
    {
        if(other.gameObject.GetComponent<LaundryItem>())
        {
            percentageFull += 20;
            rb.mass += 20; // make the basket have more mass as theres more items in the basket
            UpdateBasketMesh();
            Destroy(other.gameObject);
        }
    }

    private void UpdateBasketMesh()
    {//Check what percentage the basket is full and display correct mesh
        if(percentageFull == 0)
        {
            //Somethings wrong, display empty basket
            meshFilter.mesh = basketMeshes[0];
        }
        else if (percentageFull == 20)
        {
            meshFilter.mesh = basketMeshes[1];
        }
        else if(percentageFull == 40)
        {
            meshFilter.mesh = basketMeshes[2];
        }
        else if(percentageFull == 60)
        {
            meshFilter.mesh = basketMeshes[3];
        }
        else if (percentageFull == 80)
        {
            meshFilter.mesh = basketMeshes[4];
        }
        else if (percentageFull >= 100)
        {
            meshFilter.mesh = basketMeshes[5];
            CompletePuzzle();
        }
    }

    private void CompletePuzzle()
    {
        //Debug.Log("All the laundry has been collected");
        PuzzleManager.instance.SetLaundryCollectionComplete();
        isFull = true;
    }
}
