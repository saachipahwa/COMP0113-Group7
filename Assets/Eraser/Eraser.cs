using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ubiq.XR;
using Ubiq.Messaging;

public class Eraser : MonoBehaviour, IGraspable, IUseable
{   
    Hand grasped;
    NetworkContext context;
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    public bool owner; 
    private bool isUsing = false;
    private Rigidbody rigidBody;

    public void Grasp(Hand controller)
    {
        if (owner == true)
        {
            grasped = controller;
        }
        else
        {
            Release(controller);
        }
    }

    public void Release(Hand controller)
    {
        grasped = null;
    }

    public void Use(Hand controller) 
    {
        isUsing = true;
    }

    public void UnUse(Hand controller)
    {
        isUsing = false;
    }

    void Start()
    {
        context = NetworkScene.Register(this);
        rigidBody = GetComponent<Rigidbody>();
    }

    private struct Message
    {
        public Vector3 position;
        public Quaternion rotation;
        public string name;
        public bool isErasing;
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        // Parse the message
        var msg = message.FromJson<Message>();

        if (msg.name == transform.name)
        {
            isUsing = msg.isErasing;
            
            // Use the message to update the Component
            transform.position = msg.position;
            transform.rotation = msg.rotation;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isUsing)
        {
            if(other.gameObject.tag == "Topping"|| other.gameObject.tag == "Icing")
            {
                Destroy(other.gameObject);
            }
        } 
    }

    void Update()
    {
        if (grasped)
        {
            transform.position = grasped.transform.position;
            transform.rotation = grasped.transform.rotation;
        }
        if (owner)
        {
            context.SendJson(new Message()
            {
                position = transform.position,
                rotation = transform.rotation,
                name = transform.name,
                isErasing = isUsing
            });
        }
    
    }
}
