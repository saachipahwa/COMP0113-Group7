using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    public GameObject Cube; // The paintbrush GameObject to spawn

    public void OnClick()
    {
        Instantiate(Cube, transform.position, transform.rotation);
    }
}
