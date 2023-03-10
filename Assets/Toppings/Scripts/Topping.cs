using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ubiq.Messaging;
using Ubiq.Spawning;


public class Topping : MonoBehaviour
{
    Transform lastPlacement;
    NetworkContext context;
    public Vector3 lastPosition;
    public Quaternion lastRotation;
    private Rigidbody rigidBody;

    void Start()
    {
        Debug.Log($"{transform.position} | {transform.rotation}");
        rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
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
