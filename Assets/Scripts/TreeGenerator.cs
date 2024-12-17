using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    public List<GameObject> treePrefabs;
    public float treeDensity = 0.1f; 
    public float maxViewDst = 200f;   
    public Transform viewer;          
    public int chunkSize = 50;        
    private int chunksVisibleInViewDst;
    private Dictionary<Vector2, TreeChunk> treeChunkDictionary = new Dictionary<Vector2, TreeChunk>();
    private List<TreeChunk> treeChunksVisibleLastUpdate = new List<TreeChunk>();

    void Start()
    {
        maxViewDst = 200;
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize); 
    }

    void Update()
    {
        UpdateVisibleTreeChunks();
    }

    void UpdateVisibleTreeChunks()
    {
        for (int i = 0; i < treeChunksVisibleLastUpdate.Count; i++)
        {
            treeChunksVisibleLastUpdate[i].SetVisible(false);
        }
        treeChunksVisibleLastUpdate.Clear();

        Vector2 viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (treeChunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    treeChunkDictionary[viewedChunkCoord].UpdateTreeChunk();
                    if (treeChunkDictionary[viewedChunkCoord].IsVisible())
                    {
                        treeChunksVisibleLastUpdate.Add(treeChunkDictionary[viewedChunkCoord]);
                    }
                }
                else
                {
                    treeChunkDictionary.Add(viewedChunkCoord, new TreeChunk(viewedChunkCoord, chunkSize, treePrefabs, viewer));
                }
            }
        }
    }

    public class TreeChunk
    {
        GameObject chunkObject;
        Vector2 position;
        Bounds bounds;
        List<GameObject> trees = new List<GameObject>();
        Transform viewer;
        private float maxViewDst = 200;

        public TreeChunk(Vector2 coord, int size, List<GameObject> treePrefabs, Transform viewer)
        {
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            this.viewer = viewer;

            chunkObject = new GameObject("TreeChunk");
            chunkObject.transform.position = new Vector3(position.x, 0, position.y);

            GenerateTrees(size, treePrefabs);

            SetVisible(false);  
        }

        void GenerateTrees(int size, List<GameObject> treePrefabs)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (Random.value < 0.001f)  
                    {

                        Vector3 position = new Vector3(i, 0, j);
                        position.y = GetTerrainHeightAtPosition(position);
                        GameObject treePrefab = treePrefabs[Random.Range(0, treePrefabs.Count)];

                        trees.Add(Instantiate(treePrefab, position, Quaternion.identity));
                    }
                }
            }
        }

        float GetTerrainHeightAtPosition(Vector3 position)
        {
            RaycastHit hit;
            Ray ray = new Ray(position + Vector3.up * 100f, Vector3.down);
            if (Physics.Raycast(ray, out hit))
            {
                return hit.point.y;
            }

            return 0;
        }


        public void UpdateTreeChunk()
        {
            float viewerDstFromChunk = Mathf.Sqrt(bounds.SqrDistance(new Vector2(viewer.position.x, viewer.position.z)));
            bool visible = viewerDstFromChunk <= maxViewDst;  
        }

        public void SetVisible(bool visible)
        {
            chunkObject.SetActive(visible);  
        }

        public bool IsVisible()
        {
            return chunkObject.activeSelf;
        }
    }
}
