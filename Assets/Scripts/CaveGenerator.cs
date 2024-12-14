using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveGenerator : MonoBehaviour
{
    public int width = 200;          
    public int height = 200;         
    public int depth = 200;              
    public float fillPercent = 0.1f; 
    public int smoothIterations = 5; 
    public GameObject player;        
    public Material caveMaterial;    

    private int[,,] caveMap;         
    private GameObject caveObject;   
    private MeshFilter meshFilter;   
    private MeshRenderer meshRenderer; 

    public GameObject GetCaveObject() => caveObject; 

    public IEnumerator GenerateCaveAsync()
    {
        caveMap = new int[width, height, depth];
        InitializeCave();
        SmoothCave();

        caveObject = new GameObject("GeneratedCave");
        caveObject.transform.position = Vector3.zero;

        meshFilter = caveObject.AddComponent<MeshFilter>();
        meshRenderer = caveObject.AddComponent<MeshRenderer>();

        if (caveMaterial != null)
        {
            meshRenderer.material = caveMaterial;
        }
        else
        {
            Debug.LogWarning("Cave material not set. Using default material.");
            meshRenderer.material = new Material(Shader.Find("Standard"));
        }

        GenerateMesh();

        MeshCollider meshCollider = caveObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = meshFilter.mesh;

        caveObject.SetActive(false);

        yield return null;
    }

    public void EnterCave()
    {
        if (caveObject == null || player == null)
        {
            Debug.LogError("Cave, player, or cave entrance not properly set up.");
            return;
        }

        caveObject.SetActive(true);

        Terrain[] terrains = FindObjectsOfType<Terrain>();
        foreach (Terrain terrain in terrains)
        {
            terrain.gameObject.SetActive(false);
        }

        Vector3 spawnPosition = FindWalkablePosition();
        if (spawnPosition != Vector3.zero)
        {
            player.transform.position = spawnPosition;
        }
        else
        {
            Debug.LogError("No walkable position found inside the cave.");
        }
    }

    private void InitializeCave()
    {
        System.Random rand = new System.Random();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    if (x == 0 || x == width - 1 || y == 0 || y == height - 1 || z == 0 || z == depth - 1)
                    {
                        caveMap[x, y, z] = 1; 
                    }
                    else
                    {
                        caveMap[x, y, z] = (rand.NextDouble() < fillPercent) ? 1 : 0;
                    }
                }
            }
        }
    }


    private void SmoothCave()
    {
        for (int i = 0; i < smoothIterations; i++)
        {
            int[,,] newMap = new int[width, height, depth];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        int neighborCount = CountNeighbors(x, y, z);

                        if (neighborCount > 5) newMap[x, y, z] = 1;
                        else if (neighborCount < 4) newMap[x, y, z] = 0;
                        else newMap[x, y, z] = caveMap[x, y, z];
                    }
                }
            }

            caveMap = newMap;
        }
    }

    private int CountNeighbors(int x, int y, int z)
    {
        int count = 0;

        for (int nx = -1; nx <= 1; nx++)
        {
            for (int ny = -1; ny <= 1; ny++)
            {
                for (int nz = -1; nz <= 1; nz++)
                {
                    if (nx == 0 && ny == 0 && nz == 0) continue;

                    int checkX = x + nx;
                    int checkY = y + ny;
                    int checkZ = z + nz;

                    if (checkX >= 0 && checkX < width && checkY >= 0 && checkY < height && checkZ >= 0 && checkZ < depth)
                    {
                        count += caveMap[checkX, checkY, checkZ];
                    }
                    else
                    {
                        count++;
                    }
                }
            }
        }

        return count;
    }

    private void GenerateMesh()
    {
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    if (caveMap[x, y, z] == 1) 
                    {
                        AddCubeMesh(vertices, triangles, x, y, z);
                    }
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals(); 

        meshFilter.mesh = mesh;
    }

    private void AddCubeMesh(List<Vector3> vertices, List<int> triangles, int x, int y, int z)
    {
        int vertexIndex = vertices.Count;

        Vector3[] cubeVertices = new Vector3[]
        {
            new Vector3(x, y, z),
            new Vector3(x + 1, y, z),
            new Vector3(x + 1, y + 1, z),
            new Vector3(x, y + 1, z),
            new Vector3(x, y, z + 1),
            new Vector3(x + 1, y, z + 1),
            new Vector3(x + 1, y + 1, z + 1),
            new Vector3(x, y + 1, z + 1)
        };

        int[] cubeTriangles = new int[]
        {
            0, 2, 1, 0, 3, 2,
            4, 5, 6, 4, 6, 7, 
            0, 1, 5, 0, 5, 4,
            2, 3, 7, 2, 7, 6,
            0, 4, 7, 0, 7, 3,
            1, 2, 6, 1, 6, 5 
        };

        for (int i = 0; i < cubeVertices.Length; i++)
        {
            vertices.Add(cubeVertices[i]);
        }

        for (int i = 0; i < cubeTriangles.Length; i++)
        {
            triangles.Add(vertexIndex + cubeTriangles[i]);
        }
    }

    private Vector3 FindWalkablePosition()
    {
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                for (int z = 1; z < depth - 1; z++)
                {
                    if (caveMap[x, y, z] == 0) 
                    {
                        return new Vector3(x + 0.5f, y + 0.5f, z + 0.5f);
                    }
                }
            }
        }

        return Vector3.zero; 
    }
}
