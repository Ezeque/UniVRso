using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float raycastDistance = 100f; 
    private Vector3 groundPosition;

    void Start()
    {
        groundPosition = transform.position; 
        RaycastHit hit;
        Ray ray = new Ray(transform.position + Vector3.up * 1f, Vector3.down); 

        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            groundPosition = hit.point; 
        }

        transform.position = new Vector3(transform.position.x, groundPosition.y + 10f, transform.position.z);
    }

    void Update()
    {
        
    }
}
