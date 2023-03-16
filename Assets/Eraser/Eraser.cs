using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ubiq.XR;
using Ubiq.Messaging;

//Script is attached to eraser prefab
public class Eraser : MonoBehaviour, IGraspable, IUseable
{   
    Hand grasped;
    NetworkContext context;
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    private bool owner; 
    private bool isUsing = false;
    public GameObject indicator;

    public void Grasp(Hand controller)
    //used to keep track of when user is holding the eraser
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
    //used to keep track of when user stops holding the eraser
    {
        grasped = null;
    }

    public void Use(Hand controller) 
    //used to keep track if user is using the eraser
    {
        isUsing = true;
    }

    public void UnUse(Hand controller)
    //used to keep track if user stops using the eraser
    {
        isUsing = false;
    }

    void Start()
    //start function initialises the networking
    {
        context = NetworkScene.Register(this);
    }

    private struct Message
    //message contains position and rotation of the eraser, 
    //as well as that it is an eraser, and if it is currently being used.
    {
        public Vector3 position;
        public Quaternion rotation;
        public string name;
        public bool isErasing;
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    //sends message to other players
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
    //when eraser enters a topping or icing blob, it destroys it
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
    //once per frame, owner sends message updating other players of the eraser's status
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

    public void setOwner(bool isOwner)
    //checks if user is owner of tool
    //akes band visible around eraser if so
    {
        owner = isOwner;
        indicator.SetActive(isOwner);
    }
}
