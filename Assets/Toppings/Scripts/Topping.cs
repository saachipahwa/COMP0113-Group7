using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ubiq.Messaging;
using Ubiq.Spawning;

//Script is attached to Strawberry, Flower and Candle game objects
public class Topping : MonoBehaviour
{
    Transform lastPlacement;
    NetworkContext context;
    public Vector3 lastPosition;
    public Quaternion lastRotation;
    private Rigidbody rigidBody;

    void Start()
    //start function initialises topping as a rigid body 
    //the purpose of this is to detect collisions with cake and floor
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    //Detects collision with floor and cake and acts accordingly
    {
        if (other.gameObject.tag == "Cake")
        {
            rigidBody.isKinematic = true;
        }
        if (other.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 5); // destroy gameobject after 5s of touching floor
        }
    }
}
