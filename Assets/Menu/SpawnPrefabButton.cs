using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using Ubiq.Spawning;
using Ubiq.XR;
using Ubiq.Samples;

// This script is attached onto the ToolSpawnManager and has methods used by buttons in the tools selection menu to spawn the correct tool and assign its owner
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
    private GameObject currentPlayerTool = null;
    private Color icingColour;
    NetworkContext context;

    void Start()
    {
        player = GameObject.Find("Player");
        context = NetworkScene.Register(this);
    }
    public void buttonPressedSpawnPrefab(int prefabID) // method used by button OnClick events
    {
        SpawnPrefab(prefabID);
    }
    public void SpawnPrefab(int prefabID, Vector3? pos = null, Quaternion? rot = null, bool owner = true)
    {
        /*
        Spawns the prefab
        
        prefabID: the index of the prefab to be spawned
        pos: position to spawn prefab, will be player's position if not specified
        rot: rotation to spawn prefab, will be player's rotation if not specified
        owner: whether the player is the owner of the tool

        */
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
        spawnPosition += playerDirection * 0.5f;
        spawnPosition.y += 1f;

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
                icing_script.setOwner(owner);
                icing_script.colour = icingColour;
                break;
            case 5: // eraser
                var eraser_script = spawnedObject.GetComponent<Eraser>();
                eraser_script.setOwner(owner);
                break;
            case 6: // sprinkles
                var sprinkles_script = spawnedObject.GetComponent<SprinklesTool>();
                sprinkles_script.setOwner(owner);
                break;
            default: // toppings
                var topping_script = spawnedObject.GetComponent<ToppingTool>();
                topping_script.setOwner(owner);
                break;
        }
        
        // if player is the owner, we send a message to others to tell them to spawn a tool
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
    }

    public void setColour(Color c) // sets colour of icing brush
    {
        icingColour = c;
        if (currentPlayerTool != null)
        {
            if (currentPlayerTool.name.Contains("Icing"))
            {
                var icing_script = currentPlayerTool.GetComponent<IcingBrush>();
                icing_script.colour = c;
            }
        }
    }


    struct Message
    {
        public Vector3 position; // position to spawn prefab
        public Quaternion rotation; // rotation to spawn prefab
        public bool spawn; // spawning flag
        public int prefab_ID; // prefab index
        public bool destroy; // destroying flag
        public string destroyObjectName; // object name to destroy
    }

    public void ProcessMessage (ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();
        // if spawn flag, then spawn prefab at position and rotation specified in message. ensure owner is set to false
        if (msg.spawn)
        {
            SpawnPrefab(msg.prefab_ID, msg.position, msg.rotation, false);
        }
        else if (msg.destroy) // if destroy flag, destroy the object with the name specified in the message
        {
            GameObject toDestroy = GameObject.Find(msg.destroyObjectName);
            Destroy(toDestroy);
        }
    }
}

