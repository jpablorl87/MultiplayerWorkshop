using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private int poolSize = 20;
    private List<GameObject> pooledObjects = new List<GameObject>();
    private void Awake()
    {
        if (obstaclePrefab == null)
        {
            Debug.LogError("[ObjectPool] Error: ObstaclePrefab not assigned");
            return;
        }
        for (int i= 0; i < poolSize; i++)
        {
            //create object and make it child of this manager
            GameObject obj = Instantiate(obstaclePrefab, transform);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }
    public GameObject GetPooledObject()
    {
        foreach (GameObject obj in pooledObjects)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }
        Debug.LogError("[ObjectPool] Pool vacía, aumenta el tamaño.");
        return null;
    }
    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        //Reset position
        obj.transform.localPosition = Vector3.zero;
        //Reset rotation
        obj.transform.localRotation = Quaternion.identity;
        //Reset scale
        obj.transform.localScale = Vector3.one;
        //Reset velocities
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }
}
