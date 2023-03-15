using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using Ubiq.Spawning;
using Ubiq.XR;
using Ubiq.Samples;

public class SpawnPrefabButton : MonoBehaviour
{
    private int objectLabel = 0; // this is to ensure unique names for spawned tools
    public GameObject[] prefabToSpawn;
    /*
    0: icing (sphere)
    1: icing (star)
    2: candle
    3: flower
    4: strawberry
    5: eraser
    */
    private GameObject player;
    // private Hand hand;
    private GameObject currentPlayerTool = null;
    private Color icingColour;
    NetworkContext context;

    void Start()
    {
        player = GameObject.Find("Player");
        context = NetworkScene.Register(this);
    }
    public void buttonPressedSpawnPrefab(int prefabID)
    {
        SpawnPrefab(prefabID);
    }
    public void SpawnPrefab(int prefabID, Vector3? pos = null, Quaternion? rot = null, bool owner = true)
    {
        // Debug.Log("here");
        Vector3 spawnPosition = Vector3.zero;
        Quaternion spawnRotation = Quaternion.identity;

        // if we specify a position and rotation, use that. Otherwise use the player's position and rotation
        if (pos == null)
        {
            spawnPosition = player.transform.position;
        }
        else
        {
            spawnPosition = (Vector3)pos;
        }
        if (rot == null)
        {
            spawnRotation = player.transform.rotation;
        }
        else{
            spawnRotation = (Quaternion)rot;
        }

        // set the spawn position to be in front of the player
        Vector3 playerDirection = player.transform.forward;
        spawnPosition += playerDirection;
        spawnPosition.y += 0.8f;

        if (owner)
        {
            // destroy the existing tool if player has one
            if (currentPlayerTool != null)
            {
                context.SendJson(new Message()
                {
                    position = transform.position,
                    rotation = transform.rotation,
                    spawn = false,
                    prefab_ID = prefabID,
                    destroy = true,
                    destroyObjectName = currentPlayerTool.name
                });
                Destroy(currentPlayerTool);
            }
        }

        // slightly hacky way to spawn the correct icing shape
        GameObject spawnedObject = Instantiate(prefabToSpawn[prefabID], spawnPosition, spawnRotation);
        spawnedObject.name = $"{objectLabel}_{spawnedObject.name}";
        objectLabel++;
        
        switch(prefabID)
        {
            case 0: // icing
            case 1: 
                var icing_script = spawnedObject.GetComponent<IcingBrush>();
                icing_script.icingID = prefabID;
                icing_script.owner = owner;
                icing_script.colour = icingColour;
                break;
            case 5: // eraser
                var eraser_script = spawnedObject.GetComponent<Eraser>();
                eraser_script.owner = owner;
                break;
            default: // toppings
                var topping_script = spawnedObject.GetComponent<ToppingTool>();
                topping_script.owner = owner;
                break;
        }

        if (owner)
        {
            currentPlayerTool = spawnedObject;
            context.SendJson(new Message()
            {
                position = transform.position,
                rotation = transform.rotation,
                spawn = true,
                prefab_ID = prefabID,
                destroy = false,
                destroyObjectName = ""
            });
            context.SendJson(new Message()
            {
                position = transform.position,
                rotation = transform.rotation,
                spawn = false,
                prefab_ID = prefabID,
                destroy = false,
                destroyObjectName = ""
            });
        }

        // TODO: if hand is not null, destroy object in hand, then .attach()
    }

    public void setColour(Color c)
    {
        icingColour = c;
    }


    struct Message
    {
        public Vector3 position;
        public Quaternion rotation;
        public bool spawn;
        public int prefab_ID;
        public bool destroy;
        public string destroyObjectName;
    }

    public void ProcessMessage (ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();
        if (msg.spawn)
        {
            SpawnPrefab(msg.prefab_ID, msg.position, msg.rotation, false);
        }
        else if (msg.destroy)
        {
            GameObject toDestroy = GameObject.Find(msg.destroyObjectName);
            Destroy(toDestroy);
        }
    }
}

