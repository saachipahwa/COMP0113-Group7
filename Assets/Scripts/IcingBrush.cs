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

    struct Message
    {
        public Vector3 position;
        public Quaternion rotation;
        public bool isDrawing;

        public Message(Transform transform, bool isDrawing)
        {
            this.position = transform.position;
            this.rotation = transform.rotation;
            this.isDrawing = isDrawing;
        }
    }

    public void ProcessMessage (ReferenceCountedSceneGraphMessage msg)
    {
        var data = msg.FromJson<Message>();
        transform.position = data.position;
        transform.rotation = data.rotation;

        // Make sure the logic in FixedUpdate doesn't trigger as a result of this message
        lastPosition = transform.position;
        lastRotation = transform.rotation;

    }
    void Start()
    {
        nib = transform.Find("PipingTip/Nib"); //just the transform
        nib_obj = GameObject.Find("Nib"); //the object itself
        context = NetworkScene.Register(this);
        var shader = Shader.Find("Particles/Standard Surface");
        drawingMaterial = new Material(shader);
        drawingMaterial.SetColor("_Color", Color.red); // sets colour, TODO: add to menu
        prevNibPos = new Vector3(0f, 0f, 0f);
        icingObjects = new List<GameObject>();
    }

    private void FixedUpdate()
    {
        if (owner)
        {
            context.SendJson(new Message(transform, isDrawing:currentDrawing));

            //send new position of icing brush to network
            if(lastPosition != transform.position)
            {
                lastPosition = transform.position;
                context.SendJson(new Message()
                {
                    position = transform.position
                });
            }

            //send new rotation of icing brush to network
            if(lastRotation != transform.rotation)
            {
                lastRotation = transform.rotation;
                context.SendJson(new Message()
                {
                    rotation = transform.rotation
                });
            }
        }

        if (isUsing)
        {
            if (isTouchingCake)
            {
                if (prevNibPos != nib.transform.position)
                {
                    // GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    //below line sets rotation of sphere to be the same as nibs
                    GameObject sphere = NetworkSpawnManager.Find(this).SpawnWithPeerScope(icingTips[1]);
                    // GameObject sphere = Instantiate(icingTips[1], nib.transform.position, nib.transform.rotation); 
                    sphere.transform.Rotate(90, 0, 0);
                    sphere.name = "Icing";
                    sphere.tag = "Cake";
                    MeshRenderer meshRenderer = sphere.GetComponent<MeshRenderer>();
                    meshRenderer.material.color = Color.red;
                    sphere.transform.position = nib.transform.position;
                    // sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f); //for circles
                    sphere.transform.localScale = new Vector3(10f, 10f, 10f); //for stars
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
