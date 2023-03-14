using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ubiq.XR;
using Ubiq.Messaging;

public class Icing : MonoBehaviour
{
    NetworkContext context;
    public NetworkId NetworkId { get; set; }
    public bool owner;
    private bool msgSent = false;
    
    // Start is called before the first frame update
    void Start()
    {
        context = NetworkScene.Register(this);
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
                    rotation = transform.localRotation,
                    scale = transform.localScale
                });
                msgSent = true;
            }
        }
    }

    private struct Message
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        // Parse the message
        var m = message.FromJson<Message>();
        msgSent = true;
        // Use the message to update the Component
        transform.localPosition = m.position;
        transform.localRotation = m.rotation;
        transform.localScale = m.scale;
    }
}
