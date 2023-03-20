using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using Ubiq.Spawning;
using Ubiq.XR;
using UnityEngine;

public class SprinklesTool : MonoBehaviour, IGraspable, IUseable
{
    private int NUMBER_OF_SPRINKLES;
    private Hand hand;
    private Vector3 previous;
    private Vector3 velocity;
    public GameObject sprinklePrefab;
    NetworkContext context;
    private bool isPlacing = false;
    private bool owner = true; // TODO: REMOVE = true
    public GameObject indicator;
    public Material indicator_material_owner;
    private Color[] colors;

    void Start()
    {
        colors = new Color[6];

        colors[0] = Color.red; // red
        colors[1] = Color.white; // white
        colors[2] = Color.green; // green
        colors[3] = Color.blue; // blue
        colors[4] = Color.yellow; // yellow
        colors[5] = Color.magenta; // magenta

        context = NetworkScene.Register(this);
        NUMBER_OF_SPRINKLES = sprinklePrefab.GetComponentsInChildren<Transform>().Length - 1;
    }   

    public void Grasp(Hand controller)
    {
        if (owner == true)
        {
            hand = controller;
        }
        else
        {
            Release(controller);
        }
    }

    public void Release(Hand controller)
    {
        hand = null;
    }
    
    public void UnUse(Hand controller)
    {

    }
    
    public void Use(Hand controller)
    {
        /*
        GameObject spawnedSprinkle = Instantiate(sprinklePrefab, transform.position, transform.rotation);

        spawnedSprinkle.GetComponent<SprinkleController>().setVelocity(velocity * 2); 
        */
        isPlacing = true;
    }

    public void throwSprinkle(Vector3 handPos, Quaternion handRot, Vector3[] sprinklesPos, int[] colour_index, Vector3 velo)
    {
        GameObject spawnedSprinkles = Instantiate(sprinklePrefab, handPos, handRot);
        // spawnedSprinkles.GetComponent<SprinkleController>().setVelocity(velocity * 2);
        Transform[] sprinkle_transforms = spawnedSprinkles.GetComponentsInChildren<Transform>();
        for (int i = 1; i < sprinkle_transforms.Length; i++)
        {
            sprinkle_transforms[i].transform.position = sprinklesPos[i-1];
            // sprinkle_transforms[i].transform.rotation = sprinklesRot[i-1];
            sprinkle_transforms[i].GetComponent<Renderer>().material.color = colors[colour_index[i-1]];
            Rigidbody r = sprinkle_transforms[i].GetComponent<Rigidbody>();
            r.isKinematic = false;
            r.velocity = velo;
        }
        isPlacing = false;
    }
    
    public struct Message
    {
        /* message contains
            position of tool
            rotation of tool
            name of tool
            whether user used tool (placing)
            positions of sprinkles to spawn (pos_list)
            colours of sprinkles to spawn (colour_index)
            velocity tool was used (velo)
        */
        public Vector3 position;
        public Quaternion rotation;
        public string name;
        public bool placing;
        public Vector3[] pos_list;
        public int[] colour_index;
        public Vector3 velo;
    }

    void Update()
    {
        if (hand)
        {
            transform.position = hand.transform.position;
            transform.rotation = hand.transform.rotation;
            velocity = ((transform.position - previous)) / Time.deltaTime;
            previous = transform.position;
        }
        if (owner) // send message to other players updating them of sprinkle tool's behaviour
        {
            if (isPlacing)
            {
                Vector3[] positions = new Vector3[NUMBER_OF_SPRINKLES]; // list of slightly different positions of sprinkles
                int[] colour_indices = new int[NUMBER_OF_SPRINKLES]; // list of random colours

                for (int i = 0; i < NUMBER_OF_SPRINKLES; i++)
                {
                    positions[i] = transform.position + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
                    colour_indices[i] = Random.Range(0, colors.Length);
                }
                context.SendJson(new Message() // send all info
                {
                    position = transform.position,
                    rotation = transform.rotation,
                    name = transform.name,
                    placing = isPlacing,
                    pos_list = positions,
                    colour_index = colour_indices,
                    velo = velocity
                });
                throwSprinkle(transform.position, transform.rotation, positions, colour_indices, velocity); // throw the sprinkle
            }
            else
            {
                context.SendJson(new Message() // if not placing
                {
                    position = transform.position,
                    rotation = transform.rotation,
                    name = transform.name,
                    placing = isPlacing,
                    pos_list = {},
                    colour_index = {},
                    velo = velocity
                });
            }
        }

    }
    
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();

        if (msg.name == transform.name)
        {
            if (msg.placing)
            {
                throwSprinkle(msg.position, msg.rotation, msg.pos_list, msg.colour_index, msg.velo);
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


