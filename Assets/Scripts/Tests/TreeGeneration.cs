using System;
using System.Collections.Generic;
using UnityEngine;

public class TreeGeneration : MonoBehaviour
{
    [Serializable]
    public struct TrunkObject
    {
        public Sprite trunk;
        public PolygonCollider2D leafArea;
        public int minPerLayer;
        public int maxPerLayer;
    }
    public List<TrunkObject> trunks = new List<TrunkObject>();
    public List<Sprite> leaves = new List<Sprite>();
    
    public SpriteRenderer trunkRenderer;
    public Transform leavesRear;
    public Transform leavesFront;
    public Color lightestRear;
    public Color darkestRear;
    public Color darkestFront;

    public float minDistance = 0.2f;

    public TreeShadows shadows;
    public Material shadowMaterial;
    public TreeLeavesShake leavesShake;
}
