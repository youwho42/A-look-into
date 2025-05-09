using System;
using System.Collections.Generic;
using UnityEngine;

public class GenerateCliffPlant : MonoBehaviour
{
    [Serializable]
    public struct LeavesObject
    {
        public bool isActive;
        [ConditionalHide("isActive", true)]
        public PolygonCollider2D leafArea;
        [ConditionalHide("isActive", true)]
        public Color lightest;
        [ConditionalHide("isActive", true)]
        public Color darkest;
        [ConditionalHide("isActive", true)]
        public bool flipped;
        [ConditionalHide("isActive", true)]
        public int minAmount;
        [ConditionalHide("isActive", true)]
        public int maxAmount;
    }

    
    public List<LeavesObject> leaveObjects = new List<LeavesObject>();

    public List<Sprite> leaves = new List<Sprite>();

    //public PolygonCollider2D leafArea;
    public Transform plantHolder;

    public float minDistance = 0.2f;
    public DrawZasYDisplacement displacement;
    public bool isOnCliffEdge;
    [ConditionalHide("isOnCliffEdge", true)]
    public AnimationCurve easeInAmount;
}
