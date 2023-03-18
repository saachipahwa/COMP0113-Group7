using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ubiq.XR;

public class SprinkleController : MonoBehaviour
{
    public Vector3 velo;
    public void setVelocity(Vector3 velocity)
    {
        velo = velocity;
    }
}
