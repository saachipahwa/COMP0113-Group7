using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ubiq.XR;

public class SprinkleController : MonoBehaviour, IGraspable
{
    private Rigidbody rigidBody;
    private Hand hand;
    private Vector3 previous;
    private Vector3 velocity;
    private Transform[] allChildTransforms;
    private List<GameObject> allChildObjs;
    public void Grasp(Hand controller)
    {
        hand = controller;
    }

    public void Release(Hand controller)
    {
        hand = null;
        foreach (GameObject child in allChildObjs)
        {
            Rigidbody r = child.GetComponent<Rigidbody>();
            r.isKinematic = false;
            r.velocity = velocity;// + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        } 
    }
    void Start()
    {
        // rigidBody = GetComponent<Rigidbody>();
        allChildTransforms = GetComponentsInChildren<Transform>(includeInactive: false);
        allChildObjs = new List<GameObject>();
        bool first = true;
        foreach (Transform child in allChildTransforms)
        {
            if (first)
            {
                first = false;
                continue;
            }
            allChildObjs.Add(child.gameObject);
            child.position += new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
            child.Rotate(Random.Range(0, 90f), Random.Range(0f, 90f), Random.Range(0f, 90f));
        }
    }

    void Update()
    {
        if (hand)
        {
            transform.position = hand.transform.position;
            velocity = ((transform.position - previous)) / Time.deltaTime;
            previous = transform.position;
        }
    }
}
