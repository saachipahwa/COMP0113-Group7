using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprinkle : MonoBehaviour
{
    private Rigidbody r;
    public Color[] colors;

    private objectPoolManager ObjectPoolManager;

    void Start()
    {
        ObjectPoolManager = GameObject.FindObjectOfType<objectPoolManager>();

        transform.position += new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
        transform.Rotate(Random.Range(0, 90f), Random.Range(0f, 90f), Random.Range(0f, 90f));

        colors = new Color[6];

        colors[0] = Color.red; // red
        colors[1] = Color.white; // white
        colors[2] = Color.green; // green
        colors[3] = Color.blue; // blue
        colors[4] = Color.yellow; // yellow
        colors[5] = Color.magenta; //magenta

        int randomIndex = Random.Range(0, colors.Length);

        Color randomColor = colors[randomIndex];

        GetComponent<Renderer>().material.color = randomColor;


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
            Destroy(gameObject);
            //ObjectPoolManager.ReturnObjectToPool(gameObject);
        }
    }
}
