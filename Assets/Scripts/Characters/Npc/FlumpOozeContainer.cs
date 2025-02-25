using System.Collections.Generic;
using UnityEngine;

public class FlumpOozeContainer : MonoBehaviour
{
    public FlumpOoze flumpOoze;
    public List<Vector3Int> tilePositions = new List<Vector3Int>();  
    public void SetOozeFromSave(Color color, Vector3 position)
    {
        var ooze = Instantiate(flumpOoze, position, Quaternion.identity);
        ooze.SetOoze(transform, color, true);
    }

    public void SetOccupiedTile(Vector3 position)
    {
        var pos = GridManager.instance.GetTilePosition(position);
        if (!tilePositions.Contains(pos))
            tilePositions.Add(pos);
        
        foreach (var tile in tilePositions)
        {
            if(PathRequestManager.instance.pathfinding.isometricGrid.nodeLookup.ContainsKey(tile))
                PathRequestManager.instance.pathfinding.isometricGrid.nodeLookup[tile].walkable = false;
        }
    }

    public void ResetOccupiedTiles()
    {
        foreach (var tile in tilePositions)
        {
            if (!PathRequestManager.instance.pathfinding.isometricGrid.nodeLookup.ContainsKey(tile))
                return;
            var worldPos = GridManager.instance.GetTileWorldPosition(tile) + Vector3Int.forward;
            Collider2D obstacleCheck = Physics2D.OverlapCircle(worldPos, 0.1f, LayerMask.GetMask("Obstacle"), worldPos.z, worldPos.z);
            if (obstacleCheck == null)
                PathRequestManager.instance.pathfinding.isometricGrid.nodeLookup[tile].walkable = true;
        }
    }

    
}
