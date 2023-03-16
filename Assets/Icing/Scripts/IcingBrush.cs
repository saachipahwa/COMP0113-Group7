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
    //start function finds nib and cake objects as well as initialising networking
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
    //message includes position and rotation of cake as well as nib,
    //the name tells the other players this message is about the icing brush
    //message also includes the colour in RGB format
    {
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
    //sends message
    {
        var msg = message.FromJson<Message>();
        if (msg.name == transform.name)
        {
            if (msg.isIcing)
            {
                Color icingColour = new Color(msg.c_red, msg.c_green, msg.c_blue);
                placeIcing(msg.nib_pos, msg.nib_rot, icingColour);
            }
            transform.position = msg.position;
            transform.rotation = msg.rotation;
        }
    }

    private void placeIcing(Vector3 nib_pos, Quaternion nib_rot, Color? colour_param = null)
    //places icing blob at the same position and rotation as nib
    //sets colour using mesh renderer
    {
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
        sphere.transform.parent = cake.transform;
    }

    private void FixedUpdate()
    //owner sends message if position or rotation changed
    //places icing at position and rotation of nib
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
    //checks if user is owner of tool
    //makes band visible around icing brush if so
    {
        owner = isOwner;
        if (owner)
        {
            Renderer renderer = indicator.GetComponent<Renderer>();
            renderer.material = indicator_material_owner;
        }
    }
}
