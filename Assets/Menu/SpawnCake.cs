using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using Ubiq.Spawning;
using Ubiq.XR;
using Ubiq.Samples;

// Script is attached to CakeSpawnManager and contains methods used by buttons used to create the cakes at the start, as well as the buttons to change the flavour of the cake
public class SpawnCake : MonoBehaviour
{
    public GameObject toolsMenu;
    public GameObject basesMenu;
    public GameObject[] prefabToSpawn;
    /*
    0-2: round cake
    3-6: square cake
    */
    private GameObject currentCake = null;

    NetworkContext context;

    void Start()
    {
        context = NetworkScene.Register(this);
    }
    // if button is pressed, spawn the specified cake
    public void buttonPressedSpawnPrefab(int prefabID)
    {
        SpawnPrefab(prefabID);
    }

    // destroys current cake if exists and spawns a new one
    // sends message to destroy current cake and to spawn a new one for other players
    public void SpawnPrefab(int prefabID, bool owner = true)
    {
        if (owner)
        {
            // destroy the existing cake if exists
            if (currentCake != null)
            {
                context.SendJson(new Message()
                {
                    spawn = false,
                    prefab_ID = prefabID,
                    destroy = true,
                    destroyObjectName = currentCake.name,
                    confirm = false
                });
                Destroy(currentCake);
            }
        }

        Vector3 cakeSpawn = new Vector3(0f, 0.126f, 0f); // so round cakes spawn above the ground
        if (prefabID > 2)
        {
            cakeSpawn = new Vector3(0f, 0.146f, 0f); // if square cake, move it a bit lower
        }

        currentCake = Instantiate(prefabToSpawn[prefabID], cakeSpawn, Quaternion.identity);
        currentCake.name = "Cake";

        if (owner)
        {
            context.SendJson(new Message()
            {
                spawn = true,
                prefab_ID = prefabID,
                destroy = false,
                destroyObjectName = "",
                confirm = false
            });
            context.SendJson(new Message()
            {
                spawn = false,
                prefab_ID = prefabID,
                destroy = false,
                destroyObjectName = "",
                confirm = false
            });
        }
    }

    struct Message
    /*
    message contains:
        if a cake was spawned (spawn)
        what cake it was (prefab_ID)
        if a cake was destroyed (destroy)
        which one was destroyed (destroyObjectName)
        and if cake menu was closed using 'confirm' button (confirm)
    */
    {
        public bool spawn;
        public int prefab_ID;
        public bool destroy;
        public string destroyObjectName;
        public bool confirm;
    }

    // acts upon message depending on if cake was spawned, destroyed or if menu was closed
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();
        if (msg.spawn)
        {
            SpawnPrefab(msg.prefab_ID, false);
        }
        else if (msg.destroy)
        {
            GameObject toDestroy = GameObject.Find(msg.destroyObjectName);
            Destroy(toDestroy);
        }
        else if (msg.confirm)
        {
            Confirm(false);
        }
    }

    public void Confirm(bool owner = true)
    // called when confirm is pressed and spawns second menu
    {
        if (currentCake != null)
        {
            if (owner)
            {
                context.SendJson(new Message()
                {
                    spawn = false,
                    prefab_ID = -1,
                    destroy = false,
                    destroyObjectName = currentCake.name,
                    confirm = true
                });
            }
            basesMenu.SetActive(false);
            toolsMenu.SetActive(true);
        }
    }

    // called when the buttons to change flavour are pressed
    public void changeMaterial_button(int x)
    {
        var cakeBaseScript = currentCake.GetComponent<CakeBase>();
        cakeBaseScript.changeMaterial_send_msg(x);
    }
}

