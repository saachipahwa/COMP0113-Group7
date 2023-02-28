using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp : MonoBehaviour
{
    private Texture2D image;
    public int res = 1024;
    // Start is called before the first frame update
    void Start()
    {
        this.image = new Texture2D(res, res, TextureFormat.RGBA32, false);
        GetComponent<Renderer>().material.SetTexture("_MainTex", this.image);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 256; i++)
        {
            var x = Random.Range(0, res);
            var y = Random.Range(0, res);
            this.image.SetPixel(x, y, Random.ColorHSV(0f, 1f, 0f, 0f, 0f, 1f, 0f, 1f));
        }
        this.image.Apply();
    }
}
