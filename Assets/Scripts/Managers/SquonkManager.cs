using Klaxon.GOAD;
using Klaxon.GravitySystem;
using System.Collections;
using UnityEngine;

public class SquonkManager : MonoBehaviour
{
    public static SquonkManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }
    public GameObject theSquonk;
    public SpriteRenderer squonkSprite;
    Material squonkMaterial;
    public DrawZasYDisplacement appearCenter;
    public GOAD_ScriptableCondition dayToSpawn;
    [Range(0.0f, 1.0f)]
    public float chanceToSpawn;
    float currentChance;
    public bool isActive;
    private void Start()
    {
        currentChance = chanceToSpawn;
        theSquonk.SetActive(false);
        isActive = false;
        GameEventManager.onDayStateChangeEvent.AddListener(TrySpawnSquonk);
        squonkMaterial = squonkSprite.material;
    }
    private void OnDisable()
    {
        GameEventManager.onDayStateChangeEvent.RemoveListener(TrySpawnSquonk);
    }

    void TrySpawnSquonk()
    {

        //if (!GOAD_WorldBeliefStates.instance.HasState(dayToSpawn.Condition, dayToSpawn.State))
        //    return;
        if (RealTimeDayNightCycle.instance.dayState == RealTimeDayNightCycle.DayState.Sunset)
        {
            if (Random.Range(0.0f, 1.0f) <= currentChance)
            {
                currentChance = chanceToSpawn;
                isActive = true;
                StartCoroutine(SquonkAppearCo());
            }
            else
            {
                currentChance += currentChance * 0.2f;
            }
                
        }
        else if (RealTimeDayNightCycle.instance.dayState == RealTimeDayNightCycle.DayState.Night)
        {
            StartCoroutine(SquonkDisappearCo(false));
        }


    }

    public void SquonkDisappear()
    {
        if (!isActive)
            return;
        StopAllCoroutines();
        
        StartCoroutine(SquonkDisappearCo(true));
    }

    IEnumerator SquonkAppearCo()
    {
        var t = RealTimeDayNightCycle.instance.currentTimeRaw + 20;
        while (RealTimeDayNightCycle.instance.currentTimeRaw < t)
            yield return null;
        var pos = GetSquonkPosition();
        theSquonk.transform.position = pos;
        theSquonk.GetComponent<CurrentTilePosition>().position = GridManager.instance.GetTilePosition(pos);
        theSquonk.GetComponent<GravityItemNew>().currentLevel = (int)pos.z - 1;

        squonkMaterial.SetFloat("_Fade", 0);
        theSquonk.SetActive(true);
        DissolveEffect.instance.StartDissolve(squonkMaterial, 2.0f, true);
        yield return new WaitForSeconds(2.0f);
        
    }

    IEnumerator SquonkDisappearCo(bool immediate)
    {
        if (!immediate)
        {
            var t = RealTimeDayNightCycle.instance.currentTimeRaw + 20;
            while (RealTimeDayNightCycle.instance.currentTimeRaw < t)
                yield return null;
        }
        

        DissolveEffect.instance.StartDissolve(squonkMaterial, 2.0f, false);
        yield return new WaitForSeconds(2.0f);
        isActive = false;
        squonkSprite.GetComponent<GatherableItem>().hasBeenHarvested = false;
        theSquonk.SetActive(false);


    }
    private Vector3 GetSquonkPosition()
    {
        var pos = GridManager.instance.GetRandomTileWorldPosition(appearCenter.transform.position, appearCenter.size);
        var hit = Physics2D.OverlapPoint(pos, LayerMask.GetMask("Obstacle"), pos.z, pos.z);
        if (!hit)
            return pos;
        for (int d = 1; d < 3; d++)
        {
            for (float i = 0; i < Mathf.PI * 2; i += Mathf.PI * 0.25f)
            {
                Vector2 dir = new Vector2(Mathf.Cos(i), Mathf.Sin(i));
                dir = dir.normalized;
                dir *= d * 0.1f;
                var posI = pos + (Vector3)dir;
                var h = Physics2D.OverlapPoint(posI, LayerMask.GetMask("Obstacle"), posI.z, posI.z);
                if (!h && GridManager.instance.GetTileValid(posI))
                    return posI;
                //var dir = Vector2.
            }
        }
        return GetSquonkPosition();
    }

}
