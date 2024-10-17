
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuadTree
{
    public Bounds bounds;
    public int capacity;
    private List<DrawZasYDisplacement> allSpots;
    private bool divided;
    private QuadTree northWest;
    private QuadTree northEast;
    private QuadTree southWest;
    private QuadTree southEast;

    public QuadTree(Bounds bounds, int capacity)
    {
        this.bounds = bounds;
        this.capacity = capacity;
        this.allSpots = new List<DrawZasYDisplacement>();
        this.divided = false;
    }

    public bool Insert(DrawZasYDisplacement spot)
    {
        if (!bounds.Contains(spot.transform.position))
            return false;

        if (allSpots.Count < capacity)
        {
            allSpots.Add(spot);
            return true;
        }
        else
        {
            if (!divided)
                Subdivide();

            // Insert the spot into the appropriate quadrant
            if (northWest.Insert(spot)) return true;
            if (northEast.Insert(spot)) return true;
            if (southWest.Insert(spot)) return true;
            if (southEast.Insert(spot)) return true;
        }

        return false; // Should not reach here
    }

    private void Subdivide()
    {
        float halfWidth = bounds.size.x / 2f;
        float halfHeight = bounds.size.y / 2f;
        Vector3 center = bounds.center;

        // Define the bounds for each quadrant
        Bounds nw = new Bounds(new Vector3(center.x - halfWidth / 2f, center.y + halfHeight / 2f, center.z), new Vector3(halfWidth, halfHeight, bounds.size.z));
        northWest = new QuadTree(nw, capacity);

        Bounds ne = new Bounds(new Vector3(center.x + halfWidth / 2f, center.y + halfHeight / 2f, center.z), new Vector3(halfWidth, halfHeight, bounds.size.z));
        northEast = new QuadTree(ne, capacity);

        Bounds sw = new Bounds(new Vector3(center.x - halfWidth / 2f, center.y - halfHeight / 2f, center.z), new Vector3(halfWidth, halfHeight, bounds.size.z));
        southWest = new QuadTree(sw, capacity);

        Bounds se = new Bounds(new Vector3(center.x + halfWidth / 2f, center.y - halfHeight / 2f, center.z), new Vector3(halfWidth, halfHeight, bounds.size.z));
        southEast = new QuadTree(se, capacity);

        divided = true;

        // Redistribute existing spots into children
        List<DrawZasYDisplacement> spotsToRedistribute = new List<DrawZasYDisplacement>(allSpots);
        allSpots.Clear();
        foreach (var spot in spotsToRedistribute)
        {
            Insert(spot);
        }
    }

    public List<DrawZasYDisplacement> QueryTree(Bounds boundary)
    {
        List<DrawZasYDisplacement> foundSpots = new List<DrawZasYDisplacement>();

        if (!bounds.Intersects(boundary))
            return foundSpots;

        foreach (var spot in allSpots)
        {
            if (spot == null)
                continue;

            if (boundary.Contains(spot.transform.position))
                foundSpots.Add(spot);
        }

        if (divided)
        {
            foundSpots.AddRange(northWest.QueryTree(boundary));
            foundSpots.AddRange(northEast.QueryTree(boundary));
            foundSpots.AddRange(southWest.QueryTree(boundary));
            foundSpots.AddRange(southEast.QueryTree(boundary));
        }

        return foundSpots;
    }
}

public class InteractAreasManager : MonoBehaviour
{
    public SpotType spotType;
    
    public List<DrawZasYDisplacement> allAreas = new List<DrawZasYDisplacement>();

    public Bounds baseBounds = new Bounds(new Vector3(13, -10, 0), new Vector3(128, 128, 20));
    QuadTree quadTree;

    float lastRefreshTime;
    float maxRefreshRate = 120;

    void Start()
    {
        RefreshQuadTree();
        lastRefreshTime = Time.time;
    }

    void RefreshQuadTree()
    {
        allAreas.Clear();
        var all = FindObjectsOfType<DrawZasYDisplacement>();
        foreach (var item in all)
        {
            if (item.spotType == spotType && GridManager.instance.GetTileValid(item.transform.position))
            {
                allAreas.Add(item);
            }
        }

        quadTree = new QuadTree(baseBounds, 10);
        
        foreach (var spot in allAreas)
        {
            quadTree.Insert(spot);
        }
    }

    public List<DrawZasYDisplacement> QueryQuadTree(Bounds boundry)
    {
        var t = quadTree.QueryTree(boundry);
        if (Time.time - lastRefreshTime >= maxRefreshRate)
        {
            lastRefreshTime = Time.time;
            RefreshQuadTree();
        }

        return t;
    }


}
