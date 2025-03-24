using System.Collections.Generic;
using UnityEngine;

public class TreeGeneration : MonoBehaviour
{
    public List<Sprite> trunks = new List<Sprite>();
    public List<Sprite> leaves = new List<Sprite>();
    public SpriteRenderer trunkRenderer;
    public Transform leavesRear;
    public Transform leavesFront;
    public Color darkestRear;
    public Color darkestFront;
    
    public int minPerLayer = 4;
    public int maxPerLayer = 6;

    public PolygonCollider2D leafArea;

    public float minDistance = 0.2f;

}
