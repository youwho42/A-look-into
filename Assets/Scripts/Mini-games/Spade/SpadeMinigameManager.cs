using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpadeMinigameManager : MonoBehaviour, IMinigame
{

    [Serializable]
    public struct TargetBar
    {
        public SpriteRenderer controlAreaSprite;
        public SimpleRotate rotateControlArea;
        public DetectAreaHit controlAreaHit;
        public Collider2D hitDetectCollider;
    }


    [Serializable]
    public struct TargetArea
    {
        public SpriteRenderer dificultyAreaSprite;
        public MiniGameDificulty gameDificulty;
        public SimpleRotate dificultyAreaRotate;
        public Collider2D targetHitCollider;
    }

    AudioSource source;
    [SerializeField]
    public SoundSet[] soundSets;


    JunkPileInteractor currentJunkPile;
    MiniGameDificulty currentDificulty;
    public List<TargetBar> targetBars = new List<TargetBar>();
    public List<TargetArea> targetAreas = new List<TargetArea>();
    List<TargetArea> currentAreas = new List<TargetArea>();
    int mainSpinDirection;
    float setupTime = 0.3f;
    int currentStage = 0;
    int toolTier;
    Vector2Int gatherAmount;
    bool fullSuccess;
    bool minigameIsActive;
    bool transitioning;
    bool stageSet;
    
    Color initialIntensity;
    [ColorUsage(true, true)]
    public Color successEmission;
    [ColorUsage(true, true)]
    public Color failEmission;

    void Start()
    {
        stageSet = false;
        source = GetComponent<AudioSource>();
        initialIntensity = targetBars[0].controlAreaSprite.material.GetColor("_EmissionColor"); 
    }
    void OnEnable()
    {
        fullSuccess = true;
        GameEventManager.onMinigameMouseClickEvent.AddListener(OnMouseClick);
    }
    void OnDisable()
    {
        GameEventManager.onMinigameMouseClickEvent.RemoveListener(OnMouseClick);
    }

    void OnMouseClick()
    {
        if (!minigameIsActive || transitioning || !stageSet)
            return;

        if (currentStage < 3)
            StartCoroutine(CompleteCurrentStage(targetBars[currentStage].controlAreaHit.IsInArea()));
        

    }


    Vector2Int SetAmountPerHit()
    {
        if (toolTier == 1 && currentDificulty == MiniGameDificulty.Easy ||
            toolTier == 2 && currentDificulty == MiniGameDificulty.Normal ||
            toolTier == 3 && currentDificulty == MiniGameDificulty.Hard)
            return new Vector2Int(1,3);
        if (toolTier == 2 && currentDificulty == MiniGameDificulty.Easy ||
            toolTier == 3 && currentDificulty == MiniGameDificulty.Normal)
            return new Vector2Int(3, 6);
        if (toolTier == 4)
            return new Vector2Int(8, 12);
        return new Vector2Int(6, 9);
    }

    public void SetupMiniGame(JunkPileInteractor junkPile, MiniGameDificulty gameDificulty)
    {
        currentStage = 0;
        fullSuccess = true;
        minigameIsActive = true;
        currentJunkPile = junkPile;
        currentDificulty = gameDificulty;
        mainSpinDirection = UnityEngine.Random.Range(0, 2) * 2 - 1;
        toolTier = (int)PlayerInformation.instance.equipmentManager.GetEquipmentTier(EquipmentSlot.Hands) + 1;
        gatherAmount = SetAmountPerHit();
        SetDificulty(currentDificulty);
        Invoke("SetCurrentStage", setupTime * 2);
    }
    public void SetupMiniGame(QI_ItemData item, GameObject gameObject, MiniGameDificulty gameDificulty){}

    public void SetupMiniGame(PokableItem pokable, MiniGameDificulty gameDificulty){}

    IEnumerator CompleteCurrentStage(bool success)
    {
        transitioning = true;
        stageSet = false;
        StopCurrentStage();
        if (success)
        {
            var item = currentJunkPile.junkPileDatabase.GetRandomWeightedItem();
            int amount = UnityEngine.Random.Range(gatherAmount.x, gatherAmount.y);
            if(PlayerInformation.instance.playerInventory.AddItem(item, amount, false))
                Notifications.instance.SetNewNotification("", item, amount, NotificationsType.Inventory);
            else
                LostAndFoundManager.instance.AddToLostAndFound(item, amount);
                
                
            PlaySound(0);
            StartCoroutine(GlowOn(currentAreas[currentStage].dificultyAreaSprite.material, successEmission));
            StartCoroutine(GlowOn(targetBars[currentStage].controlAreaSprite.material, successEmission));
        }
        else
        {
            PlaySound(1);
            StartCoroutine(GlowOn(currentAreas[currentStage].dificultyAreaSprite.material, failEmission));
            StartCoroutine(GlowOn(targetBars[currentStage].controlAreaSprite.material, failEmission));
            fullSuccess = false;
        }
            
        
            
        yield return new WaitForSeconds(setupTime);
        StartCoroutine(GlowOn(currentAreas[currentStage].dificultyAreaSprite.material, initialIntensity));
        StartCoroutine(GlowOn(targetBars[currentStage].controlAreaSprite.material, initialIntensity));

        currentStage++;
        if (currentStage < 3)
            SetCurrentStage();
        else
        {
            currentJunkPile.StartParticles();
            yield return new WaitForSeconds(setupTime*2);
            CompleteMinigame();
        }
        transitioning = false;
        yield return null;
    }

    void SetCurrentStage()
    {
        targetAreas[currentStage].dificultyAreaRotate.SetRotationDirection(mainSpinDirection);
        targetBars[currentStage].rotateControlArea.SetRotationDirection(mainSpinDirection * -1);
        stageSet = true;
        
    }
    void StopCurrentStage()
    {
        targetAreas[currentStage].dificultyAreaRotate.SetRotationDirection(0);
        targetBars[currentStage].rotateControlArea.SetRotationDirection(0);
        
    }

    void SetDificulty(MiniGameDificulty dificulty)
    {

        currentDificulty = dificulty;
        List<int> startRotation = new List<int>();
        currentAreas.Clear();
        foreach (var target in targetAreas)
        {
            if (dificulty == target.gameDificulty)
            {
                int rot = UnityEngine.Random.Range(0, 359);
                startRotation.Add(rot);
                target.targetHitCollider.enabled = true;
                target.dificultyAreaRotate.gameObject.SetActive(true);
                target.dificultyAreaSprite.enabled = true;
                target.dificultyAreaRotate.StartToFromRotation(0, rot, setupTime);
                currentAreas.Add(target);
            }
            else
            {
                target.targetHitCollider.enabled = false;
                target.dificultyAreaRotate.gameObject.SetActive(false);
                target.dificultyAreaSprite.enabled = false;
            }

        }
        for (int i = 0; i < targetBars.Count; i++)
        {
            targetBars[i].rotateControlArea.StartToFromRotation(0, startRotation[i]-180, setupTime);
        }
        
    }

    void CompleteMinigame()
    {
        
        if (fullSuccess)
        {
            //have a grand ol time
        }

        
        MiniGameManager.instance.EndMiniGame(MiniGameType.Spade);
        if (currentJunkPile.gameObject.TryGetComponent(out SpawnableBallPersonArea spawnableBallPerson))
            spawnableBallPerson.SpawnBP();

    }
    public void ResetMiniGame()
    {
        currentAreas.Clear();
        fullSuccess = true;
        minigameIsActive = false;
        currentStage = 0;
    }


    IEnumerator GlowOn(Material materialToSet, Color color)
    {
        float elapsedTime = 0;
        float waitTime = 0.2f;

        while (elapsedTime < waitTime)
        {
            Color i = Color.Lerp(initialIntensity, color, (elapsedTime / waitTime));

            materialToSet.SetColor("_EmissionColor", i);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        materialToSet.SetColor("_EmissionColor", color);
        yield return null;
    }

    void PlaySound(int soundSet)
    {
        int t = UnityEngine.Random.Range(0, soundSets[soundSet].clips.Length);
        soundSets[soundSet].SetSource(source, t);
        soundSets[soundSet].Play();

    }

}
