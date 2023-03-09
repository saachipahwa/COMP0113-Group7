using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ubiq.XR;
using Ubiq.Messaging;

public class Eraser : MonoBehaviour, IGraspable, IUseable
{   
    Hand grasped;
    NetworkContext context;
    Vector3 lastPosition;
    
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
    }


    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        // Parse the message
        var msg = message.FromJson<Message>();
      
        // Use the message to update the Component
        transform.position = msg.position;

        // Make sure the logic in Update doesn't trigger as a result of this message
        lastPosition = transform.position;
    }

    void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        Destroy(collision.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (owner)
        {
            if(lastPosition != transform.position)
            {
                lastPosition = transform.position;
                 context.SendJson(new Message()
                {
                    position = transform.localPosition,
                });

            }
        }

    }
}
