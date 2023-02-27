using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ubiq.Messaging;
using Ubiq.XR;

public class SprayCan : MonoBehaviour, IGraspable, IUseable
{
    Hand grapsed;

    public bool isUsing;

    // NetworkContext context;

    public void Grasp(Hand controller)
    {
        grapsed = controller;
    }

    public void Release(Hand controller)
    {
        grapsed = null;
    }

    public void UnUse(Hand controller)
    {
        isUsing = false;
    }

    public void Use(Hand controller)
    {
        isUsing = true;
    }
    void Start()
    {
        // context = NetworkScene.Register(this);
    }

    struct Message
    {
        public Vector3 position;
    }


    void FixedUpdate() 
    {
        int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask) && (hit.collider.tag == "Cake"))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
            // Texture2D tMap = (Texture2D)hit.collider.GetComponent<Renderer>().material.mainTexture;
            // int x = Mathf.FloorToInt(hit.point.x);
            // int y = Mathf.FloorToInt(hit.point.y);

            // Color pColor = tMap.GetPixel(x , y);
            // requiredComponents.CollisionEvents.PixelColor = pColor;
            Renderer rend = hit.transform.GetComponent<Renderer>();
            MeshCollider meshCollider = hit.collider as MeshCollider;

            if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
                return;

            Texture2D tex = rend.material.mainTexture as Texture2D;
            Vector2 pixelUV = hit.textureCoord;
            pixelUV.x *= tex.width;
            pixelUV.y *= tex.height;

            tex.SetPixel((int)pixelUV.x, (int)pixelUV.y, Color.black);
            tex.Apply();
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            Debug.Log("Did not Hit");
        }
    }
    void LateUpdate()
    {
        if (grapsed)
        {
            transform.localPosition = grapsed.transform.position;
            transform.localRotation = grapsed.transform.rotation;
            // context.SendJson(new Message()
            // {
            //     position = transform.localPosition
            // });
        }

        // if (fired) {
        //     Debug.Log("used");
        // }
    }

    // public void ProcessMessage(ReferenceCountedSceneGraphMessage m)
    // {
    //     transform.localPosition = m.FromJson<Message>().position;
    // }
}
