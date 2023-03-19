using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectPoolManager : MonoBehaviour
{
    
    public int poolSize = 3;
    public GameObject prefab;

    private Queue<GameObject> objectPool = new Queue<GameObject>();

    void Start()
    {
        
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            objectPool.Enqueue(obj);
        }
    }

    public GameObject GetObjectFromPool(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        
        if (objectPool.Count == 0)
        {
            GameObject obj = Instantiate(prefab, pos, rot);
            obj.SetActive(false);
            objectPool.Enqueue(obj);
        }

        
        GameObject pooledObject = objectPool.Dequeue();
        pooledObject.SetActive(true);
        return pooledObject;
    }

    public void ReturnObjectToPool(GameObject obj)
    {
        
        obj.SetActive(false);
        objectPool.Enqueue(obj);
    }

}
