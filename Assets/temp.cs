using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ubiq.XR;

public class temp : MonoBehaviour, IGraspable
{
    private Rigidbody rigidBody;
    private Hand hand;
    private Vector3 previous;
    private Vector3 velocity;
    public void Grasp(Hand controller)
    {
        hand = controller;
    }

    public void Release(Hand controller)
    {
        hand = null;
        rigidBody.velocity = velocity;
    }
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hand)
        {
            transform.position = hand.transform.position;
        }
        velocity = ((transform.position - previous)) / Time.deltaTime;
        previous = transform.position;
        Debug.Log(velocity);
    }
}
