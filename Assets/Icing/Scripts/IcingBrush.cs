using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ubiq.XR;
using Ubiq.Messaging;
using Ubiq.Spawning;

public class IcingBrush : MonoBehaviour, IGraspable, IUseable, INetworkSpawnable
{
    Hand grasped;
    NetworkContext context;
    public NetworkId NetworkId { get; set; }
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
    private List<GameObject> icingObjects;

    // public void Attach(Hand controller)
    // {
    //     Debug.Log($"called: {controller}");
    //     grasped = controller;
    //     Debug.Log(grasped);
    //     owner = true;
    // }
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
    void Start()
    {
        Transform[] allChildTransforms = GetComponentsInChildren<Transform>(includeInactive: false);
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
        icingObjects = new List<GameObject>();
        cake = GameObject.Find("Cake");
    }

    struct Message
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 nib_pos;
        public Quaternion nib_rot;
        public string name;
        public bool isIcing;
    }

    public void ProcessMessage (ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();
        if (msg.name == transform.name)
        {
            if (msg.isIcing)
            {
                placeIcing(msg.nib_pos, msg.nib_rot);
            }
            transform.position = msg.position;
            transform.rotation = msg.rotation;
        }
    }

    private void placeIcing(Vector3 nib_pos, Quaternion nib_rot) // potential TODO: add colour as a parameter (will need to add colours into message and processmessage)
    {
        GameObject sphere = Instantiate(icingTips[icingID], nib_pos, nib_rot);
        sphere.transform.rotation = transform.rotation;
        // sphere.transform.Rotate(90, 0, 0);
        sphere.name = "Icing";
        sphere.tag = "Icing";
        MeshRenderer meshRenderer = sphere.GetComponent<MeshRenderer>();
        if (colour != null)
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
        icingObjects.Add(sphere);
    }

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
                    name = transform.name
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

}
