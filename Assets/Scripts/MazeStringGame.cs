using Klaxon.MazeTech;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeStringGame : MonoBehaviour
{
    public MazeCreator mazeCreator;
    [HideInInspector]
    public int currentIndex = -1;
    public LineRenderer line;
    public List<MazePost> endPostItems = new List<MazePost>();
    [HideInInspector]
    public bool inMaze;
    [HideInInspector]
    public bool mazeComplete;
    public QI_ItemDatabase mazeItemDatabase;
    public QI_Inventory rewardBox;
    FixingSounds attachSound;

    [Space]
    [Header("Line Shader")]
    public float bloomMinIntensity;
    public float bloomMaxIntensity;
    RealTimeDayNightCycle dayNightCycle;
    Material lineMaterial;
    Color initialColor;
    bool isStable;

    private void Start()
    {
        lineMaterial = line.material;
        dayNightCycle = RealTimeDayNightCycle.instance;
        initialColor = lineMaterial.GetColor("_EmissionColor");
        attachSound = GetComponentInChildren<FixingSounds>();
    }

    private void OnEnable()
    {
        GameEventManager.onTimeTickEvent.AddListener(SetGlow);
    }
    private void OnDisable()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(SetGlow);
    }

    public void ResetLine()
    {
        inMaze = false;
        line.positionCount = 0;
        mazeComplete = false;
        currentIndex = -1;
    }

    private void Update()
    {
        if(inMaze && line.positionCount > 1 && !mazeComplete) 
            line.SetPosition(line.positionCount - 1, PlayerInformation.instance.player.position + new Vector3(0, 0.2990625f*0.6f, endPostItems[currentIndex].transform.position.z + 3));
    }

    
    public void SetLinePosition()
    {
        currentIndex++;
        line.positionCount = currentIndex + 2;
        line.positionCount = Mathf.Clamp(line.positionCount, 0, endPostItems.Count + 1);
        if (currentIndex < line.positionCount)
            StartCoroutine(AttachString());

    }

    void TryCompleteMaze()
    {
        if (currentIndex == endPostItems.Count - 1)
        {
            line.positionCount = currentIndex + 1;
            mazeComplete = true;
            inMaze = false;
            rewardBox.AddItem(mazeItemDatabase.GetRandomWeightedItem(), 1, false);
        }
    }

    IEnumerator AttachString()
    {
        attachSound.StartSoundsWithTimer();
        var player = PlayerInformation.instance;
        player.playerInput.isInUI = true;
        player.animatePlayerScript.SetCraftAnimation(true);
        yield return new WaitForSeconds(2.0f);
        line.SetPosition(currentIndex, endPostItems[currentIndex].transform.position + endPostItems[currentIndex].lineDisplacement.displacedPosition + new Vector3(0, 0, 3));
        player.playerInput.isInUI = false;
        player.animatePlayerScript.SetCraftAnimation(false);
        TryCompleteMaze();
        
    }

    public void SetGlow(int time)
    {
        if (dayNightCycle.dayState == RealTimeDayNightCycle.DayState.Sunrise)
            SetGlowAmount(time, false);
        if (dayNightCycle.dayState == RealTimeDayNightCycle.DayState.Sunset)
            SetGlowAmount(time, true);
        if (dayNightCycle.dayState == RealTimeDayNightCycle.DayState.Day && !isStable)
            SetStableGlow(true);
        if (dayNightCycle.dayState == RealTimeDayNightCycle.DayState.Night && !isStable)
            SetStableGlow(false);

    }
    void SetStableGlow(bool state)
    {
        float intensity = state ? bloomMinIntensity : bloomMaxIntensity;
        lineMaterial.SetColor("_EmissionColor", initialColor * intensity);
        isStable = true;
    }

    void SetGlowAmount(int time, bool state)
    {
        isStable = false;
        float elapsedTime = time - (state ? dayNightCycle.nightStart : dayNightCycle.dayStart);
        float waitTime = dayNightCycle.dayNightTransitionTime;

        float startIntensity = state ? bloomMinIntensity : bloomMaxIntensity;
        float endIntinsity = state ? bloomMaxIntensity : bloomMinIntensity;
        float j = Mathf.Lerp(startIntensity, endIntinsity, (elapsedTime / waitTime));
        lineMaterial.SetColor("_EmissionColor", initialColor * j);
    }

    


}
