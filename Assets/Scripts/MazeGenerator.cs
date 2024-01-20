using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MazeGenerator : MonoBehaviour
{
    public class MazeTile
    {
        public bool collapsed;
        public List<MazeTilePiece> possibleTiles = new List<MazeTilePiece>();
    }
    
    public List<MazeTilePiece> tilePieces = new List<MazeTilePiece>();

    public int mazeSize = 12;
    public int zLevel;
    public Vector3Int bottomCornerTileLocation;
    public GridManager gridManager;
    public Transform mazeHolder;
    List<MazeTile> mazeTiles = new List<MazeTile>();
    MazeTile[,] tileArray;

    [ContextMenu("Initialize Maze")]
    private void InitializeMaze()
    {
        tileArray = new MazeTile[mazeSize, mazeSize];
        mazeTiles.Clear();
        for (int x = 0; x < mazeSize; x++)
        {
            for (int y = 0; y < mazeSize; y++)
            {
                MazeTile tile = new MazeTile();
                tile.collapsed = false;
                tile.possibleTiles = new List<MazeTilePiece>(tilePieces);
                mazeTiles.Add(tile);
                tileArray[x, y] = tile;
                if (x == 0)
                    SetTile(tile, 3, x, y);
                else if (y == 0)
                    SetTile(tile,  2, x, y);
                else if (x == mazeSize-1 && y != 6)
                    SetTile(tile, 0, x, y);
                else if (x == mazeSize - 1 && y == 6)
                    SetTile(tile, 8, x, y);
                else if (y == mazeSize-1)
                    SetTile(tile, 1, x, y);


            }
        }
        
    }

    void SetTile(MazeTile tile, int tileIndex, int xOff, int yOff)
    {
        var gridPos = new Vector3Int(bottomCornerTileLocation.x + xOff, bottomCornerTileLocation.y + yOff, bottomCornerTileLocation.z);
        var pos = gridManager.groundMap.GetCellCenterWorld(gridPos) + Vector3Int.forward;
        tile.collapsed = true;
        var t = tile.possibleTiles[tileIndex];
        tile.possibleTiles.Clear();
        tile.possibleTiles.Add(t);
        Instantiate(t, pos, Quaternion.identity, mazeHolder);

    }

    [ContextMenu("Generate Maze")]
    public void GenerateMaze()
    {
        //ClearMaze();
        
        MazeTile tile = null;
        do
        {
            SetEntropy();
            tile = GetLowestEntropyTile(mazeTiles);
            List<MazeTile> next = new List<MazeTile>();
            for (int y = 0; y < mazeSize; y++)
            {
                for (int x = 0; x < mazeSize; x++)
                {
                
                    int index = y + x * mazeSize;
                    
                    if (mazeTiles[index] == tile)
                    {
                        int r = Random.Range(0, tile.possibleTiles.Count);
                        SetTile(tile, r, x, y);
                    }
                    next.Add(mazeTiles[index]);

                }
            }
            mazeTiles = next;
        } 
        while (tile != null);
        
    }

    MazeTile GetLowestEntropyTile(List<MazeTile> maze)
    {
        List<MazeTile> ordered = maze.OrderBy(bob => bob.possibleTiles.Count).SkipWhile(x=>x.possibleTiles.Count<2).ToList();
        int lowest = -1;
        List<MazeTile> l = new List<MazeTile>();
        if (ordered.Count > 0)
            lowest = ordered[0].possibleTiles.Count;
        for (int i = 0; i < ordered.Count; i++)
        {
            if (ordered[i].possibleTiles.Count == lowest)
                l.Add(ordered[i]);
        }
        int r = Random.Range(0, l.Count);
        if(l.Count>0)
            return l[r];
        return null;
    }


    void CheckNeighbor(int current, int neighbor, TileDirections direction)
    {
        List<MazeTilePiece> newOptions = new List<MazeTilePiece>();
        foreach (var dir in mazeTiles[neighbor].possibleTiles[0].tileDirections)
        {
            if(dir.Direction == direction)
            {
                for (int i = 0; i < dir.TileObjects.Count; i++)
                {
                    if (mazeTiles[current].possibleTiles.Contains(dir.TileObjects[i]))
                        newOptions.Add(dir.TileObjects[i]);
                }
                break;
            }
        }
        


        
        mazeTiles[current].possibleTiles = newOptions;
    }

    
    void SetEntropy()
    {
        for (int y = 0; y < mazeSize; y++)
        {
            for (int x = 0; x < mazeSize; x++)
            {
                
                int current = y + x * mazeSize;
                if (mazeTiles[current].collapsed)
                    continue;
                int neighbor;
                // check tl
                neighbor = (y + 1) + (x * mazeSize);
                if (mazeTiles[neighbor].collapsed)
                    CheckNeighbor(current, neighbor, TileDirections.BottomRight);
                // check tr
                neighbor = y + ((x + 1) * mazeSize);
                if (mazeTiles[neighbor].collapsed)
                    CheckNeighbor(current, neighbor, TileDirections.BottomLeft);
                // check bl
                neighbor = y + ((x - 1) * mazeSize);
                if (mazeTiles[neighbor].collapsed)
                    CheckNeighbor(current, neighbor, TileDirections.TopRight);
                // check br
                neighbor = (y - 1) + (x * mazeSize);
                if (mazeTiles[neighbor].collapsed)
                    CheckNeighbor(current, neighbor, TileDirections.TopLeft);


            }
        }
    }


    void ClearMaze()
    {
        mazeTiles.Clear();
        foreach (Transform child in mazeHolder)
        {
            Destroy(child.gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        
        
        var x = gridManager.groundMap.GetCellCenterWorld(bottomCornerTileLocation);
        Gizmos.DrawWireSphere(x, 0.1f);
        
    }

}
