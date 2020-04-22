﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedHole : MonoBehaviour
{
    private MeshFilter filter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;
    private GameObject parent;

    public Mesh[] treeMeshes;
    public Material[] treeMaterails;
    public bool hasBeenPlanted = false;


    private Mesh saplingMesh;
    private Material saplingMaterial;

    private void Awake()//Script setup and references
    {
        parent = transform.parent.gameObject;
        filter = parent.GetComponent<MeshFilter>();
        meshRenderer = parent.GetComponent<MeshRenderer>();
        meshCollider = parent.GetComponent<MeshCollider>();
        PuzzleManager.instance.AddHole(this.gameObject);
    }

    private void OnCollisionEnter(Collision col)//Triggered when the seed is dropped into the hole
    {
        if (col.gameObject.name == "Seed")
        {
            //Debug.Log("Seed was dropped into the hole!");
            hasBeenPlanted = true;
            PuzzleManager.instance.CheckTreePuzzleComplete();
            MoveSeed(col.gameObject);
            PlantSapling();
            Invoke("StartGrowth", 5f);//Starting tree growth after 5 seconds
            this.gameObject.SetActive(false);
        }
    }
    private void StartGrowth()//Turning on the grow tree script to start the growth stage
    {
        parent.GetComponent<GrowTree>().enabled = true;
    }

    private void MoveSeed(GameObject seed)
    {
        Rigidbody rb = seed.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        seed.transform.position = new Vector3(0, -100, 0);//Moving the seed below the game world and holding it in space

    }

    private void PickTree()//Picking a random tree out of a array
    {
        int index = Random.Range(0, treeMeshes.Length - 1);
        saplingMesh = treeMeshes[index];
        saplingMaterial = treeMaterails[0];
    }

    private void PlantSapling()// changing the mesh and materials of the hole object to the sapling object
    {
        PickTree();
        filter.sharedMesh = saplingMesh;
        meshRenderer.sharedMaterial = saplingMaterial;
        float scale = Random.Range(0.3f, 0.8f);
        parent.transform.localScale = new Vector3(scale, scale, scale);
        parent.transform.Rotate(new Vector3(0, Random.Range(0, 360), 0));
        meshCollider.sharedMesh = saplingMesh;

    }
}
