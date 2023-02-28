using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ubiq.Messaging;
using Ubiq.XR;

public class SprayCan : MonoBehaviour, IGraspable, IUseable
{
    Hand grapsed;

    public bool isUsing;

    private Transform head;
    private Texture2D img;
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
        head = transform.Find("Head");
        img = new Texture2D(100, 100, TextureFormat.RGBA32, false);
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
        if (Physics.Raycast(head.position, head.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask) && (hit.collider.tag == "Cake"))
        {
            Debug.DrawRay(head.position, head.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
            // Texture2D tMap = (Texture2D)hit.collider.GetComponent<Renderer>().material.mainTexture;
            // int x = Mathf.FloorToInt(hit.point.x);
            // int y = Mathf.FloorToInt(hit.point.y);

            // Color pColor = tMap.GetPixel(x , y);
            // requiredComponents.CollisionEvents.PixelColor = pColor;
            // Texture2D tex = rend.material.mainTexture as Texture2D;
            // Vector2 pixelUV = hit.textureCoord;
            // pixelUV.x *= tex.width;
            // pixelUV.y *= tex.height;

            // tex.SetPixel((int)pixelUV.x, (int)pixelUV.y, Color.black);
            // tex.Apply();
            Renderer rend = hit.transform.GetComponent<Renderer>();
            MeshCollider meshCollider = hit.collider as MeshCollider;

            // if (rend == null || rend.sharedMaterial == null || meshCollider == null)
            // {
            //     return;
            // }


            if (rend.sharedMaterial.mainTexture == null)
            {
                rend.material.SetTexture("_MainTex", img);
            }
            
            for (int x = 0; x < 10; x++)
            {
                for (int y = 10; y < 50; y++)
                {
                    img.SetPixel(x, y, Color.yellow);
                }
            }
            for (int x = 10; x < 50; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    img.SetPixel(x, y, Color.yellow);
                }
                for (int y = 10; y < 50; y++)
                {
                    img.SetPixel(x, y, Color.blue);
                }
                for (int y = 50; y < 60; y++)
                {
                    img.SetPixel(x, y, Color.yellow);
                }
            }
            for (int x = 50; x < 60; x++)
            {
                for (int y = 10; y < 50; y++)
                {
                    img.SetPixel(x, y, Color.yellow);
                }
            }
            for (int x = 60; x < 100; x++)
            {
                for (int y = 10; y < 50; y++)
                {
                    img.SetPixel(x, y, Color.blue);
                }
            }
            
            img.Apply();
            
            // Texture2D texture2D = rend.material.mainTexture as Texture2D;
            // Renderer renderer = hit.collider.GetComponent<MeshRenderer>();
            // Vector2 pCoord = hit.textureCoord;
            // pCoord.x *= texture2D.width;
            // pCoord.y *= texture2D.height;

            // Vector2 tiling = renderer.material.mainTextureScale;
            // Color color = texture2D.GetPixel(Mathf.FloorToInt(pCoord.x * tiling.x) , Mathf.FloorToInt(pCoord.y * tiling.y));

            // Debug.Log("Picked color : " + color);
        }
        else
        {
            Debug.DrawRay(head.position, head.TransformDirection(Vector3.forward) * 1000, Color.white);
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
