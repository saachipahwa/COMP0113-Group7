using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NibOnCake : MonoBehaviour
{
    public bool isTouching;

    private void Start() {
        isTouching = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Cake")
        {
            isTouching = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Cake")
        {
            isTouching = false;
        }
    }
}
