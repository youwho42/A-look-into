using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point
{
    public Vector2 position;
    public Vector2 previousPosition;
    public bool lockedPosition;
}
public class Stick
{
    public Point pointA; 
    public Point pointB;
    public float stickLength;
}

public class RopeSimulator : MonoBehaviour
{
    
}
