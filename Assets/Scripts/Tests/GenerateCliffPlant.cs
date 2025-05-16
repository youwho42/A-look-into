using System;
using System.Collections.Generic;
using UnityEngine;
public enum VineSegmentName
{
    Base,
    Mid,
    Tip
}

public class GenerateCliffPlant : MonoBehaviour
{
    [Serializable]
    public struct LeavesObject
    {
        public bool isActive;
        [ConditionalHide("isActive", true)]
        public Collider2D leafArea;
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
    [Serializable]
    public struct VineObject
    {
        public bool isActive;
        [ConditionalHide("isActive", true)]
        public List<GenerateVineObject> vineBases;
        [ConditionalHide("isActive", true)]
        public List<GenerateVineObject> vineSegments;
        [ConditionalHide("isActive", true)]
        public List<GenerateVineObject> vineTips;
        [ConditionalHide("isActive", true)]
        public Color lightest;
        [ConditionalHide("isActive", true)]
        public Color darkest;
        [ConditionalHide("isActive", true)]
        public bool flipped;
        [ConditionalHide("isActive", true)]
        public float heightVariance;
        
    }

    public List<LeavesObject> leaveObjects = new List<LeavesObject>();
    public VineObject vineObject;

    public List<Sprite> leaves = new List<Sprite>();

    //public PolygonCollider2D leafArea;
    public Transform plantHolder;
    public Transform vineHolder;

    public float minDistance = 0.2f;
    public float displacement;
    public bool isOnCliffEdge;
    [ConditionalHide("isOnCliffEdge", true)]
    public AnimationCurve easeInAmount;

    private void OnDrawGizmosSelected()
    {
        var pos = new Vector3(0, GlobalSettings.SpriteDisplacementY * displacement, displacement) + transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, 0.1f);
        Gizmos.color = Color.cyan;
        
    }
}
