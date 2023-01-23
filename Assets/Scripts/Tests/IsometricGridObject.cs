
using UnityEngine;
using UnityEngine.Tilemaps;

public class IsometricGridObject : MonoBehaviour
{
    public static IsometricGridObject instance;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    private float angle = 30; // The angle of the isometric grid
    public float  size { get; private set; } // The size of each cell in the grid
    
    public float xComponent { get; private set; } // The x component of the angle
    public float yComponent { get; private set; }// The y component of the angle


    [HideInInspector]
    public Grid grid;
    [HideInInspector]
    public Tilemap groundMap;


    private void SetIsometric()
    {
        size = 0.2890626f / 2;
        float angleRad = angle * Mathf.Deg2Rad;
        xComponent = Mathf.Cos(angleRad);
        yComponent = Mathf.Sin(angleRad);
    }

     // returns the real world position on the grid
    public Vector2 FindPositionOnGrid(Vector2 position)
    {
        SetIsometric();
        // Calculate the x and y coordinates on the grid
        var pos = GetGridLocation(position);
        // Return the (x, y) position as a Vector2
        float posX = (pos.x - pos.y) * size * xComponent;
        float posY = (pos.x + pos.y) * size * yComponent;
        return new Vector2(posX, posY);
    }



    // Returns the int x, y position on the grid.
    public Vector2Int GetGridLocation(Vector2 position)
    {
        int x = Mathf.RoundToInt((position.x / xComponent + position.y / yComponent) / size);
        int y = Mathf.RoundToInt((position.y / yComponent - position.x / xComponent) / size);
        return new Vector2Int(x, y);
    }

    public Vector3Int GetTileZ(Vector3 position)
    {
        SetGrid();

        Vector3Int cellIndex = groundMap.WorldToCell(position - Vector3.forward);
        for (int i = groundMap.cellBounds.zMax; i > groundMap.cellBounds.zMin - 1; i--)
        {
            cellIndex.z = i;
            var tile = groundMap.GetTile(cellIndex);
            if (tile != null)
            {

                return cellIndex;
            }
        }
        return cellIndex;
    }


    void SetGrid()
    {
        if (grid != null)
            return;

        grid = FindObjectOfType<Grid>();
        Tilemap[] maps = grid.GetComponentsInChildren<Tilemap>();
        foreach (var map in maps)
        {
            if (map.gameObject.name == "GroundTiles")
            {
                groundMap = map;
            }
        }
    }
}

