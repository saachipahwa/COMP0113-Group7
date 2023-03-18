using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using Ubiq.Spawning;
using Ubiq.XR;
using UnityEngine;

// Script is attached to the Candle tool, Flower tool and Strawbrery tool (and any other topping tools) game objects
public class ToppingTool : MonoBehaviour, IGraspable, IUseable, INetworkSpawnable
{
    Hand attached;
    public NetworkId NetworkId { get; set; }
    private bool owner;
    private NetworkContext context;
    public GameObject topping;
    private bool isPlacing = false;
    public GameObject indicator;
    public Material indicator_material_owner;

    public void Grasp(Hand controller)
    // keep track of when user is holding the topping tool
    {
        if (owner == true) // only owners can grab the tool
        {
            attached = controller;
        }
        else
        {
            Release(controller);
        }
    }

    // keep track of when user stops holding the topping tool
    public void Release(Hand controller)
    {
        attached = null;
    }
    
    void Start()
    {
        context = NetworkScene.Register(this);
    }

    public void UnUse(Hand controller)
    {
    }

    // keeps track of when user is using topping tool
    public void Use(Hand controller)
    {
        isPlacing = true;
    }

    // spawns topping at specified position (pos) and rotation (rot)
    public void placeTopping(Vector3 pos, Quaternion rot)
    {
        GameObject spawnedTopping = Instantiate(topping, pos, rot);
        isPlacing = false;
    }

    public struct Message
    /* message contains
        position of topping
        rotation of topping
        what topping it is (name)
        whether user used tool (placing)
    */
    {
        public Vector3 position;
        public Quaternion rotation;
        public string name;
        public bool placing;
    }

    // called once per frame
    void Update()
    {
        if (attached) // tool follows hand if grabbing
        {
            transform.position = attached.transform.position;
            transform.rotation = attached.transform.rotation;
        }

        if (owner) // send message to other players updating them of topping tool's behaviour
        {
            context.SendJson(new Message()
            {
                position = transform.position,
                rotation = transform.rotation,
                name = transform.name,
                placing = isPlacing
            });
        }

        if (isPlacing) // place a topping
        {
            placeTopping(transform.position, transform.rotation);
        }
    }
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();

        if (msg.name == transform.name)
        {
            if (msg.placing)
            {
                placeTopping(msg.position, msg.rotation);
            }
            transform.position = msg.position;
            transform.rotation = msg.rotation;
        }
    }

    // sets 'owner' of tool
    // if owner, make band green to indicate it's your tool
    public void setOwner(bool isOwner)
    {
        owner = isOwner;
        if (owner)
        {
            Renderer renderer = indicator.GetComponent<Renderer>();
            renderer.material = indicator_material_owner;
        }
    }
}

