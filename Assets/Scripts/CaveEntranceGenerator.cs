using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveEntranceGenerator : MonoBehaviour
{
    public List<GameObject> caveEntrancePrefabs; 
    private List<GameObject> generatedCaves = new List<GameObject>();

    public void GenerateCaveEntrances(Terrain terrain)
    {
        float terrainWidth = terrain.terrainData.size.x;
        float terrainHeight = terrain.terrainData.size.z;

        Vector3 terrainPosition = terrain.transform.position; 
        int numberOfEntrances = caveEntrancePrefabs.Count;

        for (int i = 0; i < numberOfEntrances; i++)
        {
            float randomX = Random.Range(terrainPosition.x, terrainPosition.x + terrainWidth);
            float randomZ = Random.Range(terrainPosition.z, terrainPosition.z + terrainHeight);

            float y = terrain.SampleHeight(new Vector3(randomX, 0, randomZ));

            Vector3 position = new Vector3(randomX, y, randomZ);

            generatedCaves.Add(Instantiate(caveEntrancePrefabs[0], position, Quaternion.identity));
        }
    }



    public List<GameObject> GetGeneratedCaves()
    {
        return generatedCaves;
    }

}
