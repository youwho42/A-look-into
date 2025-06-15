using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Collections;
using Unity.Burst;
using UnityEngine.Jobs;

using random = Unity.Mathematics.Random;
using Unity.Jobs;


public class Bubbles : MonoBehaviour
{
    private NativeArray<Vector3> bubbleMovements;
    private NativeArray<Vector3> bubbleDisplacements;
    private NativeArray<Vector2> windDirections;
    private NativeArray<float> windSpeeds;

    private TransformAccessArray movementAccessArray;
    private TransformAccessArray displacementAccessArray;
    private List<Transform> shadows = new List<Transform>();

    private NativeArray<bool> resetFlags;
    private NativeArray<bool> growing;
    private NativeArray<float> randomHeight;
    private NativeArray<float> randomSize;
    private NativeArray<float> randomDisplacementSpeed;
    private NativeArray<Vector3> randomWindAngle;

    public bool bubblesActive;
    bool resettingBubbles;
    public int amountOfBubbles;
    public bool allStopped;

    public Transform objectPrefab;
    public Transform objectHolder;
    public float maxHeightZ;
    public float movementSpeed;
    public float displacementSpeed;
    public float spawnDistanceFromCenter;
    public Vector2 spawnTimeRange;

    public Vector2 minMaxSize = Vector2.up;
    public float growSpeed;

    private MovementUpdateJob movementUpdateJob;
    private DisplacementUpdateJob displacementUpdateJob;

    private JobHandle movementUpdateJobHandle;
    private JobHandle displacementUpdateJobHandle;
    public DrawZasYDisplacement baseDisplacement;
   

    private void Start()
    {
        //the object that we move lives here
        bubbleMovements = new NativeArray<Vector3>(amountOfBubbles, Allocator.Persistent);
        bubbleDisplacements = new NativeArray<Vector3>(amountOfBubbles, Allocator.Persistent);
        windDirections = new NativeArray<Vector2>(amountOfBubbles, Allocator.Persistent);
        windSpeeds = new NativeArray<float>(amountOfBubbles, Allocator.Persistent);

        //the object that we move lives here
        movementAccessArray = new TransformAccessArray(amountOfBubbles);
        displacementAccessArray = new TransformAccessArray(amountOfBubbles);


        resetFlags = new NativeArray<bool>(amountOfBubbles, Allocator.Persistent);
        growing = new NativeArray<bool>(amountOfBubbles, Allocator.Persistent);
        randomHeight = new NativeArray<float>(amountOfBubbles, Allocator.Persistent);
        randomSize = new NativeArray<float>(amountOfBubbles, Allocator.Persistent);
        randomDisplacementSpeed = new NativeArray<float>(amountOfBubbles, Allocator.Persistent);
        randomWindAngle = new NativeArray<Vector3>(amountOfBubbles, Allocator.Persistent);
        allStopped = true;
        resettingBubbles = !bubblesActive;
        for (int i = 0; i < amountOfBubbles; i++)
        {
            randomWindAngle[i] = GetRandomAngle();
            
        }
        
        for (int i = 0; i < amountOfBubbles; i++)
        {
            Vector3 spawnPoint = (transform.position + new Vector3(Random.Range(-spawnDistanceFromCenter, spawnDistanceFromCenter), Random.Range(-spawnDistanceFromCenter, spawnDistanceFromCenter),0));
            Transform t = Instantiate(objectPrefab, spawnPoint, Quaternion.identity, objectHolder);
            float size = Random.Range(minMaxSize.x, minMaxSize.y);
            
            Transform to = t.GetChild(0);
            Transform ts = t.GetChild(1);
            var anim = to.GetComponent<Animator>();
            float randomIdleStart = Random.Range(0, anim.GetCurrentAnimatorStateInfo(0).length);
            anim.speed = Random.Range(0.8f, 1.3f);
            anim.Play(0, 0, randomIdleStart);
            to.localScale = new Vector3(size, size, 1);
            to.localPosition = baseDisplacement.displacedPosition;
            ts.localScale = new Vector3(size, size, 1);
            randomHeight[i] = Random.Range(maxHeightZ / 2, maxHeightZ);
            randomSize[i] = size;
            
            randomDisplacementSpeed[i] = Random.Range(0.8f,1.2f);
            bubbleDisplacements[i] = baseDisplacement.displacedPosition;
            movementAccessArray.Add(t);
            displacementAccessArray.Add(to);
            shadows.Add(ts);
            t.gameObject.SetActive(false);
        }
    }
    Vector2 GetRandomAngle()
    {
        return new Vector2(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));
    }
    IEnumerator ResetBubbles()
    {
        for (int i = 0; i < resetFlags.Length; i++)
        {
            if (movementAccessArray[i].transform.gameObject.activeSelf)
                continue;
            Vector3 spawnPoint = (transform.position + new Vector3(Random.Range(-spawnDistanceFromCenter, spawnDistanceFromCenter), Random.Range(-spawnDistanceFromCenter, spawnDistanceFromCenter), 0));
            movementAccessArray[i].transform.position = spawnPoint;
            displacementAccessArray[i].transform.localPosition = baseDisplacement.displacedPosition;
            bubbleDisplacements[i] = baseDisplacement.displacedPosition;
            movementAccessArray[i].transform.gameObject.SetActive(true);
            displacementAccessArray[i].transform.localScale = new Vector3(0.1f, 0.1f, 1);
            growing[i] = true;


            yield return new WaitForSeconds(Random.Range(spawnTimeRange.x, spawnTimeRange.y));
        }
    }
    private void Update()
    {
        if (!bubblesActive && !allStopped)
        {
            StopAllCoroutines();
            allStopped = true;
            for (int i = 0; i < resetFlags.Length; i++)
            {
                if (resetFlags[i])
                {
                    movementAccessArray[i].transform.gameObject.SetActive(false);
                    resetFlags[i] = false;
                }

            }
            for (int i = 0; i < movementAccessArray.length; i++)
            {

                if (movementAccessArray[i].transform.gameObject.activeSelf)
                {
                    allStopped = false;
                    break;
                }

            }

            resettingBubbles = false;
        }
        if(bubblesActive)
        {
            if (!resettingBubbles)
            {
                StartCoroutine(ResetBubbles());
                resettingBubbles = true;
                allStopped = false;
            }
        }


        if (!allStopped)
        {
            for (int i = 0; i < windDirections.Length; i++)
            {
                windDirections[i] = WindManager.instance.GetWindDirectionFromPosition(movementAccessArray[i].position);
                windSpeeds[i] = WindManager.instance.GetWindMagnitude(movementAccessArray[i].position);
            }
            for (int i = 0; i < shadows.Count; i++)
            {
                shadows[i].localScale = displacementAccessArray[i].localScale;
            }
            movementUpdateJob = new MovementUpdateJob()
            {
                objectMovements = bubbleMovements,
                windDirection = windDirections,
                jobDeltaTime = Time.deltaTime,
                time = Time.time,
                basePosition = transform.position,
                speed = movementSpeed,
                resetFlags = resetFlags,
                speeds = windSpeeds,
                stopBubbles = !bubblesActive,
                dist = spawnDistanceFromCenter,
                seed = System.DateTimeOffset.Now.Millisecond,
                randomDirections = randomWindAngle
            };

            displacementUpdateJob = new DisplacementUpdateJob()
            {
                objectDisplacements = bubbleDisplacements,
                jobDeltaTime = Time.deltaTime,
                time = Time.time,
                resetFlags = resetFlags,
                speed = displacementSpeed,
                maxHeight = maxHeightZ,
                height = randomHeight,
                speeds = randomDisplacementSpeed,
                seed = System.DateTimeOffset.Now.Millisecond,
                baseDisplacement = baseDisplacement.displacedPosition,
                growing = growing,
                size = randomSize,
                growSpeed = growSpeed
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
        windDirections.Dispose();
        displacementAccessArray.Dispose();
        bubbleMovements.Dispose();
        bubbleDisplacements.Dispose();
        resetFlags.Dispose();
        randomHeight.Dispose();
        randomDisplacementSpeed.Dispose();
        windSpeeds.Dispose();
        randomWindAngle.Dispose();
        randomSize.Dispose();
        growing.Dispose();
    }

    [BurstCompile]
    struct MovementUpdateJob : IJobParallelForTransform
    {
        public NativeArray<Vector3> objectMovements;
        public NativeArray<Vector2> windDirection;
        public float jobDeltaTime;
        public float time;
        public Vector3 basePosition;
        public float speed;
        public NativeArray<bool> resetFlags;
        public NativeArray<float> speeds;
        public NativeArray<Vector3> randomDirections;
        public float seed;
        public float dist;
        
        public bool stopBubbles;
        
        public void Execute(int i, TransformAccess transform)
        {
            
            random randomGen = new random((uint)(i * time + 1 + seed));

            
            transform.position += ((Vector3)windDirection[i] + randomDirections[i]).normalized * speed * speeds[i] * jobDeltaTime;
            objectMovements[i] = transform.position;
            
            
            
            
            
            // Reset position here
            if (resetFlags[i] && !stopBubbles)
            {
                Vector3 spawnPoint = new Vector3(randomGen.NextFloat(-dist, dist), randomGen.NextFloat(-dist, dist), 0);
                transform.position = basePosition + spawnPoint;
                objectMovements[i] = transform.position;
                
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
        public NativeArray<float> height;
        public NativeArray<float> size;
        public NativeArray<float> speeds;
        public float positionZ;
        public Vector3 displacedPosition;
        public Vector3 baseDisplacement;
        public float seed;
        public NativeArray<bool> growing;
        public float growSpeed;

        public void Execute(int i, TransformAccess transform)
        {
            random randomGen = new random((uint)(i * time + 1 + seed));


            if (growing[i])
            {
                transform.localScale += new Vector3(growSpeed, growSpeed, 0);
                if (transform.localScale.x >= size[i])
                    growing[i] = false;
            }
            
            Vector3 currentVelocity = objectDisplacements[i];
            positionZ = currentVelocity.z;
            positionZ += gravity * speed * speeds[i] * jobDeltaTime;
            displacedPosition = new Vector3(transform.localPosition.x, GlobalSettings.SpriteDisplacementY * positionZ, positionZ);
            transform.localPosition = displacedPosition;
            objectDisplacements[i] = transform.localPosition;
            
            
            Vector3 currentPosition = transform.localPosition;

            //Reset position here
            if (currentPosition.y >= GlobalSettings.SpriteDisplacementY * height[i])
            {
                height[i] = randomGen.NextFloat(maxHeight / 2, maxHeight);
                speeds[i] = randomGen.NextFloat(0.8f, 1.2f);
                Vector3 spawnPoint = baseDisplacement;
                transform.localPosition = spawnPoint;
                objectDisplacements[i] = spawnPoint;
                resetFlags[i] = true;
                transform.localScale = new Vector3(0.1f, 0.1f, 1);
                growing[i] = true;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnDistanceFromCenter);
    }
}
