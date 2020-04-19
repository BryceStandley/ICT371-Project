using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedHole : MonoBehaviour
{
    private MeshFilter filter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;
    private GameObject parent;

    public Mesh saplingMesh;
    public Material saplingMaterial;

    private void Awake()
    {
        parent = transform.parent.gameObject;
        filter = parent.GetComponent<MeshFilter>();
        meshRenderer = parent.GetComponent<MeshRenderer>();
        meshCollider = parent.GetComponent<MeshCollider>();
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "Seed")
        {
            Debug.Log("Seed was dropped into the hole!");
            MoveSeed(col.gameObject);
            PlantSapling();
            Invoke("StartGrowth", 5f);
            this.gameObject.SetActive(false);
        }
    }
    private void StartGrowth()
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

    private void PlantSapling()// changing the mesh and materials of the hole object to the sapling object
    {
        filter.sharedMesh = saplingMesh;
        meshRenderer.sharedMaterial = saplingMaterial;
        float scale = Random.Range(0.3f, 0.8f);
        parent.transform.localScale = new Vector3(scale, scale, scale);
        parent.transform.Rotate(new Vector3(0, Random.Range(0, 360), 0));
        meshCollider.sharedMesh = saplingMesh;

    }
}
