using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ubiq.XR;
using Ubiq.Messaging;

public class Icing : MonoBehaviour
{
    NetworkContext context;
    public bool owner;

    
    // Start is called before the first frame update
    void Start()
    {
        context = NetworkScene.Register(this);
    }

    void Update()
    {
        if (owner){
            context.SendJson(new Message(transform));
        }
    }

    private struct Message
    {
        // public Transform transform;
        public TransformMessage transform;

        public Message(Transform transform)
        {
            // this.transform = transform;
            this.transform = new TransformMessage(transform);
        }

    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        // Parse the message
        var m = message.FromJson<Message>();

        // Use the message to update the Component
        transform.localPosition = m.transform.position;
        transform.localRotation = m.transform.rotation;
    }
}
