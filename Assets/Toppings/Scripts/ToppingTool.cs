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
        var topping_script = spawnedTopping.GetComponent<Topping>();
        topping_script.owner = true;
    }


    public struct Message
    {
        public Vector3 position;
        public Quaternion rotation;
        public string name;
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
            context.SendJson(new Message()
            {
                position = transform.localPosition,
                rotation = transform.localRotation,
                name = transform.name
            });
        }
    }
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();


        if (msg.name == transform.name)
        {
            transform.localPosition = msg.position;
            transform.localRotation = msg.rotation;
        }
    }

}

