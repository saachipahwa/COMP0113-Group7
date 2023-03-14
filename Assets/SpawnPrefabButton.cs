using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using Ubiq.Spawning;
using Ubiq.XR;
using Ubiq.Samples;

public class SpawnPrefabButton : MonoBehaviour
{
    public GameObject[] prefabToSpawn;
    private GameObject player;
    
    /*
    0: icing (sphere)
    1: icing (star)
    2: candle
    3: flower
    4: strawberry
    5: eraser
    */
    private Hand hand;
    private GameObject spawnedObject = null;
    void Start()
    {
        player = GameObject.Find("Player");
    }
    public void SpawnPrefab(int prefabID)
    {
        // set the spawn position to be in front of the player
        Vector3 spawnPosition = player.transform.position;
        Vector3 playerDirection = player.transform.forward;
        spawnPosition += playerDirection;
        spawnPosition.y += 0.8f;
        Quaternion spawnRotation = player.transform.rotation;

        // destroy the existing tool if player has one
        if (spawnedObject != null)
        {
            Destroy(spawnedObject);
        }

        // slightly hacky way to spawn the correct icing shape
        spawnedObject = Instantiate(prefabToSpawn[prefabID], spawnPosition, spawnRotation);
        if (prefabID == 0 || prefabID == 1)
        {
            var icing_script = spawnedObject.GetComponent<IcingBrush>();
            icing_script.icingID = prefabID;
        }

        // TODO: if hand is not null, destroy object in hand, then .attach()
    }
}

