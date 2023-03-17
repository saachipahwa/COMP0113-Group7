using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprinkle : MonoBehaviour
{
    private Rigidbody r;
    void Start()
    {
        r = GetComponent<Rigidbody>();
        r.isKinematic = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Cake")
        {
            Debug.Log("hi");
            r.isKinematic = true;
        }
    }
}
