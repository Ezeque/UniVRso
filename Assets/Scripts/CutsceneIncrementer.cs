using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class CutsceneIncrementer : MonoBehaviour
{
    public int cutsceneCounter = 0;

    public VideoClip[] videoClips;

    public void incrementCounter(){
        cutsceneCounter++;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
