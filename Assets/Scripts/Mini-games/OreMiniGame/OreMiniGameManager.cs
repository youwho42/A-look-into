using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreMiniGameManager : MonoBehaviour, IMinigame
{
    [Serializable]
    public struct PlayerControlArea
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

    QI_ItemData item;

    bool transitioning;
    int currentSection;

    
    
    
    public bool hasCompletedMiniGame;
    Material material;
    Color initialIntensity;
    public MiniGameType miniGameType;
    public List<PlayerControlArea> playerControlSections = new List<PlayerControlArea>();
    public List<TargetArea> targetAreas = new List<TargetArea>();
    MiniGameDificulty currentDificulty;


    private void Start()
    {
        SetDificulty(MiniGameDificulty.Easy);
        source = GetComponent<AudioSource>();
        initialIntensity = playerControlSections[currentSection].controlAreaSprite.material.GetColor("_EmissionColor");
        ResetMiniGame();
    }


    private void Update()
    {
        
        if (currentSection != playerControlSections.Count && !transitioning)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                StartCoroutine(NextAreaCo(playerControlSections[currentSection].controlAreaHit.isInArea));
            }
        }
        if (currentSection == playerControlSections.Count && !transitioning)
        {
           
            MiniGameManager.instance.EndMiniGame(miniGameType);
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
        foreach (var target in targetAreas)
        {
            if (dificulty == target.gameDificulty)
            {
                target.targetHitCollider.enabled = true;
                target.dificultyAreaRotate.gameObject.SetActive(true);
                target.dificultyAreaSprite.enabled = true;
                target.dificultyAreaRotate.AnimateRandomizeRotation(0.5f);
            }
            else
            {
                target.targetHitCollider.enabled = false;
                target.dificultyAreaRotate.gameObject.SetActive(false);
                target.dificultyAreaSprite.enabled = false;
            }

            ResetPlayerAreaRotations();
        }
    }

    public void ResetMiniGame()
    {
        currentSection = 0;
        int rand = UnityEngine.Random.Range(0, 360);
        foreach (var section in playerControlSections)
        {
            section.controlAreaSprite.material.SetColor("_EmissionColor", initialIntensity);
            section.rotateControlArea.SetRotation(rand);
        }
        transform.parent.gameObject.SetActive(false);
    }
   
    public void ResetPlayerAreaRotations()
    {
        int dir = UnityEngine.Random.Range(0, 2) * 2 - 1;
        foreach (var section in playerControlSections)
        {
            section.controlAreaSprite.enabled = true;
            section.rotateControlArea.SetRotation(UnityEngine.Random.Range(0, 360));
            section.rotateControlArea.SetRotationDirection(dir);
            section.rotateControlArea.enabled = true;
            section.hitDetectCollider.enabled = true;
        }
    }
    
   

    IEnumerator NextAreaCo(bool success)
    {
        transitioning = true;
        material = playerControlSections[currentSection].controlAreaSprite.material;
        initialIntensity = material.GetColor("_EmissionColor");
        playerControlSections[currentSection].rotateControlArea.enabled = false;
        if (success)
        {
            currentSection++;
            PlayerInformation.instance.playerInventory.AddItem(item, 1, false);
            PlaySound(0);
            StartCoroutine(GlowOn(material, 10));
        }
         else
         {
            currentSection++;
            PlaySound(1);
            StartCoroutine(GlowOn(material, -5));
        }

         yield return new WaitForSeconds(1f);
        
        transitioning = false;
        yield return null;
    }
   
   IEnumerator GlowOn(Material materialToSet, int amount)
   {
      float elapsedTime = 0;
      float waitTime = 0.2f;

      while (elapsedTime < waitTime)
      {
          Color i = Color.Lerp(initialIntensity, initialIntensity * amount, (elapsedTime / waitTime));

            materialToSet.SetColor("_EmissionColor", i);
          elapsedTime += Time.deltaTime;

          yield return null;
      }

        materialToSet.SetColor("_EmissionColor", initialIntensity * amount);
      yield return null;
   }
   void GlowOff(Material materialToSet)
   {
        materialToSet.SetColor("_EmissionColor", initialIntensity);
   }


    bool PlaySound(int soundSet)
    {
        if (!source.isPlaying)
        {
            int t = UnityEngine.Random.Range(0, soundSets[soundSet].clips.Length);
            soundSets[soundSet].SetSource(source, t);
            soundSets[soundSet].Play();

            return true;
        }
        return false;
    }


    
}
