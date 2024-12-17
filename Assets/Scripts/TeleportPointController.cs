    using System.Collections;
    using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
    using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class TeleportPointController : MonoBehaviour
    {
        private DungeonGenerator dungeonGenerator;
        public Vector3 cavePosition;
        public bool canEnterCave = true;
        public AudioClip caveMusic;
        void Start()
        {
            canEnterCave = true;
            dungeonGenerator = GameObject.Find("DungeonGenerator").GetComponent<DungeonGenerator>();
        }

        void Update()
        {
            
        }

        void OnTriggerEnter(Collider collider){
            if (collider.tag == "Player" && canEnterCave){
                dungeonGenerator.EnterCave(cavePosition);
                canEnterCave = false;
                GameObject.Find("SoundController").GetComponent<AudioSource>().clip = caveMusic;
                GameObject.Find("SoundController").GetComponent<AudioSource>().Play();
            }
        }

        void OnTriggerExit(Collider collider){
            if (collider.tag == "Player" && canEnterCave){
                canEnterCave = true;
            }
        }
    }
