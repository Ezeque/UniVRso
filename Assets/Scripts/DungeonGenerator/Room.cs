using System;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Vector2Int coordinates;
    public Room[] connections;

    public string objectName = "FloatingObject"; 

    public void SpawnObject(GameObject parent)
    {
        GameObject prefab = Resources.Load<GameObject>(objectName);

        if (prefab == null)
        {
            Debug.LogWarning($"Objeto '{objectName}' não encontrado no diretório Resources.");
            return;
        }

        Vector3 spawnPosition = transform.position; 
        GameObject spawnedObject = Instantiate(prefab, spawnPosition, Quaternion.identity);
        spawnedObject.transform.parent = parent.transform;
        GameObject ObjSpawnPosition = parent.transform.Find("SpawnPoint").gameObject;
        spawnedObject.transform.position = ObjSpawnPosition.transform.position;
        Debug.Log("Instanciou o objeto");

        Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = spawnedObject.AddComponent<Rigidbody>();
        }

        rb.useGravity = false;

        Debug.Log($"Objeto '{objectName}' instanciado no centro da sala.");
    }
}
