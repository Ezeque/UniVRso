using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class PerlinNoise : MonoBehaviour
{
    public int width = 256;
    public int height = 256;
    float offsetX = 100;
    float offsetY = 100;
    public float scale = 50f;
    private Renderer renderer;

    void Start()
    {
        offsetX = Random.Range(0f, 1000f);
        offsetY = Random.Range(0f, 1000f);
        renderer = gameObject.GetComponent<Renderer>();
        renderer.material.mainTexture = GenerateTexture();
    }

    void Update()
    {
        
    }

    private Texture2D GenerateTexture(){
        Texture2D texture = new Texture2D(width, height);
        for (int i = 0; i < width; i ++){
            for(int j = 0; j < height; j ++){
                Color color = CalculateColor(i, j);
                texture.SetPixel(i, j, color);
            }
        }
        texture.Apply();
        return texture;
    } 

    private Color CalculateColor(int i, int j){
        float xCoord = (float) i / width * scale + offsetX;
        float yCoord = (float) j / height * scale + offsetY;
        float sample = Mathf.PerlinNoise(xCoord, yCoord);
        return new Color(sample, sample, sample);
    }
}