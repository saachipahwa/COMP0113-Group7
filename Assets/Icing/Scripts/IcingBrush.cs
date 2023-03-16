using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ubiq.XR;
using Ubiq.Messaging;
using Ubiq.Spawning;

//Script is attached to Icing brush (game object is just called "Icing")
public class IcingBrush : MonoBehaviour, IGraspable, IUseable
{
    Hand grasped;
    NetworkContext context;
    public GameObject[] icingTips; //[sphere, star]
    public int icingID;
    public Color colour;
    private GameObject cake;
    private Transform nib;
    private GameObject nib_obj;
    private bool owner;
    private bool isTouchingCake = false;
    private bool isUsing = false;
    private Vector3 prevNibPos;
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    public GameObject indicator;
    public Material indicator_material_owner;


    // keep track of when user is holding the icing bag
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

    // keep track of when user stops holding the icing bag
    public void Release(Hand controller)
    {
        grasped = null;
    }

    // keep track if user is using the icing bag
    public void Use(Hand controller) 
    {
        isUsing = true;
    }

    // keep track if user stops using the icing bag
    public void UnUse(Hand controller)
    {
        isUsing = false;
    }

    void Start()
    {
        Transform[] allChildTransforms = GetComponentsInChildren<Transform>(includeInactive: true);
        foreach (Transform child in allChildTransforms)
        {
            if (child.name == "Nib")
            {
                nib = child;
                break;
            }
        }

        nib_obj = nib.gameObject;
        context = NetworkScene.Register(this);
        prevNibPos = new Vector3(0f, 0f, 0f);
        cake = GameObject.Find("Cake");
    }

    struct Message
    {
        // message includes position and rotation of icing brush as well as the nib,
        // the name tells the other players this message is about the icing brush
        // message also includes the colour in RGB format, and whether the owner is currently using the icing bag or not
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 nib_pos;
        public Quaternion nib_rot;
        public string name;
        public bool isIcing;
        public float c_red;
        public float c_green;
        public float c_blue;
    }

    public void ProcessMessage (ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();
        if (msg.name == transform.name)
        {
            if (msg.isIcing)
            {
                Color icingColour = new Color(msg.c_red, msg.c_green, msg.c_blue); // create colour from RGB values
                placeIcing(msg.nib_pos, msg.nib_rot, icingColour);
            }
            transform.position = msg.position;
            transform.rotation = msg.rotation;
        }
    }

    private void placeIcing(Vector3 nib_pos, Quaternion nib_rot, Color? colour_param = null)
    {
        /*
        places icing blob at the same position and rotation as nib, sets the colour, and adjusts the size to be appropriate

        nib_pos: position of the nib 
        nib_rot: rotation of the nib
        colour_param: change colour to the colour_param if exists, otherwise use colour variable
        */
        GameObject sphere = Instantiate(icingTips[icingID], nib_pos, nib_rot);
        sphere.transform.rotation = transform.rotation;
        sphere.name = "Icing";
        sphere.tag = "Icing";
        MeshRenderer meshRenderer = sphere.GetComponent<MeshRenderer>();
        if (colour_param != null)
        {
            meshRenderer.material.color = (Color)colour_param;
        }
        else if (colour != null)
        {
            meshRenderer.material.color = colour;
        }
        else
        {
            meshRenderer.material.color = Color.white;
        }
        sphere.transform.localScale = sphere.transform.localScale * 2f;
        prevNibPos = sphere.transform.position;
        sphere.transform.parent = cake.transform; // icing becomes a child of the cake object
    }

    // owner sends message if position or rotation changed
    // places icing at position and rotation of nib if touching cake
    private void FixedUpdate()
    {
        if (owner)
        {
            if(lastPosition != transform.position || lastRotation != transform.rotation)
            {
                lastPosition = transform.position;
                lastRotation = transform.rotation;
                context.SendJson(new Message()
                {
                    position = transform.localPosition,
                    rotation = transform.localRotation,
                    isIcing = isUsing,
                    nib_pos = nib.transform.position,
                    nib_rot = nib.transform.rotation,
                    name = transform.name,
                    c_red = colour.r,
                    c_green = colour.g,
                    c_blue = colour.b                    
                });
            }
        }
        if (isUsing)
        {
            if (isTouchingCake)
            {
                if (prevNibPos != nib.transform.position)
                {
                    placeIcing(nib.transform.position, nib.transform.rotation);
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (grasped)
        {
            transform.position = grasped.transform.position;
            transform.rotation = grasped.transform.rotation;
        }

        isTouchingCake = nib_obj.GetComponent<NibOnCake>().isTouching;

        if (!isTouchingCake)
        {
            UnUse(grasped);
        }
    }

    public void setOwner(bool isOwner)
    // sets 'owner' of tool
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
