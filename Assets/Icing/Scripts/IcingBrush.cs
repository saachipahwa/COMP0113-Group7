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
    private Transform nib;
    private Material drawingMaterial;
    private GameObject currentDrawing;
    private GameObject nib_obj;
    private bool owner;
    private bool isTouchingCake = false;
    private bool isUsing;
    private Vector3 prevNibPos;
    private List<GameObject> icingObjects;
    public GameObject[] icingTips; //[sphere, star]
    public NetworkId NetworkId { get; set; }
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    public int icingID;
    
    public GameObject cake;


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
        // if (isTouchingCake)
        // {
        //     // if (prevNibPos != nib.transform.position)
        //     // {
        //         //below line sets rotation of sphere to be the same as nibs
        //     GameObject sphere = NetworkSpawnManager.Find(this).SpawnWithPeerScope(icingTips[icingID]);
        //     sphere.transform.rotation = transform.rotation;
        //     sphere.transform.Rotate(90, 0, 0);
        //     // sphere.name = "Icing";
        //     // sphere.tag = "Cake";
        //     // MeshRenderer meshRenderer = sphere.GetComponent<MeshRenderer>();
        //     // meshRenderer.material.color = Color.red;
        //     sphere.transform.position = nib.transform.position;
        //     sphere.transform.localScale = sphere.transform.localScale * 2f;
        //     var icingObject = sphere.GetComponent<Icing>();
        //     icingObject.owner = true;
        //         // prevNibPos = sphere.transform.position;
        //         // icingObjects.Add(sphere);
        //     // }
        // }
    }

    public void UnUse(Hand controller)
    {
        isUsing = false;
    }

    struct Message
    {
        public Vector3 position;
        public Quaternion rotation;
        public bool isIcing;
    }

    public void ProcessMessage (ReferenceCountedSceneGraphMessage msg)
    {
        var data = msg.FromJson<Message>();
        transform.localPosition = data.position;
        transform.localRotation = data.rotation;
        // isUsing = data.isIcing;

        // Make sure the logic in FixedUpdate doesn't trigger as a result of this message
        // lastPosition = transform.position;
        // lastRotation = transform.rotation;
    }
    private void Awake()
    {
        owner = false;
    }
    void Start()
    {
        nib = transform.Find("PipingTip/Nib"); //just the transform
        nib_obj = GameObject.Find("Nib"); //the object itself
        context = NetworkScene.Register(this);
        prevNibPos = new Vector3(0f, 0f, 0f);
        icingObjects = new List<GameObject>();
        // cake = GameObject.Find("Cake");
    }

    private void FixedUpdate()
    {
        if (owner)
        {
            //send new position or rotation of icing brush to network
            if(lastPosition != transform.position || lastRotation != transform.rotation)
            {
                lastPosition = transform.position;
                lastRotation = transform.rotation;
                context.SendJson(new Message()
                {
                    position = transform.localPosition,
                    rotation = transform.localRotation,
                    // isIcing = isUsing
                });
            }
        }
        if (isUsing)
        {
            if (isTouchingCake)
            {
                if (prevNibPos != nib.transform.position)
                {
                    //below line sets rotation of sphere to be the same as nibs
                    GameObject sphere = NetworkSpawnManager.Find(this).SpawnWithPeerScope(icingTips[icingID]);
                    // GameObject sphere = Instantiate(icingTips[icingID], nib.transform.position, nib.transform.rotation);
                    var icingObject = sphere.GetComponent<Icing>();
                    sphere.transform.rotation = transform.rotation;
                    sphere.transform.Rotate(90, 0, 0);
                    // sphere.name = "Icing";
                    // sphere.tag = "Cake";
                    // MeshRenderer meshRenderer = sphere.GetComponent<MeshRenderer>();
                    // meshRenderer.material.color = Color.red;
                    sphere.transform.position = nib.transform.position;
                    sphere.transform.localScale = sphere.transform.localScale * 2f;
                    // icingObject.owner = true;
                    // sphere.transform.parent = cake.transform;
                    prevNibPos = sphere.transform.position;
                    icingObjects.Add(sphere);
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
