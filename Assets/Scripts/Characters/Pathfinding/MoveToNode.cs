using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MoveToNode : MonoBehaviour
{
    public float speed;
    public bool facingRight;
    [HideInInspector]
    public List<Vector3> path = new List<Vector3>();
    int currentPathIndex;
    Vector3 targetPos;
    [HideInInspector]
    public Vector3 currentDestination;
    Vector3 currentPosition;
    
    [HideInInspector]
    public bool hasPath;
    [HideInInspector]
    public bool pathComplete;
    [HideInInspector]
    public CurrentTilePosition currentTilePosition;
    [HideInInspector]
    public float moveSpeed;
    
    [HideInInspector]
    public List<TileDirectionInfo> tileBlockInfo;

    Vector2 offset;

    private IEnumerator Start()
    {
        
        currentTilePosition = GetComponent<CurrentTilePosition>();
        yield return new WaitForSeconds(0.3f);
        pathComplete = true;
        currentPathIndex = 0;
    }

    public void PathFound(List<Vector3> newPath)
    {
        offset = new Vector2(Random.Range(0.05f, 0.2f), Random.Range(0.05f, 0.2f));
        path.Clear();
        currentPathIndex = 0;
        path = newPath;
        path[path.Count - 1] = path[path.Count - 1] += (Vector3)offset;
        hasPath = true;
        currentDestination = path[0];
        pathComplete = false;

    }

    public void ClearPath()
    {
        moveSpeed = 0;
        hasPath = false;
        pathComplete = true;
    }


    public void Move()
    {
        var dir = currentDestination - transform.position;
        
        if (dir.x != 0)
        {
            if (dir.x > 0.01f && !facingRight)
                Flip();
            else if (dir.x < 0.01f && facingRight)
                Flip();
        }

        var dist = Vector2.Distance(transform.position, currentDestination);
        if (dist <= 0.02f && currentPathIndex == path.Count - 1)
        {
            ClearPath();
        }

        if (!pathComplete)
        {
            
            currentPosition = transform.position;
            currentPosition = Vector2.MoveTowards(transform.position, new Vector2(currentDestination.x, currentDestination.y), Time.deltaTime * speed);
            currentPosition.z = currentTilePosition.position.z + 1;
            transform.position = currentPosition;
            moveSpeed = 1;

            if (dist <= 0.02f && currentPathIndex < path.Count)
            {
                currentPathIndex++;
                currentDestination = path[currentPathIndex];
                
            }

        }
        UpdateCurrentTilePosition();
    }

    void TileFound(List<TileDirectionInfo> tileBlock, bool success)
    {
        if (success)
            tileBlockInfo = tileBlock;
        else
            Debug.LogError("Tile not found in tile dictionary!");
    }

    void UpdateCurrentTilePosition()
    {
        Vector2 direction = currentDestination - transform.position;
        Vector3 checkPosition = (transform.position + (Vector3)direction * 0.15f) - Vector3.forward;
        Vector3Int nextTilePosition = currentTilePosition.grid.WorldToCell(checkPosition);

        Vector3Int nextTileKey = nextTilePosition - currentTilePosition.position;
        TileInfoRequestManager.RequestTileInfo(currentTilePosition.position, TileFound);

        if (tileBlockInfo == null || nextTileKey == Vector3Int.zero)
            return;

        int level;
        foreach (var tile in tileBlockInfo)
        {
            if (tile.direction == nextTileKey)
                level = tile.levelZ;
            else
                continue;

            ChangeNPCLocation(nextTileKey.x, nextTileKey.y, level);
        }

    }

    
    void ChangeNPCLocation(int x, int y, int z)
    {
        var newPos = new Vector3Int(x, y, z);
        currentTilePosition.position += newPos;
    }


    public Vector3 GetTileWorldPosition(Vector3Int tile)
    {
        var tileworldpos = currentTilePosition.groundMap.GetCellCenterWorld(tile);
        return tileworldpos;
    }

    public void Flip()
    {
        // Switch the way the player is labelled as facing
        facingRight = !facingRight;

        // Multiply the player's x local scale by -1
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

}
