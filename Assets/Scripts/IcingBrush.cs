using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using Ubiq.XR;
using Ubiq.Messaging;

public class IcingBrush : MonoBehaviour, IGraspable, IUseable 
{
    Hand grasped;
    NetworkContext context;
    private Transform nib;
    private Material drawingMaterial;
    private GameObject currentDrawing;
    private bool owner;

    public void Grasp(Hand controller)
    {
        owner = true;
        grasped = controller;
    }

    public void Release(Hand controller)
    {
        owner = false;
        grasped = null;
    }

    public void Use(Hand controller) 
    {
        BeginDrawing();
    }

    public void UnUse(Hand controller)
    {
        EndDrawing();
    }

    private void BeginDrawing() 
    {
        currentDrawing = new GameObject("Drawing");
        var trail = currentDrawing.AddComponent<TrailRenderer>();
        trail.time = Mathf.Infinity;
        trail.material = drawingMaterial;
        trail.startWidth = .05f;
        trail.endWidth = .05f;
        trail.minVertexDistance = .02f;

        currentDrawing.transform.parent = nib.transform;
        currentDrawing.transform.localPosition = Vector3.zero;
        currentDrawing.transform.localRotation = Quaternion.identity;
    } 
    
    private void EndDrawing()
    {
        var trail = currentDrawing.GetComponent<TrailRenderer>();
        currentDrawing.transform.parent = null;
        currentDrawing.GetComponent<TrailRenderer>().emitting = false;
        currentDrawing = null;
    }

    struct Message
    {
        public Vector3 position;
        public Quaternion rotation;
        public bool isDrawing;

        public Message(Transform transform, bool isDrawing)
        {
            this.position = transform.position;
            this.rotation = transform.rotation;
            this.isDrawing = isDrawing;
        }
    }

    public void ProcessMessage (ReferenceCountedSceneGraphMessage msg)
    {
        var data = msg.FromJson<Message>();
        transform.position = data.position;
        transform.rotation = data.rotation;

        if (data.isDrawing && !currentDrawing)
        {
            BeginDrawing();
        }
        if (!data.isDrawing && currentDrawing)
        {
            EndDrawing();
        }
    }

    void Start()
    {
        nib = transform.Find("PipingTip/Nib");
        context = NetworkScene.Register(this);
        var shader = Shader.Find("Particles/Standard Unlit");
        drawingMaterial = new Material(shader);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (owner)
        {
            context.SendJson(new Message(transform, isDrawing:currentDrawing));
        }
    }

    private void LateUpdate()
    {
        if (grasped)
        {
            transform.position = grasped.transform.position;
            transform.rotation = grasped.transform.rotation;
        }
    }

}
