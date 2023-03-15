using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using Ubiq.Spawning;
using Ubiq.XR;
using Ubiq.Samples;

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
    public void buttonPressedSpawnPrefab(int prefabID)
    {
        SpawnPrefab(prefabID);
    }

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

        Vector3 cakeSpawn = new Vector3(0f, 0.297f, 0f); // so round cakes spawn above the ground
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
    {
        public bool spawn;
        public int prefab_ID;
        public bool destroy;
        public string destroyObjectName;
        public bool confirm;
    }

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
    public void changeMaterial_button(int x)
    {
        var cakeBaseScript = currentCake.GetComponent<CakeBase>();
        cakeBaseScript.changeMaterial_send_msg(x);
    }
}

