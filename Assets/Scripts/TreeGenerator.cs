using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    public List<GameObject> treePrefabs;
    public float treeDensity = 0.1f;
    private List<GameObject> generatedTrees = new List<GameObject>();

    public void PlaceTrees(Terrain terrain)
    {
        int width = terrain.terrainData.heightmapResolution;
        int height = width;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (Random.value < treeDensity)
                {
                    Vector3 position = new Vector3(i, 0, j);
                    position.y = GetTerrainHeightAtPosition(terrain, position);
                    position += terrain.transform.position; 

                    GameObject treePrefab = treePrefabs[Random.Range(0, treePrefabs.Count)];

                    generatedTrees.Add(Instantiate(treePrefab, position, Quaternion.identity));
                }
            }
        }
    }


    float GetTerrainHeightAtPosition(Terrain terrain, Vector3 position)
    {
        RaycastHit hit;
        Ray ray = new Ray(position + Vector3.up * 100f, Vector3.down);
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point.y;
        }

        return 0;
    }

    public List<GameObject> GetGeneratedTrees()
    {
        return generatedTrees; 
    }
}
