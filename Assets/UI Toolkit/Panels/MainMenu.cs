using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    public static int[] cake = new int[2];

    private void Awake()
    {
        
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        root.Q<Button>("square").clicked += () =>
        {
            cake[0] = 0;
            Debug.Log("square");
        };
        root.Q<Button>("circle").clicked += () =>
        {
            cake[0] = 1;
            Debug.Log("circle");
        };
        root.Q<Button>("1_tier").clicked += () =>
        {
            cake[1] = 0;
            Debug.Log("1_tier");
        };
        root.Q<Button>("2_tier").clicked += () =>
        {
            cake[1] = 1;
            Debug.Log("2_tier");
        };
        root.Q<Button>("3_tier").clicked += () =>
        {
            cake[1] = 2;
            Debug.Log("3_tier");
        };
        root.Q<Button>("start").clicked += () =>
        {
            Debug.Log("start");
            //SceneManager.LoadScene(Demo);
        };



    }
}
