using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ubiq.XR;

public class SprinklesTool : MonoBehaviour, IGraspable, IUseable
{
    private Hand hand;
    private Vector3 previous;
    private Vector3 velocity;
    public GameObject sprinklePrefab;

    private objectPoolManager ObjectPoolManager;

    void Start()
    {
        
        //ObjectPoolManager = GameObject.FindObjectOfType<objectPoolManager>();

    }

    public void Grasp(Hand controller)
    {
        hand = controller;
    }

    public void Release(Hand controller)
    {
        hand = null;
    }

    public void UnUse(Hand controller)
    {

    }

    public void Use(Hand controller)
    {
        GameObject spawnedSprinkle = Instantiate(sprinklePrefab, transform.position, transform.rotation);

        //GameObject spawnedSprinkle = ObjectPoolManager.GetObjectFromPool(sprinklePrefab, transform.position, transform.rotation);

        spawnedSprinkle.GetComponent<SprinkleController>().setVelocity(velocity * 2); // add networking to spawn as well on other clients
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
