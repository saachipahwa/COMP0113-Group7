using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ubiq.Messaging;

public class Topping : MonoBehaviour
{
    Transform lastPlacement;
    NetworkContext context;
    void Start()
    {
        context = NetworkScene.Register(this);
    }

    void Update()
    {
        if (lastPlacement != transform)
        {
            lastPlacement = transform;
            context.SendJson(new Message()
            {
                msgTransform = transform
            });
        }
    }

    public struct Message
    {
        public Transform msgTransform;
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var m = message.FromJson<Message>();

        transform.position = m.msgTransform.position;
        transform.rotation = m.msgTransform.rotation;

        lastPlacement = transform;
    }
}
