using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using Ubiq.Spawning;
using Ubiq.XR;
using UnityEngine;

public class SprinklesTool : MonoBehaviour, IGraspable, IUseable
{
    private Hand hand;
    private Vector3 previous;
    private Vector3 velocity;
    public GameObject sprinklePrefab;

    private bool isPlacing = false;

    private objectPoolManager ObjectPoolManager;

    void Start()
    {
        context = NetworkScene.Register(this);
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
        private bool isPlacing = true;
    }
    
    public struct Message
        /* message contains
            position of topping
            rotation of topping
            what topping it is (name)
            whether user used tool (placing)
        */
    {
        public Vector3 position;
        public Quaternion rotation;
        public string name;
        public bool placing;
    }

    void Update()
    {
        if (hand)
        {
            transform.position = hand.transform.position;
            velocity = ((transform.position - previous)) / Time.deltaTime;
            previous = transform.position;
        }
            if (owner) // send message to other players updating them of topping tool's behaviour
            {
                context.SendJson(new Message()
                {
                    //position = transform.position,
                    //rotation = transform.rotation,
                    //name = transform.name,
                    placing = isPlacing
                });
            }

    }
    
}

    //Can you take a look at this part pls? >> giving error when removing '}' in line 98
    /*
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();

        if (msg.name == transform.name)
        {
            if (msg.placing)
            {
                Use();
            }
            //transform.position = msg.position;
            //transform.rotation = msg.rotation;
        }
    }
    */

