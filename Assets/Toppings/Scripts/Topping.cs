using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ubiq.Messaging;
using Ubiq.Spawning;

// Script is attached to Strawberry, Flower and Candle (and any other toppings) game objects
public class Topping : MonoBehaviour
{
    Transform lastPlacement;
    NetworkContext context;
    public Vector3 lastPosition;
    public Quaternion lastRotation;
    private Rigidbody rigidBody;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }


    // Detects collision with floor and cake and acts accordingly
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Cake")
        {
            rigidBody.isKinematic = true; // remove physics of topping
        }
        if (other.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 5); // destroy gameobject after 5s of touching floor
        }
    }
}
