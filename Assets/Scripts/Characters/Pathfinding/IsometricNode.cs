using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IsometricNode : IHeapItem<IsometricNode>
{

    public bool walkable;
    public Vector3Int worldPosition;
    // variable of bool slope
    // vaiable of vector2 slope direction x or y
    public int gCost;
    public int hCost;

    public int movementPenalty;

    public int gridX;
    public int gridY;

    public IsometricNode parent;
    int heapIndex;
    public IsometricNode (bool isWalkable, Vector3Int position, int x, int y, int penalty)
    {
        walkable = isWalkable;
        worldPosition = position;
        gridX = x;
        gridY = y;
        movementPenalty = penalty;
        //gridZ = int z
        //bool isSlope
        
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

    public int CompareTo(IsometricNode nodeToCompare)
    {
        int compare = FCost.CompareTo(nodeToCompare.FCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}
