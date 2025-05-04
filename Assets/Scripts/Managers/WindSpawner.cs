using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class WindSpawner : MonoBehaviour
{
    public static WindSpawner instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public Vector2Int minMaxTimeToSpawn;
    public AnimationCurve windTimeCurve;
    
    public Transform objectToFollow;
    ObjectPooler pool;
    int nextWindTick;

    private void Start()
    {
        pool = GetComponent<ObjectPooler>();
        nextWindTick = GetNextWindTick(RealTimeDayNightCycle.instance.currentTimeRaw);
    }

    private void OnEnable()
    {
        GameEventManager.onTimeTickEvent.AddListener(StartWindOnTick);
    }

    private void OnDisable()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(StartWindOnTick);
    }

    void StartWindOnTick(int tick)
    {
        
        if (tick >= nextWindTick)
        {
            SpawnObject();
            nextWindTick = GetNextWindTick(tick);
        }
    }

    
    public int GetNextWindTick(int tick)
    {
        
        var t = (windTimeCurve.Evaluate(Random.Range(0.0f, 1.0f))) * (minMaxTimeToSpawn.y - minMaxTimeToSpawn.x) + minMaxTimeToSpawn.x;
        
        return ((int)t + tick) % 1440;
    }

    private void SpawnObject()
    {

        GameObject go = pool.GetPooledObject();

        if(go != null)
        {
            go.transform.position = GetRandomPosition();
            CurrentGridLocation locationZ = go.GetComponent<CurrentGridLocation>();
            if (locationZ != null)
            {
                locationZ.GetTileLocation();
                locationZ.UpdateLocationAndPosition();
            }
            go.SetActive(true);
            IPoolPrefab np = go.GetComponent<IPoolPrefab>();
            if(np != null)
            {
                np.OnObjectSpawn();
                StartCoroutine(CheckAffectedObjects(go.transform));
            }
                
        }
    }

    private IEnumerator CheckAffectedObjects(Transform location)
    {
        var hit = Physics2D.OverlapCircleAll(location.position, 1.75f);
        if (hit.Length > 0)
        {
            
            for (int i = 0; i < hit.Length; i++)
            {
                if(hit[i].TryGetComponent(out IWindEffect affected))
                {
                    
                    affected.Affect(true);
                    yield return new WaitForSeconds(.1f);
                }
            }
        }
        yield return null;
    }

    Vector2 GetRandomPosition()
    {
        return (Random.insideUnitCircle * 2) + (Vector2)objectToFollow.position;
    }
    
}
