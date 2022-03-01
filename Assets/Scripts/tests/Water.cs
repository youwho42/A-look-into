using UnityEngine;

using Unity.Collections;
using Unity.Burst;
using UnityEngine.Jobs;

using math = Unity.Mathematics.math;
using random = Unity.Mathematics.Random;
using Unity.Jobs;
using UnityEngine.Tilemaps;

[BurstCompile]
public class Water : MonoBehaviour
{

    public static Grid groundGrid;
    public static Tilemap groundMap;
    SurroundingTiles surroundingTiles;
    //the objects movement lives here
    private static NativeArray<Vector3> waterMovements;

    //the objects directions lives here
    private static NativeArray<Vector3> waterDirections;

    //the object that we move lives here
    private TransformAccessArray movementAccessArray;

    public int amountOfWater;

    public Vector3 initialDirection;

    public Transform objectPrefab;

    public float spawnDisplacement;

    public float pseudoNoise;

    public float speed;

    public Vector2 spawnBounds;

    public Vector2 minMaxSize = Vector2.up;

    private MovementUpdateJob movementUpdateJob;
    
    private JobHandle movementUpdateJobHandle;

    delegate Vector3 CanReachNextPositionDelegate(Vector3 position, Vector3 direction);

    FunctionPointer<CanReachNextPositionDelegate> fp;


    private void Start()
    {
        //BurstCompiler.CompileFunctionPointer<CanReachNextPositionDelegate>(CanReachNextPosition);

        fp = BurstCompiler.CompileFunctionPointer<CanReachNextPositionDelegate>(Water.CanReachNextPosition);
        surroundingTiles = GetComponent<SurroundingTiles>();

        //the objects movement lives here
        waterMovements = new NativeArray<Vector3>(amountOfWater, Allocator.Persistent);
        
        //the objects direction lives here
        waterDirections = new NativeArray<Vector3>(amountOfWater, Allocator.Persistent);

        //the object that we move lives here
        movementAccessArray = new TransformAccessArray(amountOfWater);



        for (int i = 0; i < amountOfWater; i++)
        {

            float distanceX = Random.Range(-spawnBounds.x / 2, spawnBounds.x / 2) + Random.Range(-spawnDisplacement, spawnDisplacement);
            float distanceY = Random.Range(-spawnBounds.y / 2, spawnBounds.y / 2) + Random.Range(-spawnDisplacement, spawnDisplacement);
            float size = Random.Range(minMaxSize.x, minMaxSize.y);

            Vector3 spawnPoint = transform.position + new Vector3(distanceX, distanceY, transform.position.z);

            Transform t = Instantiate(objectPrefab, spawnPoint, Quaternion.identity);

            t.parent = transform;
            Transform tc = t.GetChild(0);
            tc.localScale = new Vector3(size, size, 1);
            movementAccessArray.Add(t);

            for (int x = 0; x < waterDirections.Length; x++)
            {
                waterDirections[x] = initialDirection;
            }
        }
        SetGrid();
    }


    private void Update()
    {
        movementUpdateJob = new MovementUpdateJob()
        {
            functionPoint = fp,
            objectMovements = waterMovements,
            objectDirections = waterDirections,
            jobDeltaTime = Time.deltaTime,
            time = Time.time,

            speed = speed,

            seed = System.DateTimeOffset.Now.Millisecond
    };
        
        movementUpdateJobHandle = movementUpdateJob.Schedule(movementAccessArray);
        
    }

    private void LateUpdate()
    {
        movementUpdateJobHandle.Complete();
        

    }

    private void OnDestroy()
    {
        movementAccessArray.Dispose();
        
        waterMovements.Dispose();
        waterDirections.Dispose();
    }



    void SetGrid()
    {
        if (groundGrid != null)
            return;

        groundGrid = FindObjectOfType<Grid>();
        Tilemap[] maps = groundGrid.GetComponentsInChildren<Tilemap>();
        foreach (var map in maps)
        {
            if (map.gameObject.name == "GroundTiles")
            {
                groundMap = map;
            }
        }

    }



    [BurstCompile]
    public static Vector3 CanReachNextPosition(Vector3 position, Vector3 direction)
    {

        
        
        float distance = 0.05f;
        Vector3 checkPosition = (position + direction * distance);
        Vector3Int currentPosition = groundGrid.WorldToCell(position);
        Vector3Int nextTilePosition = groundGrid.WorldToCell(checkPosition);
        if (currentPosition == nextTilePosition - Vector3Int.forward)
            return direction;

        var tile = groundMap.GetTile(nextTilePosition);
        if (tile == null)
            return direction;

        Vector3Int diff = nextTilePosition - currentPosition;
        var newDirection = new Vector2(direction.y, direction.x);
        if (diff.x != 0)
            newDirection *= -1;

        return newDirection;
    

    }





    [BurstCompile]
    struct MovementUpdateJob : IJobParallelForTransform
    {

        public FunctionPointer<Water.CanReachNextPositionDelegate> functionPoint;
        public NativeArray<Vector3> objectMovements;
        public NativeArray<Vector3> objectDirections;
        public float jobDeltaTime;
        public float time;
        
        
        //public Vector3 direction;
        public float speed;

        public float seed;

        public void Execute(int i, TransformAccess transform)
        {
            

            
            transform.position += objectDirections[i] * speed * jobDeltaTime;

            objectMovements[i] = transform.position;
            objectDirections[i] = functionPoint.Invoke(objectMovements[i], objectDirections[i]);
            
        }
        

    }

}
