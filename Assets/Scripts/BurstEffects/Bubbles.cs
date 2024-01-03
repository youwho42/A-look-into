using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Collections;
using Unity.Burst;
using UnityEngine.Jobs;

using math = Unity.Mathematics.math;
using random = Unity.Mathematics.Random;
using Unity.Jobs;
using UnityEngine.Tilemaps;


public class Bubbles : MonoBehaviour
{
    private NativeArray<Vector3> bubbleMovements;
    private NativeArray<Vector3> bubbleDisplacements;

    private TransformAccessArray movementAccessArray;
    private TransformAccessArray displacementAccessArray;

    private NativeArray<bool> resetFlags;

    public int amountOfBubbles;
    

    public Transform objectPrefab;

    public float maxHeightZ;

    
    public float pseudoNoise;

    public float  movementSpeed;
    public float  displacementSpeed;

    public float spawnDistanceFromCenter;

    public Vector2 minMaxSize = Vector2.up;

    public Vector2 generalDirection;

    
    DisableOnInvisible disableOnInvisible;

    private MovementUpdateJob movementUpdateJob;
    private DisplacementUpdateJob displacementUpdateJob;

    private JobHandle movementUpdateJobHandle;
    private JobHandle displacementUpdateJobHandle;

    private IEnumerator Start()
    {
        //the object that we move lives here
        bubbleMovements = new NativeArray<Vector3>(amountOfBubbles, Allocator.Persistent);
        bubbleDisplacements = new NativeArray<Vector3>(amountOfBubbles, Allocator.Persistent);

        //the object that we move lives here
        movementAccessArray = new TransformAccessArray(amountOfBubbles);
        displacementAccessArray = new TransformAccessArray(amountOfBubbles);

        resetFlags = new NativeArray<bool>(amountOfBubbles, Allocator.Persistent);


        disableOnInvisible = GetComponentInChildren<DisableOnInvisible>();


        yield return new WaitForSeconds(0.2f);
        // need to spawn the bubbles here

        for (int i = 0; i < amountOfBubbles; i++)
        {
            Vector3 spawnPoint = (transform.position + new Vector3(Random.Range(-spawnDistanceFromCenter, spawnDistanceFromCenter), Random.Range(-spawnDistanceFromCenter, spawnDistanceFromCenter),0));

            Transform t = Instantiate(objectPrefab, spawnPoint, Quaternion.identity, transform);
            float size = Random.Range(minMaxSize.x, minMaxSize.y);
            
            Transform tc = t.GetChild(0);
            var anim = tc.GetComponent<Animator>();
            float randomIdleStart = Random.Range(0, anim.GetCurrentAnimatorStateInfo(0).length);
            anim.speed = Random.Range(0.8f, 1.3f);
            anim.Play(0, 0, randomIdleStart);
            t.localScale = new Vector3(size, size, 1);
            
            movementAccessArray.Add(t);
            displacementAccessArray.Add(t.GetChild(0));
            yield return new WaitForSeconds(Random.Range(0.8f, 2.3f));
        }
        yield return null;
        
    }
    private void Update()
    {
        if (disableOnInvisible.isVisible)
        {
            
            movementUpdateJob = new MovementUpdateJob()
            {
                objectMovements = bubbleMovements,
                jobDeltaTime = Time.deltaTime,
                time = Time.time,
                basePosition = transform.position,
                direction = generalDirection.normalized,
                speed = movementSpeed,
                resetFlags = resetFlags,
                dist = spawnDistanceFromCenter,
                seed = System.DateTimeOffset.Now.Millisecond
            };


            displacementUpdateJob = new DisplacementUpdateJob()
            {

                objectDisplacements = bubbleDisplacements,
                jobDeltaTime = Time.deltaTime,
                time = Time.time,
                resetFlags = resetFlags,
                speed = displacementSpeed,
                maxHeight = maxHeightZ,
                sizeMinMax = minMaxSize,
                noise = pseudoNoise,
                seed = System.DateTimeOffset.Now.Millisecond
            };

            // Schedule the displacement job first
            displacementUpdateJobHandle = displacementUpdateJob.Schedule(displacementAccessArray);

            // Schedule the movement job with a dependency on the displacement job
            movementUpdateJobHandle = movementUpdateJob.Schedule(movementAccessArray, displacementUpdateJobHandle);
            
        }
    }

    private void LateUpdate()
    {
        movementUpdateJobHandle.Complete();
        displacementUpdateJobHandle.Complete();
    }

    private void OnDestroy()
    {
        movementAccessArray.Dispose();
        displacementAccessArray.Dispose();
        bubbleMovements.Dispose();
        bubbleDisplacements.Dispose();
        resetFlags.Dispose();
    }

    [BurstCompile]
    struct MovementUpdateJob : IJobParallelForTransform
    {
        public NativeArray<Vector3> objectMovements;
        public float jobDeltaTime;
        public float time;
        public Vector3 basePosition;
        public Vector3 direction;
        public float speed;
        public NativeArray<bool> resetFlags;
        public float seed;
        public float dist;

        public void Execute(int i, TransformAccess transform)
        {


            random randomGen = new random((uint)(i * time + 1 + seed));

            transform.position += direction * speed * jobDeltaTime;

            objectMovements[i] = transform.position;

            // Reset position here

            if (resetFlags[i])
            {
                Vector3 spawnPoint = new Vector3(randomGen.NextFloat(-dist, dist), randomGen.NextFloat(-dist, dist), 0);
                transform.position = basePosition + spawnPoint;
                objectMovements[i] = transform.position;

                // Reset the flag after performing the reset
                resetFlags[i] = false;
            }

            

        }

    }

    [BurstCompile]
    struct DisplacementUpdateJob : IJobParallelForTransform
    {
        
        public NativeArray<Vector3> objectDisplacements;
        public float jobDeltaTime;
        public float time;
        public NativeArray<bool> resetFlags;
        const float gravity = 10f;
        public float speed;
        public float maxHeight;
        public Vector2 sizeMinMax;
        public float positionZ;
        const float displacementY = 0.2990625f;
        public Vector3 displacedPosition;
        public float noise;
        public float seed;

        public void Execute(int i, TransformAccess transform)
        {

            Vector3 currentVelocity = objectDisplacements[i];
            positionZ = currentVelocity.z;

            random randomGen = new random((uint)(i * time + 1 + seed));

            positionZ += gravity * speed * jobDeltaTime;
            displacedPosition = new Vector3(transform.localPosition.x, displacementY * positionZ, positionZ);
            transform.localPosition = displacedPosition;

            objectDisplacements[i] = transform.localPosition;
            Vector3 currentPosition = transform.localPosition;

            //Reset position here

            if (currentPosition.y >= displacementY * maxHeight)
            {

                Vector3 spawnPoint = new Vector3(0, 0, 0);
                transform.localPosition = spawnPoint;
                objectDisplacements[i] = spawnPoint;
                resetFlags[i] = true;
            }


        }
    }

}
