using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ubiq.XR;
using Ubiq.Messaging;

public class Eraser : MonoBehaviour, IGraspable, IUseable
{   
    // Script is attached to eraser prefab
    Hand grasped;
    NetworkContext context;
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    private bool owner; 
    private bool isUsing = false;
    public GameObject indicator;
    public Material indicator_material_owner;


    // keep track of when user is holding the eraser
    public void Grasp(Hand controller)
    {
        if (owner == true) // only owners can grab the tool
        {
            grasped = controller;
        }
        else
        {
            Release(controller);
        }
    }

    // keep track of when user stops holding the eraser
    public void Release(Hand controller)
    {
        grasped = null;
    }

    // keep track if user is using the eraser
    public void Use(Hand controller) 
    {
        isUsing = true;
    }

    // keep track if user stops using the eraser
    public void UnUse(Hand controller)
    {
        isUsing = false;
    }

    void Start()
    {
        context = NetworkScene.Register(this);
    }

    private struct Message
    {
        // message contains position and rotation of the eraser, 
        // as well as the name for networking purposes, and if it is currently being used
        public Vector3 position;
        public Quaternion rotation;
        public string name;
        public bool isErasing;
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();

        if (msg.name == transform.name)
        {
            isUsing = msg.isErasing;
            
            transform.position = msg.position;
            transform.rotation = msg.rotation;
        }
    }

    // when eraser colliders with a topping or icing blob, it destroys it
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

    // once per frame, owner sends message updating other players of the eraser's status, also keep tool in hand if grasped
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

    public void setOwner(bool isOwner)
    // sets 'owner' of eraser
    // if owner, make band green to indicate it's your tool
    {
        owner = isOwner;
        if (owner)
        {
            Renderer renderer = indicator.GetComponent<Renderer>();
            renderer.material = indicator_material_owner;
        }
    }
}
