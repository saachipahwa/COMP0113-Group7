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
    private bool isPlacing = false;

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
        isPlacing = true;
    }

    public void placeTopping(Vector3 pos, Quaternion rot)
    {
        GameObject spawnedTopping = Instantiate(topping, pos, rot);
        isPlacing = false;
    }


    public struct Message
    {
        public Vector3 position;
        public Quaternion rotation;
        public string name;
        public bool placing;
    }

    void Update()
    {
        if (attached)
        {
            transform.position = attached.transform.position;
            transform.rotation = attached.transform.rotation;
        }

        if (owner)
        {
            context.SendJson(new Message()
            {
                position = transform.position,
                rotation = transform.rotation,
                name = transform.name,
                placing = isPlacing
            });
        }

        if (isPlacing)
        {
            placeTopping(transform.position, transform.rotation);
        }
    }
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();

        if (msg.name == transform.name)
        {
            if (msg.placing)
            {
                placeTopping(msg.position, msg.rotation);
            }
            transform.position = msg.position;
            transform.rotation = msg.rotation;
        }
    }
}

