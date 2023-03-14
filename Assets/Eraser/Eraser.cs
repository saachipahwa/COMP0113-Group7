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
    private bool owner; 
    private bool isUsing = false;
    private Rigidbody rigidBody;

    public void Grasp(Hand controller)
    {
        owner = true;
        grasped = controller;
    }

    public void Release(Hand controller)
    {
        owner = false;
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

    private void Awake()
    {
        owner = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        context = NetworkScene.Register(this);
        rigidBody = GetComponent<Rigidbody>();
    }

    private struct Message
    {
        public Vector3 position;
        public Quaternion rotation;
        public bool isErasing;
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        // Parse the message
        var msg = message.FromJson<Message>();
        isUsing = msg.isErasing;
        
        // Use the message to update the Component
        transform.position = msg.position;
        transform.rotation = msg.rotation;

        // Make sure the logic in Update doesn't trigger as a result of this message
        lastPosition = transform.position;
        lastRotation = transform.rotation;    
    }

    void OnTriggerEnter(Collider other)
    {
        // if (isUsing)
        // {
            if(other.gameObject.tag == "Topping"|| other.gameObject.tag == "Icing")
            {
                Destroy(other.gameObject);
            }
        // } 
    }

    // Update is called once per frame
    void Update()
    {
        if (owner)
        {
            lastPosition = transform.position;
            lastRotation = transform.rotation;
            context.SendJson(new Message()
            {
                position = transform.localPosition,
                rotation = transform.localRotation,
                isErasing = isUsing
            });
        }
    
        if (grasped)
        {
            transform.position = grasped.transform.position;
            transform.rotation = grasped.transform.rotation;
        }
    }
}
