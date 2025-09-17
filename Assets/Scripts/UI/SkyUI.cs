using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class SkyUI : MonoBehaviour
{

    public static SkyUI instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }
    public CanvasGroup canvasGroup;
    public RectTransform starHolder;
    //public RectTransform starObject;
    public RectTransform shootingStarObject;
    public RectTransform LightObject;
    Image LightImage;
    RectTransform currentShootingStar;

    public Image sky;
    public Color nightSkyColor;
    //public AnimationCurve starAlphaCurve;
    //public AnimationCurve starSizeCurve;
    //public AnimationCurve starColorCurve;
    public AnimationCurve shootingStarAlphaCurve;

    //[Header("Noise")]
    //public bool useOpenSimplex;
    //public float noiseScale;
    //public int octaves;
    //[Range(0, 1)]
    //public float persistance;
    //public float lacunarity;
    //public int seed;
    //public Vector2 offset;
    //float[,] starMap;
    //float[,] starIntensities;

    //Vector2 lastStarHolderSize;
    RealTimeDayNightCycle dayNightCycle;
    CallbackOnTick currentCallback = null;
    UnityEngine.Rendering.Universal.Light2D sun;
    HammockInteractable currentHammock;

    public void SetHammock(HammockInteractable hammock)
    {
        currentHammock = hammock;
    }
    void Start()
    {

        LightImage = LightObject.GetComponent<Image>();
        dayNightCycle = RealTimeDayNightCycle.instance;
        
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if(dayNightCycle == null)
            dayNightCycle = RealTimeDayNightCycle.instance;
        //CreateStarField.instance.CreateStarMap();
        SetSkyColor();
        GameEventManager.onDayStateChangeEvent.AddListener(SetSkyColor);
        sun = dayNightCycle.GetSun();
    }
    private void OnDisable()
    {
        GameEventManager.onDayStateChangeEvent.RemoveListener(SetSkyColor);
        GameEventManager.onTimeTickEvent.RemoveListener(FadeStarsIn);
        GameEventManager.onTimeTickEvent.RemoveListener(FadeStarsOut);
        if (currentCallback != null)
        {
            dayNightCycle.RemoveCallbackOnTick(currentCallback);
            currentCallback = null;
        }
        StopAllCoroutines();
        currentShootingStar = null;
        if (currentHammock != null)
            currentHammock.HideSky();
    }

    void SetSkyColor()
    {
        switch (dayNightCycle.dayState)
        {
            case RealTimeDayNightCycle.DayState.Sunrise:
                GameEventManager.onTimeTickEvent.AddListener(FadeStarsOut);
                FadeStarsOut(dayNightCycle.currentTimeRaw);
                break;
            case RealTimeDayNightCycle.DayState.Day:
                canvasGroup.alpha = 0;
                sky.color = Color.white;
                SetLightImageColor();
                break;
            case RealTimeDayNightCycle.DayState.Sunset:
                GameEventManager.onTimeTickEvent.AddListener(FadeStarsIn);
                FadeStarsIn(dayNightCycle.currentTimeRaw);
                break;
            case RealTimeDayNightCycle.DayState.Night:
                canvasGroup.alpha = 0.2f;
                sky.color = nightSkyColor;
                SetLightImageColor();
                SetNextShootingStarTime();
                break;
            
        }
    }

    private void SetLightImageColor()
    {
        if (sun == null)
            return;
        var c = sun.color;
        c.a = .25f;
        LightImage.color = c;
    }

    void SetNextShootingStarTime()
    {
        var cycleTick = dayNightCycle.GetCycleTime(Random.Range(5, 120));
        currentCallback = dayNightCycle.AddCallbackOnTick(ShootingStarEvent, cycleTick);
    }
    public void ShootingStarEvent()
    {
        StartCoroutine("ShootingStarCo");
    }
    IEnumerator ShootingStarCo()
    {
        currentShootingStar = Instantiate(shootingStarObject, starHolder);

        var shootingStar = currentShootingStar.GetComponent<ShootingStarUI>();
        
        float wh = Random.Range(6.0f, 16.0f);
        
        var size = new Vector2(wh, wh);
        currentShootingStar.sizeDelta = size;
        float wOff = starHolder.rect.width * 0.05f;
        float hOff = starHolder.rect.height * 0.05f;
        float halfWidth = starHolder.rect.width * 0.5f;
        float halfHeight = starHolder.rect.height * 0.5f;
        Vector2 basePos = new Vector2(Random.Range(wOff, starHolder.rect.width - wOff), Random.Range(hOff, starHolder.rect.height - hOff));

        Vector2 startPos = new Vector2(basePos.x - halfWidth, basePos.y - halfHeight);
        currentShootingStar.anchoredPosition = startPos;

        bool up = basePos.y <= starHolder.rect.height * 0.5f;

        float speed = Random.Range(8.5f, 10.0f);
        Vector2 destination = new Vector2(Random.Range(-halfWidth, halfWidth), up ? halfHeight : -halfHeight).normalized * speed;

        Vector3 dir = (destination - startPos).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        currentShootingStar.rotation = Quaternion.Euler(0, 0, angle);

        float visibleTime = Random.Range(0.3f, .8f);
        float timer = 0;
        float trailHeight = shootingStar.trail.sizeDelta.y;
        while (timer <= visibleTime)
        {
            timer += Time.deltaTime;
            float currentTime = timer / visibleTime;
            
            //Color c = new Color(1, 1, 1, shootingStarAlphaCurve.Evaluate(currentTime));
            shootingStar.shootingStarGroup.alpha = shootingStarAlphaCurve.Evaluate(currentTime);
            
            Vector2 currentPos = Vector2.Lerp(startPos, destination, currentTime);
            currentShootingStar.anchoredPosition = currentPos;
            
            shootingStar.trail.sizeDelta = new Vector2(shootingStarAlphaCurve.Evaluate(currentTime) * (size.x * 10), trailHeight);
            yield return null;
        }

        Destroy(currentShootingStar.gameObject);
        currentShootingStar = null;
        SetNextShootingStarTime();
        yield return null;
    }
    

    void FadeStarsIn(int tick)
    {
        int startTime = dayNightCycle.nightStart + (int)(dayNightCycle.dayNightTransitionTime * .5f);
        int endTime = startTime + dayNightCycle.dayNightTransitionTime;

        float currentFadeTime = NumberFunctions.RemapNumber(tick, startTime, endTime, 0.0f, 1.0f);
        sky.color = Color.Lerp(Color.white, nightSkyColor, currentFadeTime);
        SetLightImageColor();
        float currentAlphaTime = NumberFunctions.RemapNumber(tick, startTime, endTime, 0.0f, 0.15f);
        canvasGroup.alpha = currentAlphaTime;
        if(canvasGroup.alpha >= 0.15f)
            GameEventManager.onTimeTickEvent.RemoveListener(FadeStarsIn);
    }
    void FadeStarsOut(int tick)
    {
        int startTime = dayNightCycle.dayStart;
        int endTime = startTime + (int)(dayNightCycle.dayNightTransitionTime *.5f);

        float currentFadeTime = NumberFunctions.RemapNumber(tick, startTime, endTime, 1.0f, 0.0f);
        sky.color = Color.Lerp(Color.white, nightSkyColor, currentFadeTime);
        SetLightImageColor();
        float currentAlphaTime = NumberFunctions.RemapNumber(tick, startTime, endTime, 0.15f, 0.0f);
        canvasGroup.alpha = currentAlphaTime;
        if (canvasGroup.alpha <= 0.0f)
            GameEventManager.onTimeTickEvent.RemoveListener(FadeStarsOut);
    }





    //#region Create Stars

    //public void StartCreateStars()
    //{
    //    StartCoroutine("CreateStarMap");
    //}

    //IEnumerator CreateStarMap()
    //{
    //    if(lastStarHolderSize.x == starHolder.rect.width && lastStarHolderSize.y == starHolder.rect.height)
    //        yield return null;
    //    starMap = NoiseGenerator.GenerateNoiseMap(useOpenSimplex, (int)starHolder.rect.width, (int)starHolder.rect.height, seed, noiseScale, octaves, persistance, lacunarity, offset);
    //    starIntensities = NoiseGenerator.GenerateNoiseMap(useOpenSimplex, (int)starHolder.rect.width, (int)starHolder.rect.height, 2342, noiseScale, octaves, persistance, lacunarity, offset);
    //    CreateStars();
    //    lastStarHolderSize = new Vector2(starHolder.rect.width, starHolder.rect.height);
    //}

    //void CreateStars()
    //{
    //    ClearStars();
        
        
    //    for (int x = 0; x < (int)starHolder.rect.width; x++)
    //    {
    //        for (int y = 0; y < (int)starHolder.rect.height; y++)
    //        {
    //            if (starMap[x, y] > .9f)
    //            {
    //                if (!CanSpawn(starMap, x, y))
    //                    continue;
    //                RectTransform star = Instantiate(starObject, starHolder);
    //                var starImage = star.GetComponent<Image>();
    //                float alphaEval = starAlphaCurve.Evaluate(starIntensities[x, y]);
    //                float sizeEval = starSizeCurve.Evaluate(starIntensities[x, y]);
    //                float wh = NumberFunctions.RemapNumber(sizeEval, 0.0f, 1.0f, 3.0f, 10.0f);
    //                float alpha = alphaEval + 0.002f;
    //                Random.InitState(x + y);
    //                float h = Random.value;
    //                float s = starColorCurve.Evaluate(Random.value);
    //                Color bob = Color.HSVToRGB(h, s, 1);
                    
                    
    //                Color c = new Color(bob.r, bob.g, bob.b, alpha);
    //                starImage.color = c;
    //                var size = new Vector2(wh, wh);
    //                star.sizeDelta = size;
    //                star.anchoredPosition = new Vector2(x - starHolder.rect.width * 0.5f, y - starHolder.rect.height * 0.5f);
                    
    //            }

    //        }
    //    }
       
    //}

    //bool CanSpawn(float[,] noiseMap, int x, int y)
    //{

    //    for (int i = -3; i < 3; i++)
    //    {
    //        for (int j = -3; j < 3; j++)
    //        {
    //            if (i == 0 && j == 0)
    //                continue;
    //            if (x + i < 0 || x + i > starHolder.rect.width - 1 || y + j < 0 || y + j > starHolder.rect.height - 1)
    //                continue;

    //            if (noiseMap[x + i, y + j] > .9f)
    //                return false;
    //        }
    //    }
    //    return true;
    //}

    //void ClearStars()
    //{
    //    //Get an array with all children to this transform
    //    GameObject[] allChildren = GetAllChildren();

    //    //Now destroy them
    //    foreach (GameObject child in allChildren)
    //    {
    //        if (Application.isPlaying)
    //            Destroy(child);
    //        else
    //            DestroyImmediate(child);
    //    }
    //}

    //private GameObject[] GetAllChildren()
    //{
    //    //This array will hold all children
    //    GameObject[] allChildren = new GameObject[starHolder.childCount];

    //    //Fill the array
    //    int childCount = 0;
    //    foreach (Transform child in starHolder)
    //    {
    //        allChildren[childCount] = child.gameObject;
    //        childCount += 1;
    //    }

    //    return allChildren;
    //}
    //#endregion
}
