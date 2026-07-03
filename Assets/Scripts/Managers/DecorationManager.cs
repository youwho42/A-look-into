using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public enum DecorationType
{
    None,
    Flower,
    Grass
}

public class DecorationManager : MonoBehaviour
{



    public static DecorationManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    //private Dictionary<Vector2Int, List<DrawZasYDisplacement>> flowerGrid = new Dictionary<Vector2Int, List<DrawZasYDisplacement>>();
    private Dictionary<DecorationType, Dictionary<Vector2Int, List<DrawZasYDisplacement>>> decoTypes = new Dictionary<DecorationType, Dictionary<Vector2Int, List<DrawZasYDisplacement>>>();

    private  Dictionary<Vector2Int, List<ReplaceObjectOnItemDrop>> replaceItems = new Dictionary<Vector2Int, List<ReplaceObjectOnItemDrop>>();
    public float cellSize = 1f;

   

    public void ClearDecorationRegistry()
    {
        decoTypes.Clear();
    }

    public void RegisterDecoration(DecorationType decorationType, DrawZasYDisplacement item)
    {
        if (decorationType == DecorationType.None)
            return;

        var decoKey = GetCell(item.transform.position);

        if (!decoTypes.TryGetValue(decorationType, out var decoDict))
        {
            decoDict = new Dictionary<Vector2Int, List<DrawZasYDisplacement>>();
            decoTypes[decorationType] = decoDict;
        }

        if (!decoDict.TryGetValue(decoKey, out var list))
        {
            list = new List<DrawZasYDisplacement>();
            decoDict[decoKey] = list;
        }

        list.Add(item);
    }

    public void RegisterDecoration(ReplaceObjectOnItemDrop item)
    {
        var decoKey = GetCell(item.transform.position);

        if (!replaceItems.TryGetValue(decoKey, out var list))
        {
            list = new List<ReplaceObjectOnItemDrop>();
            replaceItems[decoKey] = list;
        }
        if(!list.Contains(item))
            list.Add(item);
        
    }
    public void UnRegisterDecoration(ReplaceObjectOnItemDrop item)
    {
        foreach (var kvp in replaceItems)
        {
            if (kvp.Value.Remove(item))
            {
                
                if (kvp.Value.Count == 0)
                    replaceItems.Remove(kvp.Key);
                return;
            }
        }
    }

    public List<DrawZasYDisplacement> GetDecorations(DecorationType decorationType, Vector3 position, float radius)
    {
        var results = new List<DrawZasYDisplacement>();
        if (decoTypes.TryGetValue(decorationType, out var decoDict))
        {
            float r = radius * 0.5f;
            int cells = Mathf.CeilToInt(r / cellSize);
            var center = GetCell(position);

            for (int x = -cells; x <= cells; x++)
                for (int y = -cells; y <= cells; y++)
                {
                    var key = center + new Vector2Int(x, y);
                    if (decoDict.TryGetValue(key, out var list))
                        foreach (var f in list)
                        {
                            if (f == null)
                                continue;
                            
                            if (NumberFunctions.GetDistanceV2(f.transform.position, position) <= r)
                                results.Add(f);
                        }
                            
                }
        }


        return results;
    }
    public List<ReplaceObjectOnItemDrop> GetReplaceObjects(Vector3 position, float radius)
    {
        var results = new List<ReplaceObjectOnItemDrop>();
        
        float r = radius * 0.5f;
        int cells = Mathf.CeilToInt(r / cellSize);
        var center = GetCell(position);

        for (int x = -cells; x <= cells; x++)
            for (int y = -cells; y <= cells; y++)
            {
                var key = center + new Vector2Int(x, y);
                if (replaceItems.TryGetValue(key, out var list))
                    foreach (var f in list)
                    {
                        if (f == null)
                            continue;
                        if (NumberFunctions.GetDistanceV2(f.transform.position, position) <= r)
                            results.Add(f);
                    }

            }
        


        return results;
    }

    private Vector2Int GetCell(Vector2 pos) {
        return new Vector2Int(Mathf.FloorToInt(pos.x / cellSize), Mathf.FloorToInt(pos.y / cellSize));
    }

}
