using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WashedBasket : MonoBehaviour
{
    public Mesh basketFullMesh;
    public Mesh basketEmptyMesh;
    public bool hasClothes = false;

    public int tempWashedAt = 0;

    private MeshFilter mesh;

    private void Start()
    {
        mesh = GetComponent<MeshFilter>();
    }
    public void SetFullBasket()
    {
        mesh.sharedMesh = basketFullMesh;
        hasClothes = true;
        GetComponent<ObjectInformation>().itemStats = "Full of my clean Clothes.";
    }

    public void SetEmptyBasket()
    {
        mesh.sharedMesh = basketEmptyMesh;
        hasClothes = false;
        GetComponent<ObjectInformation>().itemStats = "Ready for me to put my clean clothes in.";
    }
}
