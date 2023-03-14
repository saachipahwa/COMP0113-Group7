using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using Ubiq.Spawning;
using Ubiq.XR;
using Ubiq.Samples;

public class SpawnPrefabButton : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public int icingID = -1;

    public void SpawnPrefab()
    {
        Vector3 spawnPosition = transform.position;
        Quaternion spawnRotation = transform.rotation;

        GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPosition, spawnRotation);
        if (icingID != -1)
        {
            spawnedObject.GetComponent<IcingBrush>().icingID = icingID;
        }

        // TODO: if hand is not null, destroy object in hand, then .attach()
    }
}

