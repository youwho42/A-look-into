using QuantumTek.QuantumInventory;
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
    }

    bool transitioning;
    public int maxAttempts;
    public int attemptSteps;
    int currentAttempts;
    int currentIndex;
    int currentAttemptHits;
    QI_ItemData item;
    public bool hasCompletedMiniGame;
    Material material;
    Color initialIntensity;
    public MiniGameType miniGameType;
    public List<Ball> balls = new List<Ball>();
    public List<DificultyArea> dificultyAreas = new List<DificultyArea>();
    MiniGameDificulty currentDificulty;
    private void Start() 
    { 
        material = balls[0].ballSprite.material;
        initialIntensity = material.GetColor("_EmissionColor");
        ResetBalls(0);
        ResetMiniGame();
    }

    private void Update()
    {
        if(currentAttempts != maxAttempts && !transitioning)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                StartCoroutine(NextBallCo(balls[currentIndex].ballHitDetection.isInArea));
            }
        }
        if (currentAttempts == maxAttempts && !transitioning)
        {
            MiniGameManager.instance.EndMiniGame(miniGameType);
        }
    }

    IEnumerator GlowOn(int amount)
    {
        float elapsedTime = 0;
        float waitTime = 0.5f;

        while (elapsedTime < waitTime)
        {
            Color i = Color.Lerp(initialIntensity, initialIntensity * amount, (elapsedTime / waitTime));

            material.SetColor("_EmissionColor", i);
            elapsedTime +=Time.deltaTime;

            yield return null;
        }

        material.SetColor("_EmissionColor", initialIntensity * amount);
        yield return null;
    }
    void GlowOff()
    {
        material.SetColor("_EmissionColor", initialIntensity);
    }


    IEnumerator NextBallCo(bool success)
    {
        transitioning = true;
        balls[currentIndex].rotateBall.enabled = false;
        if (success)
        {
            currentAttemptHits++;
            
            StartCoroutine(GlowOn(15));
        }
        else
        {
            StartCoroutine(GlowOn(-5));
        }
        yield return new WaitForSeconds(1f);
        if (currentIndex < balls.Count - 1)
        {
            currentIndex++;
        }
        else
        {
            currentIndex = 0;
            currentAttempts++;
            SetDificulty(currentDificulty);
        }
        if(currentAttemptHits == attemptSteps)
        {
            PlayerInformation.instance.playerInventory.AddItem(item, 1);
            currentAttemptHits = 0;
        }
        ResetBalls(currentIndex);
        yield return null;
    }

    private void ResetBalls(int index)
    {
        transitioning = false;
        GlowOff();
        for (int i = 0; i < balls.Count; i++)
        {
            if (index != i)
            {
                balls[i].rotateBall.RandomizeRotation();
                balls[i].rotateBall.enabled = false;
                balls[i].ballCollider.enabled = false;
                balls[i].ballSprite.enabled = false;
            }
            else
            {
                material = balls[i].ballSprite.material;
                balls[i].rotateBall.RandomizeRotation();
                balls[i].rotateBall.enabled = true;
                balls[i].rotateBall.RandomizeDirection();
                balls[i].ballCollider.enabled = true;
                balls[i].ballSprite.enabled = true;
            }
        }
    }
    public void SetupMiniGame(QI_ItemData item, GameObject gameObject, MiniGameDificulty gameDificulty) 
    {
        this.item = item;
        SetDificulty(gameDificulty);
    }
    void SetDificulty(MiniGameDificulty dificulty)
    {
        currentDificulty = dificulty;
        foreach (var dif in dificultyAreas)
        {
            if(dificulty == dif.gameDificulty)
            {
                dif.rotate.gameObject.SetActive(true);
                dif.dificultyAreaGameObject.enabled = true;
                dif.rotate.AnimateRandomizeRotation(0.5f);
                
            }
            else
            {
                dif.rotate.gameObject.SetActive(false);
                dif.dificultyAreaGameObject.enabled = false;
            }
        }
    }
    public void ResetMiniGame()
    {
        currentAttemptHits = 0;
        currentAttempts = 0;
        currentIndex = 0;
        transform.parent.gameObject.SetActive(false);
    }
}
