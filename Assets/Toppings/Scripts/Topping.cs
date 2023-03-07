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
    public bool owner = false;
    private bool msgSent = false;

    void Start()
    {
        context = NetworkScene.Register(this);
        rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!msgSent)
        {
            if (owner)
            {
                context.SendJson(new Message()
                {
                    position = transform.localPosition,
                    rotation = transform.localRotation
                });
                msgSent = true;
            }
        }
    }

    public struct Message
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var m = message.FromJson<Message>();
        msgSent = true;
        transform.localPosition = m.position;
        transform.localRotation = m.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Cake")
        {
            rigidBody.isKinematic = true;
        }
    }
}
