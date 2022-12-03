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
    public struct PlayerControlSection
    {
        public List<PlayerControlArea> controlAreas;
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

    public SpriteRenderer centerRenderer;
    Material centerMaterial;
    
    
    public bool hasCompletedMiniGame;
    Material material;
    Color initialIntensity;
    public MiniGameType miniGameType;
    public List<PlayerControlSection> playerControlSections = new List<PlayerControlSection>();
    public List<TargetArea> targetAreas = new List<TargetArea>();
    MiniGameDificulty currentDificulty;


    private void Start()
    {
        SetDificulty(MiniGameDificulty.Easy);
        source = GetComponent<AudioSource>();
        initialIntensity = playerControlSections[currentSection].controlAreas[(int)currentDificulty - 1].controlAreaSprite.material.GetColor("_EmissionColor");
        centerMaterial = centerRenderer.material;
        ResetMiniGame();
    }


    private void Update()
    {
        
        if (currentSection != playerControlSections.Count && !transitioning)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                StartCoroutine(NextAreaCo(playerControlSections[currentSection].controlAreas[(int)currentDificulty - 1].controlAreaHit.isInArea));
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
            section.controlAreas[(int)currentDificulty - 1].controlAreaSprite.material.SetColor("_EmissionColor", initialIntensity);
            section.controlAreas[(int)currentDificulty - 1].rotateControlArea.SetRotation(rand);
            centerMaterial.SetColor("_EmissionColor", initialIntensity);
        }
        transform.parent.gameObject.SetActive(false);
    }
   
    public void ResetPlayerAreaRotations()
    {
        int dir = UnityEngine.Random.Range(0, 2) * 2 - 1;
        foreach (var section in playerControlSections)
        {
            for (int i = 0; i < section.controlAreas.Count; i++)
            {
                if (i + 1 == ((int)currentDificulty))
                {
                    section.controlAreas[i].controlAreaSprite.enabled = true;
                    section.controlAreas[i].rotateControlArea.SetRotationDirection(dir);
                    section.controlAreas[i].rotateControlArea.enabled = true;
                    section.controlAreas[i].hitDetectCollider.enabled = true;
                }
                else
                {
                    section.controlAreas[i].controlAreaSprite.enabled = false;
                    section.controlAreas[i].rotateControlArea.enabled = false;
                    section.controlAreas[i].hitDetectCollider.enabled = false;
                }
            }
        }
    }
    
   

    IEnumerator NextAreaCo(bool success)
    {
        transitioning = true;
        material = playerControlSections[currentSection].controlAreas[(int)currentDificulty - 1].controlAreaSprite.material;
        initialIntensity = material.GetColor("_EmissionColor");
        playerControlSections[currentSection].controlAreas[(int)currentDificulty - 1].rotateControlArea.enabled = false;
        if (success)
        {
            currentSection++;
            PlayerInformation.instance.playerInventory.AddItem(item, 1, false);
            PlaySound(0);
            StartCoroutine(GlowOn(material, 10));
            StartCoroutine(GlowOn(centerMaterial, currentSection * 10));
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
      float waitTime = 0.5f;

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
