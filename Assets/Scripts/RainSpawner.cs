using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class RainSpawner : MonoBehaviour
{
    public Vector2 minMaxTimeToSpawn;
    float timeToSpawn;
    float nextTimeToSpawn;
    //public float speed = 0.5f;
    ObjectPooler pool;
    bool isRaining;
    public Vector2 minMaxX;
    public Vector2 minMaxY;
    AudioSource audioSource;
    bool isMoving;
    DayNightCycle dayNightCycle;
    int nextLightningStrike;
    
    public SpriteRenderer lightningRenderer;
    public Light2D lightningLight;
    public CurrentGridLocation lightningLocation;
    public AudioSource thunder;
    bool isLightning;
    private void Start()
    {
        lightningLight.enabled = false;
        dayNightCycle = DayNightCycle.instance;
        pool = GetComponent<ObjectPooler>();
        audioSource = GetComponent<AudioSource>();
        nextTimeToSpawn = dayNightCycle.tick + Random.Range(minMaxTimeToSpawn.x, minMaxTimeToSpawn.y);
        lightningRenderer.enabled = false;
        SetRandomStartPosition();
    }

    private void Update()
    {
        if (isRaining)
        {
            if (!isMoving)
            {
                nextLightningStrike = dayNightCycle.tick + Random.Range(1, 200);
                StartCoroutine(StartRainCo());
            }

            if (dayNightCycle.tick >= nextLightningStrike && !isLightning)
            {
                StartCoroutine(LightningStrikeCo());
            }

            if (Time.frameCount % 17 == 0)
            {
                SpawnObject();
            }
        }
        else
        {
            
            if (dayNightCycle.tick >= nextTimeToSpawn)
            {
                isRaining = true;
                audioSource.Play();
            }
        }
        
        
    }

    private void SpawnObject()
    {
        for (int i = 0; i < 100; i++)
        {
            GameObject go = pool.GetPooledObject();

            if (go != null)
            {
                int z = 0;
                CurrentGridLocation locationZ = go.GetComponent<CurrentGridLocation>();
                if (locationZ != null)
                {
                    z = locationZ.GetTileLocation();
                }
                
                go.transform.position = GetRandomPosition(z);
                go.SetActive(true);
                IPoolPrefab np = go.GetComponent<IPoolPrefab>();
                if (np != null)
                {
                    np.OnObjectSpawn();

                }

            }
        }
    }
    IEnumerator LightningStrikeCo()
    {
        isLightning = true;
        lightningLocation.UpdateLocationAndPosition();
        lightningRenderer.transform.position = GetRandomPosition((int)lightningRenderer.transform.position.z);
        lightningLight.enabled = true; ;
        lightningRenderer.enabled = true;
        thunder.Play();
        yield return new WaitForSeconds(0.1f);
        lightningRenderer.enabled = false;
        lightningLight.enabled = false;
        nextLightningStrike = dayNightCycle.tick + Random.Range(1, 200);
        isLightning = false;
        yield return null;
    }


    IEnumerator StartRainCo()
    {
        isMoving = true;

        

        float startTime = dayNightCycle.tick;
        float elapsedTime = 0;
        float waitTime = elapsedTime + 1000;


        while (elapsedTime < waitTime)
        {
            float posX = Mathf.Lerp(minMaxX.x, minMaxX.y, elapsedTime / waitTime);
            elapsedTime = dayNightCycle.tick - startTime;
            transform.position = new Vector3(posX, transform.position.y, transform.position.z);
            yield return null;
        }

        nextTimeToSpawn = dayNightCycle.tick + Random.Range(minMaxTimeToSpawn.x, minMaxTimeToSpawn.y);
        isMoving = false;
        isRaining = false;
        SetRandomStartPosition();
        audioSource.Stop();

        yield return null;
    }

    void SetRandomStartPosition()
    {
        transform.position = new Vector3(minMaxX.x, Random.Range(minMaxY.x, minMaxY.y), 0);
    }

    Vector3 GetRandomPosition(int z)
    {
        Vector2 temp = (Random.insideUnitCircle * 5) + (Vector2)transform.position;
        
        return new Vector3(temp.x, temp.y, z);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(new Vector3(minMaxX.x, minMaxY.x), new Vector3(minMaxX.x, minMaxY.y));
        Gizmos.DrawLine(new Vector3(minMaxX.y, minMaxY.x), new Vector3(minMaxX.y, minMaxY.y));
    }
}
