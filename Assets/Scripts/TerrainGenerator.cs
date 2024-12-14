using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public int width;
    public int height;
    public int depth = 50;
    public int scale = 10;

    public int octaves = 4;
    public float persistence = 0.5f;
    public float lacunarity = 2.0f;

    public List<GameObject> caveEntrancePrefabs; 
    private List<GameObject> generatedCaves = new List<GameObject>(); 

    public List<GameObject> associatedObjects = new List<GameObject>(); 

    private Terrain terrain;

    void Awake()
    {
        terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);

        GenerateCaveEntrances();
        AddGeneratedObjects(generatedCaves);
    }

    void Update()
    {
    }

    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        terrainData.size = new Vector3(width, depth, height);
        terrainData.heightmapResolution = width + 1;
        terrainData.SetHeights(0, 0, GenerateHeights());
        return terrainData;
    }

    float[,] GenerateHeights()
    {
        float[,] heights = new float[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                heights[i, j] = CalculateHeight(i, j);
            }
        }
        return heights;
    }

    float CalculateHeight(int i, int j)
    {
        float xCoord = (float)i / width * scale;
        float yCoord = (float)j / height * scale;

        float totalHeight = 0;
        float frequency = 1f;
        float amplitude = 1f;

        for (int octave = 0; octave < octaves; octave++)
        {
            totalHeight += Mathf.PerlinNoise(xCoord * frequency, yCoord * frequency) * amplitude;
            frequency *= lacunarity;
            amplitude *= persistence;
        }

        return totalHeight;
    }

    void GenerateCaveEntrances()
    {
        float terrainWidth = terrain.terrainData.size.x;
        float terrainHeight = terrain.terrainData.size.z;

        Vector3 terrainPosition = terrain.transform.position;
        int numberOfEntrances = 4;

        for (int i = 0; i < numberOfEntrances; i++)
        {
            float randomX = Random.Range(0, terrainWidth);
            float randomZ = Random.Range(0, terrainHeight);

            randomX += terrainPosition.x;
            randomZ += terrainPosition.z;

            float y = terrain.SampleHeight(new Vector3(randomX, 0, randomZ)) + terrainPosition.y;

            Vector3 position = new Vector3(randomX, y, randomZ);

            GameObject caveEntrance = Instantiate(caveEntrancePrefabs[0], position, Quaternion.identity);
            generatedCaves.Add(caveEntrance);
        }
    }


    void AddGeneratedObjects(IEnumerable<GameObject> objects)
    {
        foreach (GameObject obj in objects)
        {
            associatedObjects.Add(obj);
        }
    }

    void OnEnable()
    {
        SetAssociatedObjectsActive(true);
    }

    void OnDisable()
    {
        SetAssociatedObjectsActive(false);
    }

    void SetAssociatedObjectsActive(bool state)
    {
        foreach (GameObject obj in associatedObjects)
        {
            if (obj != null)
            {
                obj.SetActive(state);
            }
        }
    }
}
