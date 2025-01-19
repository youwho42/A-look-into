﻿using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodMiniGameManager : MonoBehaviour, IMinigame
{
    [Serializable]
    public struct Ball
    {
        public SpriteRenderer ballSprite;
        public Collider2D ballCollider;
        public DetectAreaHit ballHitDetection;
        public SimpleRotate rotateBall;
    }

    [Serializable]
    public struct DificultyArea
    {
        public SpriteRenderer dificultyAreaGameObject;
        public MiniGameDificulty gameDificulty;
        public SimpleRotate rotate;
        public Collider2D collider;
    }

    AudioSource source;
    [SerializeField]
    public SoundSet[] soundSets;

    bool transitioning;
    public int maxAttempts;
    public int attemptSteps;
    int currentAttempts;
    int currentIndex;
    int currentAttemptHits;
    QI_ItemData item;
    public bool hasCompletedMiniGame;
    //Material material;
    Color initialIntensity;
    public MiniGameType miniGameType;
    public List<Ball> balls = new List<Ball>();
    public List<DificultyArea> dificultyAreas = new List<DificultyArea>();
    MiniGameDificulty currentDificulty;
    GameObject currentGameObject;
    bool minigameIsActive;
    [ColorUsage(true, true)]
    public Color successEmission;
    [ColorUsage(true, true)]
    public Color failEmission;

    int toolTier;
    int gatherAmount;
    private void Start() 
    {
        
        source = GetComponent<AudioSource>();
        initialIntensity = balls[0].ballSprite.material.GetColor("_EmissionColor");
       
        ResetMiniGame();
    }
    void OnEnable()
    {
        GameEventManager.onMinigameMouseClickEvent.AddListener(OnMouseClick);
    }
    void OnDisable()
    {
        GameEventManager.onMinigameMouseClickEvent.RemoveListener(OnMouseClick);

    }

    int SetAmountPerHit()
    {
        if (toolTier == 1 && currentDificulty == MiniGameDificulty.Easy || 
            toolTier == 2 && currentDificulty == MiniGameDificulty.Normal || 
            toolTier == 3 && currentDificulty == MiniGameDificulty.Hard)
            return 1;
        if (toolTier == 2 && currentDificulty == MiniGameDificulty.Easy ||
            toolTier == 3 && currentDificulty == MiniGameDificulty.Normal)
            return 2;
        if (toolTier == 4)
            return 6;
        return 5;
    }

    int SetMaxHitAttempts()
    {
        if (toolTier == 1 && currentDificulty == MiniGameDificulty.Easy ||
            toolTier == 2 && currentDificulty == MiniGameDificulty.Normal ||
            toolTier == 3 && currentDificulty == MiniGameDificulty.Hard)
            return 3;
        if (toolTier == 2 && currentDificulty == MiniGameDificulty.Easy ||
            toolTier == 3 && currentDificulty == MiniGameDificulty.Normal)
            return 2;
        return 1;
    }

    void OnMouseClick()
    {
        if (!minigameIsActive)
            return;


        if (currentAttempts != maxAttempts && !transitioning)
            StartCoroutine(NextBallCo(balls[currentIndex].ballHitDetection.isInArea));
        
    }

    void CheckCurrentAttempts()
    {
        if (currentAttempts >= maxAttempts)
        {
            if (currentGameObject.TryGetComponent(out TreeRustling tree))
                tree.Affect(true);

            MiniGameManager.instance.EndMiniGame(miniGameType);
        }
            
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

    void GlowOff(Material materialToSet)
    {
        materialToSet.SetColor("_EmissionColor", initialIntensity);
    }

    IEnumerator NextBallCo(bool success)
    {
        
        transitioning = true;
        balls[currentIndex].rotateBall.enabled = false;
        var mat = balls[currentIndex].ballSprite.material;
        
        if (success)
        {
            currentAttemptHits++;
            if (!PlayerInformation.instance.playerInventory.AddItem(item, gatherAmount, false))
                LostAndFoundManager.instance.inventory.AddItem(item, gatherAmount, false);
            Notifications.instance.SetNewNotification("", item, gatherAmount, NotificationsType.Inventory);

            PlaySound(0);
            StartCoroutine(GlowOn(mat, successEmission));
        }
        else
        {
            PlaySound(1);
            StartCoroutine(GlowOn(mat, failEmission));
        }
        float t = currentIndex == balls.Count - 1 ? 0.75f : 0.01f;
        yield return new WaitForSeconds(t);
        if (currentIndex < balls.Count - 1)
        {
            currentIndex++;
        }
        else
        {
            currentIndex = 0;
            currentAttempts ++;
            SetDificulty(currentDificulty);
        }
        CheckCurrentAttempts();
        if (currentAttemptHits >= attemptSteps)
        {
            
            currentAttemptHits = 0;
        }
        ResetBalls(currentIndex);
        yield return null;
    }

    private void ResetBalls(int index)
    {
        if (index == 0)
        {
            
            for (int i = 0; i < balls.Count; i++)
            {
                GlowOff(balls[i].ballSprite.material);
                balls[i].rotateBall.RandomizeRotation();
                balls[i].rotateBall.RandomizeDirection();
            }
        }
        transitioning = false;
        
        for (int i = 0; i < balls.Count; i++)
        {
            if (i < index)
            {
                balls[i].rotateBall.enabled = false;
                balls[i].ballCollider.enabled = false;
            }
            else if (i > index)
            {
                balls[i].rotateBall.enabled = true;
                balls[i].ballSprite.enabled = true;
            }
            else
            {
                balls[i].rotateBall.enabled = true;
                balls[i].ballCollider.enabled = true;
                balls[i].ballSprite.enabled = true;
            }

        }
    }
    public void SetupMiniGame(QI_ItemData item, GameObject gameObject, MiniGameDificulty gameDificulty) 
    {
        
        currentGameObject = gameObject;
        this.item = item;
        SetDificulty(gameDificulty);
        minigameIsActive = true;
        ResetBalls(0);
        toolTier = (int)PlayerInformation.instance.equipmentManager.GetEquipmentTier(EquipmentSlot.Hands) + 1;
        maxAttempts = SetMaxHitAttempts();
        gatherAmount = SetAmountPerHit();
    }
    public void SetupMiniGame(PokableItem pokable, MiniGameDificulty gameDificulty){ }

    void SetDificulty(MiniGameDificulty dificulty)
    {
        currentDificulty = dificulty;
        foreach (var dif in dificultyAreas)
        {
            if(dificulty == dif.gameDificulty)
            {
                dif.rotate.gameObject.SetActive(true);
                dif.dificultyAreaGameObject.enabled = true;
                dif.collider.enabled = true;
                dif.rotate.AnimateRandomizeRotation(0.5f);
                
            }
            else
            {
                dif.rotate.gameObject.SetActive(false);
                dif.dificultyAreaGameObject.enabled = false;
                dif.collider.enabled = false;
            }
        }
    }

    void PlaySound(int soundSet)
    {
        int t = UnityEngine.Random.Range(0, soundSets[soundSet].clips.Length);
        soundSets[soundSet].SetSource(source, t);
        soundSets[soundSet].Play();
    }

    public void ResetMiniGame()
    {
        minigameIsActive = false;
        currentAttemptHits = 0;
        currentAttempts = 0;
        currentIndex = 0;
        currentGameObject = null;
    }
}
