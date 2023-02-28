using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CakeTexture : MonoBehaviour
{
    MeshFilter cakeMesh;
    Mesh mesh; 
    void Start()
    {
        cakeMesh = GetComponent<MeshFilter>();
        mesh = cakeMesh.mesh;
        Vector2[] uvMap = mesh.uv;

        // front
        uvMap[0] = new Vector2(0,0.1f);
        uvMap[1] = new Vector2(0.1f,0.1f);
        uvMap[2] = new Vector2(0,0.5f);
        uvMap[3] = new Vector2(0.1f,0.5f);

        // top
        uvMap[4] = new Vector2(0.1f,0.1f);
        uvMap[5] = new Vector2(0.5f,0.1f);
        uvMap[8] = new Vector2(0.1f,0.5f);
        uvMap[9] = new Vector2(0.5f,0.5f);

        // back
        uvMap[6] = new Vector2(0.5f,0.1f);
        uvMap[7] = new Vector2(0.6f,0.1f);
        uvMap[10] = new Vector2(0.5f,0.5f);
        uvMap[11] = new Vector2(0.6f,0.5f);

        // bottom
        uvMap[12] = new Vector2(0.6f,0.1f);
        uvMap[13] = new Vector2(1,0.1f);
        uvMap[14] = new Vector2(0.6f,0.5f);
        uvMap[15] = new Vector2(1,0.5f);

        // left
        uvMap[16] = new Vector2(0.1f,0.5f);
        uvMap[17] = new Vector2(0.5f,0.5f);
        uvMap[18] = new Vector2(0.1f,0.6f);
        uvMap[19] = new Vector2(0.5f,0.6f);

        // right
        uvMap[20] = new Vector2(0.1f,0);
        uvMap[21] = new Vector2(0.5f,0);
        uvMap[22] = new Vector2(0.1f,0.1f);
        uvMap[23] = new Vector2(0.5f,0.1f);

        mesh.uv = uvMap;

    }
}
