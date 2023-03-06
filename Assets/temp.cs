using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ubiq.XR;
using UnityEngine;
using Ubiq.Spawning;

public class temp : MonoBehaviour, IUseable
{
    private Hand hand;
    public GameObject prefabToSpawn;

    public void Use(Hand controller)
    {
        // var go = NetworkSpawnManager.Find(this).SpawnWithPeerScope(prefabToSpawn);
        // var candle = prefabToSpawn.GetComponent<ToppingTool>();
        // candle.owner = true;
        // if (candle != null)
        // {
        //     candle.Attach(hand);
        // }
    }
    public void UnUse(Hand controller)
    {
    }
    void Start()
    {
    
    }

}
