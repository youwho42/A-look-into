using UnityEngine;
using System.Collections.Generic;


public class FlowerManager : MonoBehaviour
{


    public static FlowerManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private Dictionary<Vector2Int, List<DrawZasYDisplacement>> flowerGrid = new Dictionary<Vector2Int, List<DrawZasYDisplacement>>();
    public float cellSize = 1f;



    public void RegisterFlower(DrawZasYDisplacement flower)
    {

        var key = GetCell(flower.transform.position);
        if (!flowerGrid.TryGetValue(key, out var list))
            flowerGrid[key] = list = new List<DrawZasYDisplacement>();
        list.Add(flower);

    }

    public List<DrawZasYDisplacement> GetFlowers(Vector3 position, float radius)
    {
        var results = new List<DrawZasYDisplacement>();
        int cells = Mathf.CeilToInt(radius / cellSize);
        var center = GetCell(position);

        for (int x = -cells; x <= cells; x++)
            for (int y = -cells; y <= cells; y++)
            {
                var key = center + new Vector2Int(x, y);
                if (flowerGrid.TryGetValue(key, out var list))
                    foreach (var f in list)
                        if (NumberFunctions.GetDistanceV2(f.transform.position, position) <= radius)
                            results.Add(f);
            }
        return results;
    }

    private Vector2Int GetCell(Vector2 pos) {
        return new Vector2Int(Mathf.FloorToInt(pos.x / cellSize), Mathf.FloorToInt(pos.y / cellSize));
    }

}
