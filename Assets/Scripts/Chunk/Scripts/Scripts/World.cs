using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public int renderDistance;

    public GameObject chunk;
    public Transform worldGrid;
    Dictionary<Vector2, Chunk> chunkMap;
    public Vector3 lastPos;

    private void Awake()
    {
        chunkMap = new Dictionary<Vector2, Chunk>();
    }

    private void Update()
    {
        float dist = Vector3.Distance(lastPos, transform.position);
        if (dist >= Chunk.size)
        {
            FindChunksToLoad();
            DeleteChunks();
            lastPos = transform.position;
        }


    }

    void FindChunksToLoad()
    {
       
        int xPos = (int)transform.position.x;
        int yPos = (int)transform.position.y;


        for (int i = xPos - (renderDistance * Chunk.size); i < xPos + (renderDistance * Chunk.size); i += Chunk.size)
        {
            for (int j = yPos - (renderDistance * Chunk.size); j < yPos + (renderDistance * Chunk.size); j += Chunk.size)
            {
                MakeChunkAt(i, j);
            }
        }
        
        
    }

    void MakeChunkAt(int x, int y)
    {

        x = Mathf.FloorToInt(x / (float)Chunk.size) * Chunk.size;
        y = Mathf.FloorToInt(y / (float)Chunk.size) * Chunk.size;

        if (!chunkMap.ContainsKey(new Vector2(x, y)))
        {
            GameObject go = Instantiate(chunk, new Vector3(x, y, 0), Quaternion.identity);
            go.GetComponent<Chunk>().SetTiles();
            chunkMap.Add(new Vector2(x, y), go.GetComponent<Chunk>());
        }
    }

    void DeleteChunks()
    {
        List<Chunk> deleteChunks = new List<Chunk>(chunkMap.Values);
        Queue<Chunk> deleteQueue = new Queue<Chunk>();

        for (int i = 0; i < deleteChunks.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, deleteChunks[i].transform.position);
            if (distance > renderDistance * Chunk.size)
            {
                deleteQueue.Enqueue(deleteChunks[i]);
            }
        }
        while (deleteQueue.Count > 0)
        {
            Chunk thisChunk = deleteQueue.Dequeue();
            chunkMap.Remove(thisChunk.transform.position);
            thisChunk.ClearTiles();
            Destroy(thisChunk.gameObject);

        }
    }
}
