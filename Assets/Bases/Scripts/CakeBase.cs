using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ubiq.Messaging;

//This script is attached to all cake prefabs.
public class CakeBase : MonoBehaviour
{
    NetworkContext context; 
    public Material[] materials; //[vanilla, chocolate, strawberry]
    public GameObject[] layers;
    public int materialIndex;
    
    public void changeMaterial_send_msg(int x)
    //sends message to change material for other users
    {
        context.SendJson(new Message()
        {
            materialID = x
        });
        changeMaterial(x);
    }

    private void changeMaterial(int x)
    //changes material from mwnu
    {
        foreach (GameObject layer in layers){
        Renderer renderer = layer.GetComponent<Renderer>();
        renderer.material = materials[x];
        }
    }

    void Start()
    //start function changes material to default 
    {
        changeMaterial(materialIndex);
        context = NetworkScene.Register(this);
    }

    struct Message
    //networking contains the material
    {
        public int materialID;
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    //sends message
    {
        var msg = message.FromJson<Message>();
        changeMaterial(msg.materialID);
    }

}
