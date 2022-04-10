
using System.Collections.Generic;
using UnityEngine;

using Unity.Collections;
using Unity.Burst;
using UnityEngine.Jobs;

using math = Unity.Mathematics.math;
using random = Unity.Mathematics.Random;
using Unity.Jobs;
using UnityEngine.Tilemaps;

public class RainGenerator : MonoBehaviour
{
    public int amountOfRainDrops;
    private NativeArray<Vector3> rainDropVelocities;
    
    private TransformAccessArray transformAccessArray;
    private List<Transform> allDrops = new List<Transform>();
    public Vector2 spawnBounds;
    public float spawnHeight;
    public Transform objectPrefab;
    
    public float fallSpeed;
    private PositionUpdateJob positionUpdateJob;

    private JobHandle positionUpdateJobHandle;
    public bool active;
    bool dropsActive;

    public Tilemap groundMap;

    private void Start()
    {
        rainDropVelocities = new NativeArray<Vector3>(amountOfRainDrops, Allocator.Persistent);
        transformAccessArray = new TransformAccessArray(amountOfRainDrops);

        transform.position = SetRainStormPosition();
        SpawnDrops();
        
    }

    private void Update()
    {
        if (active)
        {
            if (!dropsActive)
                EnableDrops();

            positionUpdateJob = new PositionUpdateJob()
            {
                objectVelocities = rainDropVelocities,
                jobDeltaTime = Time.deltaTime,

                time = Time.time,
                speed = fallSpeed,

                bounds = spawnBounds,
                height = spawnHeight,
                seed = System.DateTimeOffset.Now.Millisecond
            };

            positionUpdateJobHandle = positionUpdateJob.Schedule(transformAccessArray);
        }
        else
        {
            if (dropsActive)
            {
                DisableDrops();
                transform.position = SetRainStormPosition();
            }
                
        }
        

    }

    private void LateUpdate()
    {
        positionUpdateJobHandle.Complete();
    }
  

    private void OnDestroy()
    {
        transformAccessArray.Dispose();
        rainDropVelocities.Dispose();
    }
    void SpawnDrops()
    {
        for (int i = 0; i < amountOfRainDrops; i++)
        {
            Transform t = Instantiate(objectPrefab, transform.position, Quaternion.identity);
            t.parent = transform;
            allDrops.Add(t);
            transformAccessArray.Add(t.GetChild(0));
            t.gameObject.SetActive(false);
        }
    }
    void SetDropPositions()
    {
        for (int i = 0; i < allDrops.Count; i++)
        {
            float distanceX = Random.Range(-spawnBounds.x / 2, spawnBounds.x / 2);
            float distanceY = Random.Range(-spawnBounds.y / 2, spawnBounds.y / 2);
            Vector3 spawnPoint = transform.position + new Vector3(distanceX, distanceY, 1);
            allDrops[i].position = spawnPoint;
        }
    }

    Vector3 SetRainStormPosition()
    {
        Vector3Int rand = new Vector3Int(UnityEngine.Random.Range(groundMap.cellBounds.xMin + 8, groundMap.cellBounds.xMax - 10), UnityEngine.Random.Range(groundMap.cellBounds.yMin + 35, groundMap.cellBounds.yMax - 6), 0);
        var t = groundMap.GetCellCenterWorld(rand);
        return t;
    }


    void DisableDrops()
    {
        dropsActive = false;
        for (int i = 0; i < allDrops.Count; i++)
            allDrops[i].gameObject.SetActive(false);
    }
    void EnableDrops()
    {
        SetDropPositions();
        dropsActive = true;
        for (int i = 0; i < allDrops.Count; i++)
            allDrops[i].gameObject.SetActive(true);
    }




    [BurstCompile]
    struct PositionUpdateJob : IJobParallelForTransform
    {
        public NativeArray<Vector3> objectVelocities;
        
        public Vector3 bounds;
        
        public float height;
        public float jobDeltaTime;
        public float time;
        public float speed;
        const float groundPlacement = 0.001f;
        const float gravity = -20f;

        public float positionZ;
        const float displacementY = 0.27808595f;
        public Vector3 displacedPosition;

        public float seed;

        public void Execute(int i, TransformAccess transform)
        {
            // 1
            Vector3 currentVelocity = objectVelocities[i];
            positionZ = currentVelocity.z;

            random randomGen = new random((uint)(i * time + 1 + seed));

            // 3
            positionZ += gravity * jobDeltaTime * speed * randomGen.NextFloat(0.1f, 0.3f);
            displacedPosition = new Vector3(transform.localPosition.x, displacementY * positionZ, positionZ);
            transform.localPosition = displacedPosition;

            objectVelocities[i] = transform.localPosition;
            Vector3 currentPosition = transform.localPosition;
            

            // 1
            if (currentPosition.y <= groundPlacement)
            {
                
                Vector3 spawnPoint = new Vector3(0, displacementY * height, height);
                transform.localPosition = spawnPoint;
                objectVelocities[i] = spawnPoint;
                
            }
        }
            
    }
}
