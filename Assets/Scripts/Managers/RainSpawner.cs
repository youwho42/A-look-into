using System.Collections;
using UnityEngine;


public class RainSpawner : MonoBehaviour
{
    public Vector2Int minMaxTimeToSpawn;
    public Vector2Int minMaxTimeToLast;
    int nextTimeToSpawn;
    int timeToLast;
    bool isRaining;
    AudioSource audioSource;
    RainGenerator rainGenerator;
    DayNightCycle dayNightCycle;
    int nextLightningStrike;
    
    public SpriteRenderer lightningRenderer;
    public UnityEngine.Rendering.Universal.Light2D lightningLight;
    public CurrentGridLocation lightningLocation;
    public AudioSource thunder;
    bool isLightning;

    private void Start()
    {
        lightningLight.enabled = false;
        dayNightCycle = DayNightCycle.instance;
        audioSource = GetComponent<AudioSource>();
        rainGenerator = GetComponent<RainGenerator>();
        nextTimeToSpawn = dayNightCycle.tick + Random.Range(minMaxTimeToSpawn.x, minMaxTimeToSpawn.y);
        timeToLast = dayNightCycle.tick + Random.Range(minMaxTimeToLast.x, minMaxTimeToLast.y);
        lightningRenderer.enabled = false;
        rainGenerator.active = false;
    }

    private void Update()
    {
        if (isRaining)
        {
            
            if(nextLightningStrike == 0)
                nextLightningStrike = dayNightCycle.tick + Random.Range(1, 100);

            if (dayNightCycle.tick >= nextLightningStrike && !isLightning)
                StartCoroutine(LightningStrikeCo());

            if (dayNightCycle.tick >= timeToLast)
            {
                isRaining = false;
                audioSource.Stop();
                nextTimeToSpawn = dayNightCycle.tick + Random.Range(minMaxTimeToSpawn.x, minMaxTimeToSpawn.y);
                rainGenerator.active = false;
            }
        }
        else
        {
            if (dayNightCycle.tick >= nextTimeToSpawn)
            {
                isRaining = true;
                audioSource.Play();
                timeToLast = dayNightCycle.tick + Random.Range(minMaxTimeToLast.x, minMaxTimeToLast.y);
                rainGenerator.active = true;
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
        nextLightningStrike = dayNightCycle.tick + Random.Range(1, 100);
        isLightning = false;
        yield return null;
    }

    Vector3 GetRandomPosition(int z)
    {
        Vector2 temp = (Random.insideUnitCircle * 5) + (Vector2)transform.position;
        
        return new Vector3(temp.x, temp.y, z);
    }
}
