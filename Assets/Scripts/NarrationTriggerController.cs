using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class NarrationTriggerController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject environmentController;
    public Material normalSkybox;
    public Boolean canPlayVideo = false;
    public AudioClip grassAmbience;

    private CutsceneIncrementer cutsceneIncrementer;
    private GameObject dungeon;

    void Start()
    {
        videoPlayer = gameObject.GetComponent<VideoPlayer>();
        if (videoPlayer != null)
        {
            
        }

        cutsceneIncrementer = GameObject.Find("CutsceneManager").GetComponent<CutsceneIncrementer>();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player" && canPlayVideo)
        {
            videoPlayer.loopPointReached += OnVideoPlayerStopped;
            
            videoPlayer.clip = cutsceneIncrementer.videoClips[cutsceneIncrementer.cutsceneCounter];
            dungeon = GameObject.Find("Dungeon");
            Debug.Log("Dungeon Atual: " + dungeon);
            dungeon.SetActive(false);
            if (videoPlayer != null)
            {
                GameObject.Find("SoundController").GetComponent<AudioSource>().Stop();
                videoPlayer.Play(); 
            }
        }
    }

    void OnVideoPlayerStopped(VideoPlayer vp)
    {
        Debug.Log("Chegou no onVideoPlayerStopped");
        videoPlayer.loopPointReached -= OnVideoPlayerStopped;

        if (environmentController != null)
        {
            environmentController.SetActive(true);
        }

        cutsceneIncrementer.incrementCounter();

        StartCoroutine(FindTerrainsAfterActivation());
    }

    IEnumerator FindTerrainsAfterActivation()
    {
        yield return null;

        GameObject[] terrains = GameObject.FindGameObjectsWithTag("Terrain");

        if (terrains.Length > 0)
        {
            RenderSettings.skybox = normalSkybox;
            DynamicGI.UpdateEnvironment(); 

            GameObject selectedTerrain = terrains[UnityEngine.Random.Range(0, terrains.Length)];

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = new Vector3(
                    selectedTerrain.transform.position.x,
                    selectedTerrain.transform.position.y + 50,
                    selectedTerrain.transform.position.z
                );

                RaycastHit hit;
                if (Physics.Raycast(player.transform.position, Vector3.down, out hit, Mathf.Infinity))
                {
                    player.transform.position = new Vector3(
                        player.transform.position.x,
                        hit.point.y,
                        player.transform.position.z
                    );
                }
                else
                {
                    Debug.LogWarning("Raycast não encontrou o chão abaixo do jogador!");
                }
            }
        }
        else
        {
        }
        
        transform.parent.gameObject.SetActive(false);
        GameObject.Find("SoundController").GetComponent<AudioSource>().clip = grassAmbience;
        GameObject.Find("SoundController").GetComponent<AudioSource>().Play();
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoPlayerStopped;
        }
    }
}