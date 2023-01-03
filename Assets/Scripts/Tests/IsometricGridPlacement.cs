using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IsometricGridPlacement : MonoBehaviour
{
    IsometricGridObject gridObject;
   
    public GameObject prefabToPlace;
    
    
    bool placeTileModeActive;

    bool positionValid;

    public Sprite validTile, invalidTile;
    public GameObject tileOutline;
    private SpriteRenderer outlineSprite;

    public LayerMask obstacleLayer;
    Vector2 currentPosition;
    void Start()
    {
        gridObject = IsometricGridObject.instance;
        outlineSprite = tileOutline.GetComponent<SpriteRenderer>();
        tileOutline.SetActive(false);
    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.B))
            placeTileModeActive = !placeTileModeActive;
        if (!placeTileModeActive)
        {
            tileOutline.SetActive(false);
            return;
        }
            
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 0;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        currentPosition = gridObject.FindPositionOnGrid(mousePosition);
        //var mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        ShowOutlineTile(currentPosition);
        if(Input.GetMouseButtonDown(0))
        {
            PlaceTile(prefabToPlace);
        }
    }

    public void ShowOutlineTile(Vector2 position)
    {
        positionValid = false;
        Vector3 finalPos = new Vector3(position.x / 2, position.y / 2, PlayerInformation.instance.player.position.z);
        tileOutline.transform.position = finalPos;
        if (CanPlace(finalPos))
        {
            outlineSprite.sprite = validTile;
            positionValid = true;
        }
        else
        {
            outlineSprite.sprite = invalidTile;
            positionValid = false;
        }
        tileOutline.SetActive(true);
    }

    public void PlaceTile(GameObject go)
    {

        if (positionValid)
        {
            GameObject item = Instantiate(go);

            item.transform.position = tileOutline.transform.position;
            item.transform.SetParent(transform);
        }

    }

    // returns the world position of a grid x,y
    

    bool CanPlace(Vector3 position)
    {
        bool can = CanPlaceOnGroundTile(position)
            && CheckNeighbor(position, true)
            && CheckNeighbor(position, false)
            && !CollidingWithPlayer()
            && !CollidingWithObstacle()
            && MaxGridDistanceFromPlayer();

        return can;
    }

    bool CheckNeighbor(Vector2 position, bool onX)
    {
        // Calculate the x and y coordinates on the grid
        var pos = gridObject.GetGridLocation(position);
        pos.x = onX ? pos.x + 1 : pos.x;
        pos.y = !onX ? pos.y - 1 : pos.y;
        // Return the (x, y) position as a Vector2
        float posX = (pos.x - pos.y) * gridObject.size * gridObject.xComponent;
        float posY = (pos.x + pos.y) * gridObject.size * gridObject.yComponent;
        return CanPlaceOnGroundTile(new Vector3(posX / 2, posY / 2, PlayerInformation.instance.player.position.z));
    }

    
    bool MaxGridDistanceFromPlayer()
    {
        bool can = false;
        var pos = gridObject.GetGridLocation(PlayerInformation.instance.player.position);
        var mousePos = gridObject.GetGridLocation(currentPosition);
        for (int x = -2; x < 3; x++)
        {
            for (int y = -2; y < 3; y++)
            {
                
                var checkPos = new Vector2(pos.x + x, pos.y + y);
                //Debug.Log(checkPos + " : " + mousePos);
                if (checkPos == mousePos/2)
                    can = true;
            }
        }
        return can;
    }

    bool CollidingWithPlayer()
    {
        return gridObject.FindPositionOnGrid(PlayerInformation.instance.player.position) == currentPosition;
    }

    bool CollidingWithObstacle()
    {
        var hit = Physics2D.OverlapCircle(tileOutline.transform.position, gridObject.size /2, obstacleLayer);
        return hit != null;
    }

    bool CanPlaceOnGroundTile(Vector3 position)
    {
        return position.z - 1 == gridObject.GetTileZ(position).z;
    }

    
}