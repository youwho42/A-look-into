using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor.Localization.Plugins.XLIFF.V12;

public class MazeGenerator : MonoBehaviour
{
    public class MazeTile
    {
        public bool collapsed;
        public List<MazeTilePiece> tileOptions = new List<MazeTilePiece>();
    }
    
    public List<MazeTilePiece> tilePieces = new List<MazeTilePiece>();

    public int mazeSize = 10;
    public int zLevel;
    public Vector3Int bottomCornerTileLocation;
    public GridManager gridManager;
    public Transform mazeHolder;
    List<MazeTile> mazeTiles = new List<MazeTile>();
    List<MazeTile> lastMaze = new List<MazeTile>();

    private void Start()
    {
        for (int x = 0; x < mazeSize; x++)
        {
            for (int y = 0; y < mazeSize; y++)
            {
                MazeTile tile = new MazeTile();
                tile.collapsed = false;
                tile.tileOptions = new List<MazeTilePiece>(tilePieces);
                mazeTiles.Add(tile);
            }
        }
        
    }

    [ContextMenu("Generate Maze")]
    public void GenerateMaze()
    {
        ClearMaze();
        
        lastMaze = SortMazeTiles(mazeTiles);
        List<MazeTile> next = new List<MazeTile>();
        for (int x = 0; x < mazeSize; x++)
        {
            for (int y = 0; y < mazeSize; y++)
            {
                int index = y + x * mazeSize;
                if (lastMaze[index].collapsed)
                    next.Add(lastMaze[index]);
                else
                {
                    foreach (var piece in lastMaze[index].tileOptions)
                    {

                    }
                }
                //var gridPos = new Vector3Int(bottomCornerTileLocation.x + x, bottomCornerTileLocation.y + y, bottomCornerTileLocation.z);
                //var pos = gridManager.groundMap.GetCellCenterWorld(gridPos) + Vector3Int.forward;
                //int r = Random.Range(0, mazePieces.Count);
                //Instantiate(mazePieces[r], pos, Quaternion.identity, mazeHolder);
            }
        }
    }

    List<MazeTile> SortMazeTiles(List<MazeTile> maze)
    {
        var l = maze.OrderBy(bob => bob.tileOptions.Count).ToList();
        return l;
    }



    void ClearMaze()
    {
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
