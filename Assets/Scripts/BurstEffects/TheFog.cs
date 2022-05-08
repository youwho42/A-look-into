
using System.Collections.Generic;
using UnityEngine;

using Unity.Collections;
using Unity.Burst;
using UnityEngine.Jobs;

using math = Unity.Mathematics.math;
using random = Unity.Mathematics.Random;
using Unity.Jobs;

public class TheFog : MonoBehaviour
{
    
    private NativeArray<Vector3> fogMovements;
    private NativeArray<Vector3> fogDisplacements;

    private TransformAccessArray movementAccessArray;
    private TransformAccessArray displacementAccessArray;

    public int amountOfFog;

    public Transform objectPrefab;

    public float maxHeightZ;

    public float spawnDisplacement;

    public float sinFrequency;

    public float pseudoNoise;

    public Vector2 speeds;

    public Vector2 spawnBounds;
    
    public Vector2 minMaxSize = Vector2.up;

    public Vector2 generalDirection;
    

    //private MovementUpdateJob movementUpdateJob;
    private DisplacementUpdateJob displacementUpdateJob;

    //private JobHandle movementUpdateJobHandle;
    private JobHandle displacementUpdateJobHandle;

    private void Start()
    {
        //the object that we move lives here
        //fogMovements = new NativeArray<Vector3>(amountOfFog, Allocator.Persistent);
        fogDisplacements = new NativeArray<Vector3>(amountOfFog, Allocator.Persistent);

        //the object that we move lives here
        //movementAccessArray = new TransformAccessArray(amountOfFog); 
        displacementAccessArray = new TransformAccessArray(amountOfFog);



        for (int i = 0; i < amountOfFog; i++)
        {

            float distanceX = Random.Range(-spawnBounds.x / 2, spawnBounds.x / 2) + Random.Range(-spawnDisplacement, spawnDisplacement);
            float distanceY = Random.Range(-spawnBounds.y / 2, spawnBounds.y / 2) + Random.Range(-spawnDisplacement, spawnDisplacement);
            float size = Random.Range(minMaxSize.x, minMaxSize.y);
            

            Vector3 spawnPoint = transform.position + new Vector3(distanceX, distanceY, 1);

            Transform t = Instantiate(objectPrefab, spawnPoint, Quaternion.identity);

            t.parent = transform;
            Transform tc = t.GetChild(0);
            tc.localScale = new Vector3(size, size, 1);
            float z = Random.Range(0, maxHeightZ / 2);
            fogDisplacements[i] = new Vector3(transform.localPosition.x, 0.27808595f * z, z);
            //movementAccessArray.Add(t);
            displacementAccessArray.Add(t.GetChild(0));
        }
    }

    private void Update()
    {
        /*movementUpdateJob = new MovementUpdateJob()
        {
            objectMovements = fogMovements,
            jobDeltaTime = Time.deltaTime,
            time = Time.time,

            direction = generalDirection,
            speed = speeds.x,
            
            seed = System.DateTimeOffset.Now.Millisecond
        };
*/
        
        displacementUpdateJob = new DisplacementUpdateJob()
        {
            objectDisplacements = fogDisplacements,
            jobDeltaTime = Time.deltaTime,
            time = Time.time,

            frequency = sinFrequency,
            speed = speeds.y + Random.Range(-0.01f,0.01f),
            maxHeight = maxHeightZ,
            sizeMinMax = minMaxSize,
            noise = pseudoNoise,
            seed = System.DateTimeOffset.Now.Millisecond
        };

        //movementUpdateJobHandle = movementUpdateJob.Schedule(movementAccessArray);
        displacementUpdateJobHandle = displacementUpdateJob.Schedule(displacementAccessArray);
        
    }

    private void LateUpdate()
    {
        //movementUpdateJobHandle.Complete();
        displacementUpdateJobHandle.Complete();
    }

    private void OnDestroy()
    {
        //movementAccessArray.Dispose();
        displacementAccessArray.Dispose();
        //fogMovements.Dispose();
        fogDisplacements.Dispose();
    }






    /*[BurstCompile]
    struct MovementUpdateJob : IJobParallelForTransform
    {
        public NativeArray<Vector3> objectMovements;
        public float jobDeltaTime;
        public float time;

        public Vector3 direction;
        public float speed;
        
        public float seed;
        
        public void Execute(int i, TransformAccess transform)
        {
            
            
            random randomGen = new random((uint)(i * time + 1 + seed));

            transform.position += direction * speed * jobDeltaTime * randomGen.NextFloat(0.3f, 1.0f);
            
            objectMovements[i] = transform.position;
            
        }

    }*/

    [BurstCompile]
    struct DisplacementUpdateJob : IJobParallelForTransform
    {
        public NativeArray<Vector3> objectDisplacements;
        public float jobDeltaTime;
        public float time;

        const float gravity = 10f;
        public float speed;
        public float maxHeight;
        public Vector2 sizeMinMax;
        public float positionZ;
        const float displacementY = 0.27808595f;
        public Vector3 displacedPosition;
        public float frequency;
        public float noise;
        public float seed;

        public void Execute(int i, TransformAccess transform)
        {
            
            Vector3 currentVelocity = objectDisplacements[i];
            positionZ = currentVelocity.z;

            random randomGen = new random((uint)(i * time + 1 + seed));
            
            var sin = Mathf.Sin(time * frequency * randomGen.NextFloat(-noise, noise));
            positionZ += gravity * sin * speed * jobDeltaTime;
            displacedPosition = new Vector3(transform.localPosition.x, displacementY * positionZ, positionZ);
            transform.localPosition = displacedPosition;

            objectDisplacements[i] = transform.localPosition;
            Vector3 currentPosition = transform.localPosition;
        }
    }
}
