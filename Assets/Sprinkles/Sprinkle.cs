using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ubiq.Messaging;

public class Sprinkle : MonoBehaviour
{
    private Rigidbody r;
    public Color[] colors;
    private bool colSet = false;
    public bool owner;
    private NetworkContext context;
    private int randomIndex;


    void Start()
    {
        // if (owner)
        // {
        //     transform.position += new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
        //     transform.Rotate(Random.Range(0, 90f), Random.Range(0f, 90f), Random.Range(0f, 90f));
        // }
        // colSet = true;

        // colors = new Color[6];

        // colors[0] = Color.red; // red
        // colors[1] = Color.white; // white
        // colors[2] = Color.green; // green
        // colors[3] = Color.blue; // blue
        // colors[4] = Color.yellow; // yellow
        // colors[5] = Color.magenta; // magenta

        r = GetComponent<Rigidbody>();
        // r.isKinematic = false;

        // if (owner)
        // {
        // randomIndex = Random.Range(0, colors.Length);

        // Color randomColor = colors[randomIndex];

        // GetComponent<Renderer>().material.color = randomColor;
        // r.velocity = GetComponentInParent<SprinkleController>().velo;
        // }


    }

    // void FixedUpdate()
    // {
        // if (owner)
        // {
        //     context.SendJson(new Message()
        //     {
        //         position = transform.position,
        //         rotation = transform.rotation,
        //         colourIndex = randomIndex
        //     });
        // }
    // }

    // public struct Message
    // {
    //     public Vector3 position;
    //     public Quaternion rotation;
    //     public int colourIndex;
    // }

    // public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    // {
    //     var msg = message.FromJson<Message>();
        // colSet = true;
        // GetComponent<Renderer>().material.color = colors[msg.colourIndex];
        // transform.position = msg.position;
        // transform.rotation = msg.rotation;
    // }


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
