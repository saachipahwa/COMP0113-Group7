using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour
{

    public float moveSpeed;
    public float radius = 2f;
    public float speed = 1f;
    private float angle;
    // Start is called before the first frame update
    void Start()
    {

    }

    

    void Update()
    {
        angle += speed * Time.deltaTime;

        float y = Mathf.Sin(angle) * radius;
        float z = Mathf.Cos(angle) * radius;

        // Move the GameObject in a circle around the x-axis
        transform.localPosition = new Vector3(transform.localPosition.x, y, z);
    }
}
