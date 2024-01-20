
using UnityEngine;


public class IsometricGridObject : MonoBehaviour
{
    //public static IsometricGridObject instance;


    //private void Awake()
    //{
    //    if (instance == null)
    //        instance = this;
    //    else
    //        Destroy(gameObject);
    //}
    private float angle = 30; // The angle of the isometric grid
    public float  size { get; private set; } // The size of each cell in the grid
    
    public float xComponent { get; private set; } // The x component of the angle
    public float yComponent { get; private set; }// The y component of the angle


    
    
    private void SetIsometric()
    {
        size = 0.2990625f;
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

    //returns real world position with x y inputs
    public Vector3 GetWorldPosition(float x, float y)
    {
        SetIsometric();

        float worldX = (x - y) * size * xComponent;
        float worldY = (x + y) * size * yComponent;

        Vector3 worldPosition = new Vector3(worldX, worldY, 0f);

        return worldPosition;
    }



    public Vector3Int GetTileZ(Vector3 position)
    {
        var groundMap = GridManager.instance.groundMap;

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


    

    

}

