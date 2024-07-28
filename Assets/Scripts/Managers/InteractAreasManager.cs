
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuadTree
{
    public Bounds bounds;
    public int capacity;
    public List<DrawZasYDisplacement> allSpots = new List<DrawZasYDisplacement>();
    public bool divided;
    public QuadTree northWest;
    public QuadTree northEast;
    public QuadTree southWest;
    public QuadTree southEast;

    public QuadTree(Bounds bounds, int capacity)
    {
        this.bounds = bounds;
        this.capacity = capacity;
    }

    public void Insert(DrawZasYDisplacement spot)
    {
        if (!bounds.Contains(spot.transform.position))
            return;

        if (allSpots.Count < capacity)
            allSpots.Add(spot);
        else
        {
            if (!divided)
                Subdivide();
            
               
            northEast.Insert(spot);
            northWest.Insert(spot);
            southWest.Insert(spot);
            southEast.Insert(spot);
        }
           
    }

    

    public void Subdivide()
    {
        Bounds nw = new Bounds(new Vector3(bounds.center.x - bounds.extents.x / 2, bounds.center.y + bounds.extents.y / 2), bounds.extents);
        northWest = new QuadTree(nw, capacity);
        Bounds ne = new Bounds(new Vector3(bounds.center.x + bounds.extents.x / 2, bounds.center.y + bounds.extents.y / 2), bounds.extents);
        northEast = new QuadTree(ne, capacity);
        Bounds sw = new Bounds(new Vector3(bounds.center.x - bounds.extents.x / 2, bounds.center.y - bounds.extents.y / 2), bounds.extents);
        southWest = new QuadTree(sw, capacity);
        Bounds se = new Bounds(new Vector3(bounds.center.x + bounds.extents.x / 2, bounds.center.y - bounds.extents.y / 2), bounds.extents);
        southEast = new QuadTree(se, capacity);

        divided = true;
    }


    public List<DrawZasYDisplacement> QueryTree(Bounds boundry)
    {
        
        List<DrawZasYDisplacement> spots = new List<DrawZasYDisplacement>();

        if (!bounds.Intersects(boundry))
            return spots;

        foreach (var spot in allSpots)
        {
            if (spot == null)
                continue;
            if (boundry.Contains(spot.transform.position))
                spots.Add(spot);

        }

        if(divided)
        {
            spots.AddRange(northWest.QueryTree(boundry));
            spots.AddRange(northEast.QueryTree(boundry));
            spots.AddRange(southWest.QueryTree(boundry));
            spots.AddRange(southEast.QueryTree(boundry));
        }

        return spots;
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
