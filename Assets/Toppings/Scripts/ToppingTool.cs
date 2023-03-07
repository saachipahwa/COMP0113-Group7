using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using Ubiq.Spawning;
using Ubiq.XR;
using UnityEngine;

public class ToppingTool : MonoBehaviour, IGraspable, IUseable, INetworkSpawnable
{
    Hand attached;
    private Rigidbody body;
    public NetworkId NetworkId { get; set; }
    public bool owner;
    private NetworkContext context;
    public GameObject topping;

    public void Grasp(Hand controller)
    {
        owner = true;
        attached = controller;
    }

    public void Release(Hand controller)
    {
        owner = false;
        attached = null;
    }
    private void Awake()
    {
        // body = GetComponent<Rigidbody>();
        owner = false;
    }
    
    void Start()
    {
        context = NetworkScene.Register(this);
    }
    // public void Attach(Hand hand)
    // {
    //     attached = hand;
    //     owner = true;
    // }
    public void UnUse(Hand controller)
    {
    }

    public void Use(Hand controller)
    {
        GameObject spawnedTopping = NetworkSpawnManager.Find(this).SpawnWithPeerScope(topping);
        spawnedTopping.transform.position = transform.position;
        spawnedTopping.transform.rotation = transform.rotation;
        // var topping_script = spawnedTopping.GetComponent<Topping>();
        // topping_script.owner = true;
    }


    public struct Message
    {
        public TransformMessage transform;

        public Message(Transform transform)
        {
            this.transform = new TransformMessage(transform); 
        }
    }

    void FixedUpdate()
    {
        if (attached)
        {
            transform.position = attached.transform.position;
            transform.rotation = attached.transform.rotation;
        }

        if(owner)
        {
            context.SendJson(new Message(transform));
        }
    }
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
        {
            var msg = message.FromJson<Message>();
            transform.localPosition = msg.transform.position;
            transform.localRotation = msg.transform.rotation;
        }

}

