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
    CycleTicks nextCycle;
    RealTimeDayNightCycle dayNightCycle;
    CallbackOnTick currentCallback;

    private void Start()
    {
        dayNightCycle = RealTimeDayNightCycle.instance;
        pool = GetComponent<ObjectPooler>();
        GetNextWindTick();

    }

    

    private void OnDisable()
    {
        dayNightCycle.RemoveCallbackOnTick(currentCallback);
        currentCallback = null;
    }


    void StartWindOnTick()
    {
        SpawnObject();
        GetNextWindTick();
    }


    public void GetNextWindTick()
    {
        var t = (windTimeCurve.Evaluate(Random.Range(0.0f, 1.0f))) * (minMaxTimeToSpawn.y - minMaxTimeToSpawn.x) + minMaxTimeToSpawn.x;
        nextCycle = dayNightCycle.GetCycleTime((int)t);
        currentCallback = dayNightCycle.AddCallbackOnTick(StartWindOnTick, nextCycle);
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
                if (hit[i] == null)
                    continue;
                if (hit[i].TryGetComponent(out IWindEffect affected))
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
