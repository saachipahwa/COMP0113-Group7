using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprinkle : MonoBehaviour
{
    private Rigidbody r;
    void Start()
    {
        transform.position += new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
        transform.Rotate(Random.Range(0, 90f), Random.Range(0f, 90f), Random.Range(0f, 90f));

        r = GetComponent<Rigidbody>();
        r.isKinematic = false;
        r.velocity = GetComponentInParent<SprinkleController>().velo;
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == "Cake")
        {
            r.isKinematic = true;
        }
        else if (other.collider.tag != "Sprinkle")
        {
            Destroy(gameObject, 1);
        }
    }
}
