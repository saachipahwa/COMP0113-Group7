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
        if (other.collider.tag == "Cake" || other.collider.tag == "Icing")
        {
            r.isKinematic = true;
        }
        else if (other.collider.tag != "Sprinkle" && other.collider.tag != "Topping") // don't destroy if touching another sprinkle, but don't remove the physics either
        {
            Destroy(gameObject);
        }
    }
}
