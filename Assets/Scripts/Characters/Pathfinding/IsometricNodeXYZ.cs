using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsometricNodeXYZ : IHeapItem<IsometricNodeXYZ>
{

    public bool walkable;
    public Vector3Int worldPosition;
    public bool slope;
    public string tileName;
    // vaiable of vector2 slope direction x or y
    public int gCost;
    public int hCost;

    public int movementPenalty;
    public int movementPenaltyModifier;

    public int gridX;
    public int gridY;
    public int gridZ;

    public IsometricNodeXYZ parent;
    int heapIndex;
    public IsometricNodeXYZ(bool isWalkable, Vector3Int position, int x, int y, int z, int penalty, bool isSlope, string slopeTileName)
    {
        walkable = isWalkable;
        worldPosition = position;
        gridX = x;
        gridY = y;
        gridZ = z;
        movementPenalty = penalty;
        slope = isSlope;
        tileName = slopeTileName;

        //int slopeHeightDir -1 or 1
    }

    public int FCost
    {
        get { return gCost + hCost; }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(IsometricNodeXYZ nodeToCompare)
    {
        int compare = FCost.CompareTo(nodeToCompare.FCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}
