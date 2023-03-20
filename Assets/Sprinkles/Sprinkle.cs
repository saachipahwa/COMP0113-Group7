using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ubiq.Messaging;

public class Sprinkle : MonoBehaviour
{
    private Rigidbody r;

    void Start()
    {
        r = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == "Cake")
        {
            r.isKinematic = true;
        }
        else if (other.collider.tag != "Sprinkle")
        {
            Destroy(gameObject);
        }
    }
}
