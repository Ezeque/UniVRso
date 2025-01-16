using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedObjectController : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {

    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            GameObject finalRoom = GameObject.Find("Final Room(Clone)");

            if (finalRoom != null)
            {
                Transform narrationTriggerTransform = finalRoom.transform.Find("NarrationTrigger");

                if (narrationTriggerTransform != null)
                {
                    NarrationTriggerController narrationTriggerController = narrationTriggerTransform.GetComponent<NarrationTriggerController>();
                    narrationTriggerController.canPlayVideo = true;
                    GameObject.Find("DungeonGenerator").GetComponent<DungeonGenerator>().currentObjectAmount --;
                    Destroy(gameObject);
                }
            }
        }
    }
}
