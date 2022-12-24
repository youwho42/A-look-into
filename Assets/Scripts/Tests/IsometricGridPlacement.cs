using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.Xml;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class IsometricGridPlacement : MonoBehaviour
{
    // Public variables
    public float angle; // The angle of the isometric grid
    public float size;  // The size of each cell in the grid
    //public Dictionary<Vector2Int, Vector2> allTiles = new Dictionary<Vector2Int, Vector2>();
    public GameObject prefabToPlace;
    // Private variables
    private float xComponent; // The x component of the angle
    private float yComponent; // The y component of the angle
    [HideInInspector]
    public Grid grid;
    [HideInInspector]
    public Tilemap groundMap;
    bool canPlace;

    public Sprite validTile, invalidTile;
    public GameObject tileOutline;
    private SpriteRenderer outlineSprite;

    public LayerMask obstacleLayer;
    Vector2 currentPosition;
    void Start()
    {
        // Calculate the x and y components of the angle
        float angleRad = angle * Mathf.Deg2Rad;
        xComponent = Mathf.Cos(angleRad);
        yComponent = Mathf.Sin(angleRad);
        outlineSprite = tileOutline.GetComponent<SpriteRenderer>();
        tileOutline.SetActive(false);
    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.B))
            canPlace = !canPlace;
        if (!canPlace)
        {
            tileOutline.SetActive(false);
            return;
        }
            
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 0;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        currentPosition = FindPositionOnGrid(mousePosition);
        //var mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        ShowOutlineTile(currentPosition);
        if(Input.GetMouseButtonDown(0))
        {
            PlaceTile(currentPosition, prefabToPlace);
        }
    }

    
    public Vector2 FindPositionOnGrid(Vector2 position)
    {

        // Calculate the x and y coordinates on the grid
        var pos = GetGridLocation(position);
        // Return the (x, y) position as a Vector2
        float posX = (pos.x - pos.y) * size * xComponent;
        float posY = (pos.x + pos.y) * size * yComponent;

        return new Vector2(posX, posY);
    }

    bool CheckNeighbor(Vector2 position, bool onX)
    {

        // Calculate the x and y coordinates on the grid
        var pos = GetGridLocation(position);
        pos.x = onX ? pos.x + 1 : pos.x;
        pos.y = !onX ? pos.y - 1 : pos.y;
        // Return the (x, y) position as a Vector2
        float posX = (pos.x - pos.y) * size * xComponent;
        float posY = (pos.x + pos.y) * size * yComponent;
        return CanPlaceOnGroundTile(new Vector3(posX / 2, posY / 2, PlayerInformation.instance.player.position.z));
    }

    public void PlaceTile(Vector2 gridPosition, GameObject go)
    {
        Vector3 finalPos = new Vector3(gridPosition.x / 2, gridPosition.y / 2, PlayerInformation.instance.player.position.z);
        if(CanPlace(finalPos))
        {
            GameObject item = Instantiate(go);

            item.transform.position = finalPos;
            item.transform.SetParent(transform);
        }
    }

    Vector2Int GetGridLocation(Vector2 position)
    {
        
        int x = Mathf.RoundToInt((position.x / xComponent + position.y / yComponent) / size);
        int y = Mathf.RoundToInt((position.y / yComponent - position.x / xComponent) / size);
        return new Vector2Int(x, y);
    }

    public void ShowOutlineTile(Vector2 position)
    {

        Vector3 finalPos = new Vector3(position.x / 2, position.y / 2, PlayerInformation.instance.player.position.z);
        tileOutline.transform.position = finalPos;
        if (CanPlace(finalPos))
        {
            outlineSprite.sprite = validTile;
        }
        else
        {
            outlineSprite.sprite = invalidTile;
        }
        tileOutline.SetActive(true);
    }
    
    bool CanPlace(Vector3 position)
    {
        bool can = CanPlaceOnGroundTile(position) 
            && CheckNeighbor(position, true) 
            && CheckNeighbor(position, false) 
            && !CollidingWithPlayer()
            && !CollidingWithObstacle();

        return can;
    }

    bool CollidingWithPlayer()
    {
        
        return FindPositionOnGrid(PlayerInformation.instance.player.position) == currentPosition;

    }

    bool CollidingWithObstacle()
    {
        var hit = Physics2D.OverlapCircle(tileOutline.transform.position, size/2, obstacleLayer);
        return hit != null;
        
    }

    bool CanPlaceOnGroundTile(Vector3 position)
    {

        return position.z - 1 == GetTileZ(position).z;
        
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