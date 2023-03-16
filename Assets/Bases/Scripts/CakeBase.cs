using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ubiq.Messaging;


public class CakeBase : MonoBehaviour
{
    NetworkContext context;
    public Material[] materials; //[vanilla, chocolate, strawberry]
    public GameObject[] layers;
    public int materialIndex;
    
    public void changeMaterial_send_msg(int x){
        context.SendJson(new Message()
        {
            materialID = x
        });
        changeMaterial(x);
    }

    private void changeMaterial(int x)
    {
        foreach (GameObject layer in layers){
        Renderer renderer = layer.GetComponent<Renderer>();
        renderer.material = materials[x];
        }
    }

    void Start()
    {
        changeMaterial(materialIndex);
        context = NetworkScene.Register(this);
    }

    struct Message
    {
        public int materialID;
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();
        changeMaterial(msg.materialID);
    }

}
