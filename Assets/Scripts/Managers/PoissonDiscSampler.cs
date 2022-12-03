using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PoissonDiscSampler
{
    
    public static List<Vector2> GeneratePoints(float minRadius, Vector2 maxRegionSize, int numSamples = 30)
    {
        float cellSize = minRadius / Mathf.Sqrt(2);
        int[,] grid = new int[Mathf.CeilToInt(maxRegionSize.x / cellSize), Mathf.CeilToInt(maxRegionSize.y / cellSize)];
        List<Vector2> points = new List<Vector2>();
        List<Vector2> spawnPoints = new List<Vector2>();
        spawnPoints.Add(maxRegionSize / 2);
        
        while (spawnPoints.Count > 0)
        {
            int spawnIndex = Random.Range(0, spawnPoints.Count);
            Vector2 spawnCenter = spawnPoints[spawnIndex];

            bool candidateAccepted = false;
            for (int i = 0; i < numSamples; i++)
            {
                float angle = Random.value * Mathf.PI * 2;
                Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                Vector2 candidate = spawnCenter + dir * Random.Range(minRadius, 2 * minRadius);
                if (IsValid(candidate, maxRegionSize, cellSize, minRadius, points, grid))
                {
                    
                    points.Add(candidate);
                    spawnPoints.Add(candidate);
                    //Debug.Log((int)(candidate.x / cellSize) + " : " + (int)(candidate.y / cellSize));
                    grid[(int)(candidate.x / cellSize), (int)(candidate.y / cellSize)] = points.Count;
                    
                    candidateAccepted = true;
                    break;
                }
            }
            if (!candidateAccepted)
            {
                spawnPoints.RemoveAt(spawnIndex);
            }
        }
        return points;
    }

    static bool IsValid(Vector2 candidate, Vector2 maxRegionSize, float cellSize, float radius, List<Vector2> points, int[,] grid)
    {
        if(candidate.x >= 0 && candidate.x <maxRegionSize.x && candidate.y >= 0 && candidate.y < maxRegionSize.y)
        {
            
            int cellX = (int)(candidate.x / cellSize);
            int cellY = (int)(candidate.y / cellSize);
            int startSearchX = Mathf.Max(0, cellX - 2);
            int endSearchX = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);
            int startSearchY = Mathf.Max(0, cellY - 2);
            int endSearchY = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

            for (int x = startSearchX; x <= endSearchX; x++)
            {
                for (int y = startSearchY; y <= endSearchY; y++)
                {
                    int pointIndex = grid[x, y] - 1;
                    if (pointIndex != -1)
                    {
                        float dist = (candidate - points[pointIndex]).sqrMagnitude;
                        if (dist < radius * radius)
                            return false;
                    }
                }
            }
            return true;
            
        }
        

        return false;
    }
}
