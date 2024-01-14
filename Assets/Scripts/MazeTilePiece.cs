using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileDirections
{
    TopLeft,
    TopRight, 
    BottomLeft, 
    BottomRight
}
[Serializable]
public struct TileOption
{
    public TileDirections Direction;
    public List<Transform> TileObjects;
}

public class MazeTilePiece : MonoBehaviour
{
    public List<TileOption> tileOptions = new List<TileOption>();
}
