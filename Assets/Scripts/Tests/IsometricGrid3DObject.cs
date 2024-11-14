using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class IsometricGrid3DObject : MonoBehaviour
{
    private float angle = 30f;
    public float size { get; private set; }
    public float xComponent { get; private set; }
    public float yComponent { get; private set; }
    public float zComponent { get; private set; }

    public IsometricSandObject sandObject;
    private NativeHashMap<int3, int> allGridPositions;
    private List<IsometricSandObject> sandObjectInstances;
    private float disp = GlobalSettings.SpriteDisplacementY;
    private float halfSize;

    private void Start()
    {
        SetIsometric();
        allGridPositions = new NativeHashMap<int3, int>(100, Allocator.Persistent);
        sandObjectInstances = new List<IsometricSandObject>();
        InvokeRepeating("AddGrain", 0.0f, 0.5f);
    }

    private void OnDestroy()
    {
        if (allGridPositions.IsCreated)
            allGridPositions.Dispose();
    }

    private void FixedUpdate()
    {
        UpdateGrains();
    }

    void AddGrain()
    {
        CreateGrain(new int3(6, -30, 20));
    }

    void CreateGrain(int3 pos)
    {
        float zLevel = zComponent * pos.z;
        var go = Instantiate(sandObject, GetWorldPosition(pos.x, pos.y), Quaternion.identity, transform);
        go.level = pos.z;

        go.sandGrain.localPosition = new Vector3(0, disp * zLevel, zLevel);
        sandObjectInstances.Add(go);

        int index = sandObjectInstances.Count - 1;
        allGridPositions.TryAdd(pos, index); // Use the index instead of the object reference
    }

    private void UpdateGrains()
    {
        var updatedPositions = new NativeHashMap<int3, int>(allGridPositions.Count(), Allocator.TempJob);

        // Initialize a Unity.Mathematics.Random instance with a unique seed
        var randomSeed = new Unity.Mathematics.Random((uint)(UnityEngine.Random.Range(1, int.MaxValue)));

        var job = new UpdateGrainsJob
        {
            allGridPositions = allGridPositions,
            updatedPositions = updatedPositions,
            zComponent = zComponent,
            disp = disp,
            random = randomSeed // Pass the random generator to the job
        };

        JobHandle handle = job.Schedule();
        handle.Complete();

        foreach (var kvp in updatedPositions)
        {
            SetGrainPosition(kvp.Key, sandObjectInstances[kvp.Value]);
        }

        allGridPositions.Clear();
        foreach (var kvp in updatedPositions)
        {
            allGridPositions.TryAdd(kvp.Key, kvp.Value);
        }

        updatedPositions.Dispose();
    }

    private void SetGrainPosition(int3 pos, IsometricSandObject obj)
    {
        float zLevel = zComponent * pos.z;
        if (obj.sandGrain.localPosition.z == zLevel) return;

        obj.transform.position = GetWorldPosition(pos.x, pos.y);
        obj.level = pos.z;
        obj.iterations++;
        obj.sandGrain.localPosition = new Vector3(0, disp * zLevel, zLevel);
    }

    [BurstCompile]
    private struct UpdateGrainsJob : IJob
    {
        [ReadOnly] public NativeHashMap<int3, int> allGridPositions;
        public NativeHashMap<int3, int> updatedPositions;
        public float zComponent;
        public float disp;

        public Unity.Mathematics.Random random; // Random instance compatible with Burst

        public void Execute()
        {
            foreach (var grain in allGridPositions)
            {
                int3 currentPosition = grain.Key;
                int index = grain.Value;

                int3 nextPosition = GetSurroundingSpaces(currentPosition);
                if (!nextPosition.Equals(new int3(-1, -1, -1)) && !nextPosition.Equals(currentPosition))
                {
                    updatedPositions[nextPosition] = index;
                }
                else
                {
                    updatedPositions[currentPosition] = index;
                }
            }
        }

        private int3 GetSurroundingSpaces(int3 pos)
        {
            if (pos.z == 0)
                return new int3(-1, -1, -1);

            int3 belowPos = new int3(pos.x, pos.y, pos.z - 1);
            if (!allGridPositions.ContainsKey(belowPos))
            {
                return belowPos;
            }

            NativeList<int3> results = new NativeList<int3>(Allocator.Temp);
            for (int x = -2; x <= 2; x++)
            {
                for (int y = -2; y <= 2; y++)
                {
                    if (x == 0 || y == 0) continue;

                    int3 p = new int3(pos.x + x, pos.y + y, pos.z - 1);
                    if (!allGridPositions.ContainsKey(p))
                    {
                        results.Add(p);
                    }
                }
            }

            int3 resultPosition = new int3(-1, -1, -1);
            if (results.Length > 0)
            {
                int randomIndex = random.NextInt(0, results.Length);
                resultPosition = results[randomIndex];
            }

            results.Dispose();
            return resultPosition;
        }
    }

    private void SetIsometric()
    {
        size = 0.005f;
        halfSize = size * 0.5f;
        float angleRad = angle * Mathf.Deg2Rad;
        xComponent = Mathf.Cos(angleRad);
        yComponent = Mathf.Sin(angleRad);
        zComponent = 0.02f;
    }

    public Vector3 GetWorldPosition(float x, float y)
    {
        float worldX = (x - y) * size * xComponent;
        float worldY = (x + y) * size * yComponent;
        float rx = UnityEngine.Random.Range(-halfSize, halfSize);
        float ry = UnityEngine.Random.Range(-halfSize, halfSize);
        return new Vector3(worldX + rx, worldY + ry, 2);
    }

    public int3 GetGridLocation(Vector3 position)
    {
        int x = Mathf.RoundToInt((position.x / xComponent + position.y / yComponent) / size);
        int y = Mathf.RoundToInt((position.y / yComponent - position.x / xComponent) / size);
        int z = Mathf.RoundToInt(position.z / zComponent);
        return new int3(x, y, z);
    }
}



//using System.Collections.Generic;
//using UnityEngine;

//public class IsometricGrid3DObject : MonoBehaviour
//{
//    private float angle = 30f; // The angle of the isometric grid
//    public float size { get; private set; } // The size of each cell in the grid

//    public float xComponent { get; private set; } // The x component of the angle
//    public float yComponent { get; private set; } // The y component of the angle
//    public float zComponent { get; private set; } // The z component of the height layers

//    public IsometricSandObject sandObject;
//    Dictionary<Vector3Int, IsometricSandObject> allGridPositions = new Dictionary<Vector3Int, IsometricSandObject>();
//    float disp = GlobalSettings.SpriteDisplacementY;
//    float halfSize;
//    private void Start()
//    {
//        SetIsometric();

//        InvokeRepeating("AddGrain", 0.0f, 0.5f);
//    }
//    private void FixedUpdate()
//    {
//        UpdateGrains();
//    }
//    void AddGrain()
//    {
//        CreateGrain(new Vector3Int(6, -30, 20));
//    }
//    void CreateGrain(Vector3Int pos)
//    {
//        float zLevel = zComponent * pos.z;
//        var go = Instantiate(sandObject, GetWorldPosition(pos.x, pos.y), Quaternion.identity, transform);
//        go.level = pos.z;

//        go.sandGrain.localPosition = new Vector3(0, disp * zLevel, zLevel);
//        allGridPositions.Add(pos, go);
//    }

//    void SetGrainPosition(Vector3Int pos, IsometricSandObject obj)
//    {

//        float zLevel = zComponent * pos.z;
//        if (obj.sandGrain.localPosition.z == zLevel)
//            return;
//        obj.transform.position = GetWorldPosition(pos.x, pos.y);
//        obj.level = pos.z;
//        obj.iterations++;
//        obj.sandGrain.localPosition = new Vector3(0, disp * zLevel, zLevel);
//    }


//    private void UpdateGrains()
//    {
//        // Temporary dictionary to hold new positions after updates
//        Dictionary<Vector3Int, IsometricSandObject> updatedPositions = new Dictionary<Vector3Int, IsometricSandObject>();

//        foreach (var grain in allGridPositions)
//        {
//            Vector3Int currentPosition = grain.Key;
//            IsometricSandObject grainObject = grain.Value;

//            // Determine next position if there's an open space around
//            Vector3Int nextPosition = GetSurroundingSpaces(currentPosition);
//            if (nextPosition != -Vector3Int.one && nextPosition != currentPosition)
//            {
//                // Update the position if it's different
//                updatedPositions[nextPosition] = grainObject;
//            }
//            else
//            {
//                // Keep the grain in the same position if no update is necessary
//                updatedPositions[currentPosition] = grainObject;
//            }
//        }

//        // Apply the positions to `allGridPositions` and update grain visual positions
//        foreach (var grain in updatedPositions)
//        {
//            SetGrainPosition(grain.Key, grain.Value);
//        }

//        // Replace the old positions dictionary with updated positions
//        allGridPositions = updatedPositions;
//    }


//public Vector3Int GetSurroundingSpaces(Vector3Int pos)
//{
//    if (pos.z == 0)
//        return -Vector3Int.one;

//    if (!allGridPositions.ContainsKey(new Vector3Int(pos.x, pos.y, pos.z - 1)))
//    {
//        // position open
//        return new Vector3Int(pos.x, pos.y, pos.z - 1);
//    }
//    List<Vector3Int> results = new List<Vector3Int>();
//    for (int x = -2; x < 3; x++)
//    {
//        for (int y = -2; y < 3; y++)
//        {
//            if (x == 0 || y == 0)
//                continue;

//            Vector3Int p = new Vector3Int(pos.x + x, pos.y + y, pos.z - 1);
//            if (!allGridPositions.ContainsKey(p))
//            {
//                results.Add(p);
//            }
//        }
//    }
//    if (results.Count == 0)
//        return -Vector3Int.one;
//    int r = Random.Range(0, results.Count);
//    return results[r];
//}
//    private void SetIsometric()
//    {
//        size = 0.005f;
//        halfSize = size * 0.5f;
//        float angleRad = angle * Mathf.Deg2Rad;
//        xComponent = Mathf.Cos(angleRad);
//        yComponent = Mathf.Sin(angleRad);
//        zComponent = 0.02f; // Adjust this factor as needed to control height scaling
//    }

//    // Returns the real-world position on the grid, now with z input for height layers
//    public Vector3 GetWorldPosition(float x, float y)
//    {

//        float worldX = (x - y) * size * xComponent;
//        float worldY = (x + y) * size * yComponent;
//        float rx = Random.Range(-halfSize, halfSize);
//        float ry = Random.Range(-halfSize, halfSize);
//        // Return the 3D Vector position with the height component
//        return new Vector3(worldX + rx, worldY + ry, 2);
//    }

//    // Returns the int x, y, z position on the grid
//    public Vector3Int GetGridLocation(Vector3 position)
//    {

//        int x = Mathf.RoundToInt((position.x / xComponent + position.y / yComponent) / size);
//        int y = Mathf.RoundToInt((position.y / yComponent - position.x / xComponent) / size);
//        int z = Mathf.RoundToInt(position.z / zComponent); // Convert world Z position to grid Z level

//        return new Vector3Int(x, y, z);
//    }


//}
