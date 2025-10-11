
using System.Collections.Generic;
using UnityEngine;

using Unity.Collections;
using Unity.Burst;
using UnityEngine.Jobs;

using math = Unity.Mathematics.math;
using random = Unity.Mathematics.Random;
using Unity.Jobs;
using UnityEngine.Tilemaps;

public class PurpleFireSheet : MonoBehaviour
{
    public int amountOfDrops;
    private NativeArray<Vector3> dropVelocities;

    private TransformAccessArray transformAccessArray;
    private List<Transform> allDrops = new List<Transform>();
    
    public float maxHeight;
    public Transform objectPrefab;

    public float fallSpeed;
    private PositionUpdateJob positionUpdateJob;

    private JobHandle positionUpdateJobHandle;
    public bool active;
    bool dropsActive;
    public Transform posA, posB;

    DisableOnInvisible disableOnInvisible;

    private void Start()
    {
        dropVelocities = new NativeArray<Vector3>(amountOfDrops, Allocator.Persistent);
        transformAccessArray = new TransformAccessArray(amountOfDrops);
        disableOnInvisible = GetComponentInChildren<DisableOnInvisible>();
        //transform.position = SetRainStormPosition();
        SpawnDrops();

    }

    

    private void FixedUpdate()
    {
        if (active && disableOnInvisible.isVisible)
        {
            if (!dropsActive)
                EnableDrops();

            positionUpdateJob = new PositionUpdateJob()
            {
                objectVelocities = dropVelocities,
                jobDeltaTime = Time.fixedDeltaTime,

                time = Time.time,
                speed = fallSpeed,

                
                height = maxHeight,
                seed = System.DateTimeOffset.Now.Millisecond
            };

            positionUpdateJobHandle = positionUpdateJob.Schedule(transformAccessArray);
        }
        else
        {
            if (dropsActive)
            {
                DisableDrops();
                //transform.position = SetRainStormPosition();
            }

        }


    }

    private void LateUpdate()
    {
        positionUpdateJobHandle.Complete();
    }


    private void OnDestroy()
    {
        positionUpdateJobHandle.Complete();
        transformAccessArray.Dispose();
        dropVelocities.Dispose();
    }
    void SpawnDrops()
    {
        for (int i = 0; i < amountOfDrops; i++)
        {
            Transform t = Instantiate(objectPrefab, transform.position, Quaternion.identity);
            t.parent = transform;
            allDrops.Add(t);
            Transform tChild = t.GetChild(0);
            SetDisplacement(tChild, i);
            
            transformAccessArray.Add(tChild);
            t.gameObject.SetActive(false);
        }
    }
    
    void SetDropPositions()
    {
        for (int i = 0; i < allDrops.Count; i++)
        {
            Vector3 spawnPoint = GetRandomVector3Between(posA.position, posB.position) - new Vector3(0,0,0.5f);
            allDrops[i].position = spawnPoint;
        }
    }
    public Vector3 GetRandomVector3Between(Vector3 min, Vector3 max)
    {
        float r = Random.Range(0.0f, 1.0f);
        return Vector3.Lerp(min, max, r);
        
    }
    void SetDisplacement(Transform t, int i)
    {
        float displacementY = 0.27808595f;
        float distanceZ = Random.Range(0.0f, maxHeight);
        t.localPosition = new Vector3(0, displacementY * distanceZ, distanceZ);
        dropVelocities[i] = t.localPosition;
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

    private void OnDrawGizmos()
    {
        if(posA != null && posB != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(posA.position, posB.position);
        }
    }



    [BurstCompile]
    struct PositionUpdateJob : IJobParallelForTransform
    {
        public NativeArray<Vector3> objectVelocities;

        

        public float height;
        public float jobDeltaTime;
        public float time;
        public float speed;
        const float groundPlacement = 0.001f;
        const float gravity = 20f;

        public float positionZ;
        const float displacementY = 0.27808595f;
        public Vector3 displacedPosition;

        public float seed;

        public void Execute(int i, TransformAccess transform)
        {
            
            Vector3 currentVelocity = objectVelocities[i];
            positionZ = currentVelocity.z;

            random randomGen = new random((uint)(i * time + 1 + seed));

           
            positionZ += gravity * jobDeltaTime * speed * randomGen.NextFloat(0.1f, 0.3f);
            displacedPosition = new Vector3(0, displacementY * positionZ, positionZ);
            transform.localPosition = displacedPosition;
            
            objectVelocities[i] = transform.localPosition;
            Vector3 currentPosition = transform.localPosition;


            
            if (currentPosition.y >= displacementY*height)
            {

                Vector3 spawnPoint = new Vector3(0, 0, 0);
                transform.localPosition = spawnPoint;
                objectVelocities[i] = spawnPoint;

            }
        }

    }
}
