using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ubiq.Messaging;
using Ubiq.Spawning;


public class Topping : MonoBehaviour, INetworkSpawnable
{
    Transform lastPlacement;
    NetworkContext context;
    public Vector3 lastPosition;
    public Quaternion lastRotation;
    public NetworkId NetworkId { get; set; }
    private Rigidbody rigidBody;
    // public bool owner;

    void Start()
    {
        context = NetworkScene.Register(this);
        rigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (lastPlacement == null) 
        {
            lastPosition = transform.localPosition;
            lastRotation = transform.localRotation;
            context.SendJson(new Message()
            {
                position = transform.localPosition,
                rotation = transform.localRotation
            });
        }
        else if (lastPlacement.position != transform.localPosition)
        {
            if (lastPlacement.rotation != transform.localRotation)
            {
                lastPosition = transform.localPosition;
                lastRotation = transform.localRotation;
                context.SendJson(new Message()
                {
                    position = transform.localPosition,
                    rotation = transform.localRotation
                });
            }
        }
    }

    public struct Message
    {
        // public Transform msgTransform;
        public Vector3 position;
        public Quaternion rotation;
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var m = message.FromJson<Message>();

        transform.localPosition = m.position;
        transform.localRotation = m.rotation;

        lastPosition = transform.localPosition;
        lastRotation = transform.localRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Cake")
        {
            rigidBody.isKinematic = true;
        }
    }
}
