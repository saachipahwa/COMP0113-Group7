using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CakeBase : MonoBehaviour
{
    public Material[] materials; //[vanilla, chocolate, strawberry]
    public GameObject[] layers;
    public int materialIndex;
    
    public void changeMaterial(int x){
        foreach (GameObject layer in layers){
            Renderer renderer = layer.GetComponent<Renderer>();
            layer.GetComponent<MeshRenderer> ().material = materials[x];
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        changeMaterial(materialIndex);
    }

    // Update is called once per frame
    // void Update()
    // {
    //     changeMaterial(materialIndex);
    // }
}
