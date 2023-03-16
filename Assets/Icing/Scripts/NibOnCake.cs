using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script is attached to the nib of the icing brush. keeps track if it is touching the cake.
//because icing can only be placed if nib is touching cake

public class NibOnCake : MonoBehaviour
{
    public bool isTouching;

    private void Start() 
    //start function initialises boolean keeping track of if the nib is touching a cake
    {
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
