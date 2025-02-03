using Klaxon.StatSystem;
using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class InsectMiniGameManager : MonoBehaviour, IMinigame
{
    
    [Serializable]
    public struct Focal
    {
        public SpriteRenderer focalSprite;
        public Collider2D focalCollider;
        public DetectAreaHit focalHitDetection;
        public SimpleRotate rotateFocal;
    }

    [Serializable]
    public struct DifficultyArea
    {
        public SpriteRenderer dificultyAreaGameObject;
        public MiniGameDificulty gameDificulty;
        public SimpleRotate rotate;
        public Collider2D collider;
    }


    public MiniGameType miniGameType;
    MiniGameDificulty currentDificulty;

    AudioSource source;
    [SerializeField]
    public SoundSet[] soundSets;

    public Focal focal;
    public List<DifficultyArea> difficultyAreas = new List<DifficultyArea>();
    public SpriteRenderer baseImage;
    
    public QI_ItemData animal;
    public SpriteRenderer animalHolder;
    SpriteRenderer currentAnimalSprite;
    Material animalFocusMaterial;

    bool minigameIsActive;


    GameObject animalGameObject;

    public int maxAttempts;
    int successfullHits;
    
    int currentIndex;
    bool transitioning;

    public bool hasCompletedMiniGame;
    
    Color initialIntensity;
    public StatChanger statChanger;


    [ColorUsage(true, true)]
    public Color successEmission;
    [ColorUsage(true, true)]
    public Color failEmission;
    bool blurred;

    private void Start()
    {
        animalFocusMaterial = animalHolder.material;
        source = GetComponent<AudioSource>();
        initialIntensity = focal.focalSprite.material.GetColor("_EmissionColor");
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

    void Update()
    {
        if(currentAnimalSprite != null)
            animalHolder.sprite = currentAnimalSprite.sprite;
    }


    void OnMouseClick()
    {
        if (!minigameIsActive)
            return;

        if (!transitioning)
        {
            if (currentIndex != maxAttempts)
                StartCoroutine(NextTargetAreaCo(focal.focalHitDetection.IsInArea()));
        }
        
    }

    IEnumerator GlowOn(bool success)
    {
        float elapsedTime = 0;
        float waitTime = 0.2f;
        var focalMaterial = focal.focalSprite.material;
        var baseMaterial = baseImage.material;
        var color = success ? successEmission : failEmission;
        while (elapsedTime < waitTime)
        {
            Color i = Color.Lerp(initialIntensity, color, (elapsedTime / waitTime));
            if(success)
                baseMaterial.SetColor("_EmissionColor", i);
            focalMaterial.SetColor("_EmissionColor", i);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        if (success)
            baseMaterial.SetColor("_EmissionColor", color);
        focalMaterial.SetColor("_EmissionColor", color);
        yield return null;
    }

    void GlowOff()
    {
        var focalMaterial = focal.focalSprite.material;
        var baseMaterial = baseImage.material;
        baseMaterial.SetColor("_EmissionColor", initialIntensity);
        focalMaterial.SetColor("_EmissionColor", initialIntensity);
    }

    IEnumerator SetBlur(bool blur)
    {
        float timer = 0;
        float waitTime = 0.15f;
        float startBlur = blur ? 0.0f : 10.0f;
        float endBlur = blur ? 10.0f : 0.0f;
        while (timer < waitTime)
        {

            float b = Mathf.Lerp(startBlur, endBlur, timer / waitTime);
            animalFocusMaterial.SetFloat("_Blur", b);
            timer += Time.deltaTime;
            yield return null;
        }
        blurred = blur;
        animalFocusMaterial.SetFloat("_Blur", endBlur);
        yield return null;
    }

    IEnumerator NextTargetAreaCo(bool success)
    {
        transitioning = true;
        focal.rotateFocal.enabled = false;
        StartCoroutine(GlowOn(success));
        
        if (success)
        {
            successfullHits++;
            PlaySound(0);
            StartCoroutine(SetBlur(false));
        }
        else
        {
            PlaySound(1);
        }
        yield return new WaitForSeconds(0.7f);
        bool miniGameOver = false;
        if (currentIndex < maxAttempts - 1)
            currentIndex++;
        else
        {
            currentIndex = 0;


            if (successfullHits >= 0)
            {

                
                if (successfullHits > 0)
                {
                    // Add animal to compendium if not already done
                    if (!PlayerInformation.instance.playerAnimalCompendiumDatabase.Items.Contains(animal))
                    {
                        PlayerInformation.instance.playerAnimalCompendiumDatabase.Items.Add(animal);
                        PlayerInformation.instance.statHandler.ChangeStat(statChanger);
                        GameEventManager.onAnimalCompediumUpdateEvent.Invoke();
                        Notifications.instance.SetNewNotification($"{animal.localizedName.GetLocalizedString()}", null, 0, NotificationsType.Compendium);
                    }

                    // Add animal to compendium information

                    PlayerInformation.instance.animalCompendiumInformation.AddAnimal(animal.Name, successfullHits);

                    bool receivedNotification = false;
                    // Add recipe revealed if already viewed enough times and you don't have the recipe already
                    if (animal.ResearchRecipes.Count > 0)
                    {
                        int index = PlayerInformation.instance.animalCompendiumInformation.animalNames.IndexOf(animal.Name);
                        int amount = PlayerInformation.instance.animalCompendiumInformation.animalTimesViewed[index];
                        receivedNotification = PlayerInformation.instance.animalCompendiumInformation.viewedComplete[index];
                        if (!receivedNotification)
                        {
                            for (int i = 0; i < animal.ResearchRecipes.Count; i++)
                            {

                                if (amount >= animal.ResearchRecipes[i].RecipeRevealAmount)
                                {

                                    string notificationText = $"{animal.localizedName.GetLocalizedString()} {LocalizationSettings.StringDatabase.GetLocalizedString($"Static Texts", "Updated")}";
                                    Notifications.instance.SetNewNotification(notificationText, null, 0, NotificationsType.Compendium);
                                    PlayerInformation.instance.animalCompendiumInformation.SetViewedComplete(index);
                                    PlayerInformation.instance.statHandler.ChangeStat(animal.ResearchRecipes[i].agencyStatChanger);
                                }
                            }
                        }

                    }
                }
                

                
                miniGameOver = true;
                
                MiniGameManager.instance.EndMiniGame(miniGameType);
            }
        }
        if(!miniGameOver)
            ResetTargetArea();
        yield return null;
    }

    private void ResetTargetArea()
    {
        if(!blurred)
            StartCoroutine(SetBlur(true));
        transitioning = false;
        focal.rotateFocal.enabled = true;
        SetDificulty(currentDificulty);
        //animalFocusMaterial.SetFloat("_Blur", 10);
        GlowOff();
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

    public void SetupMiniGame(PokableItem pokable, MiniGameDificulty gameDificulty) { }
    public void SetupMiniGame(JunkPileInteractor junkPile, MiniGameDificulty gameDificulty) { }


    public void SetupMiniGame(QI_ItemData item, GameObject animalObject, MiniGameDificulty gameDificulty)
    {
        minigameIsActive = true;
        animal = item;
        currentAnimalSprite = animalObject.GetComponent<SpriteRenderer>();
        animalHolder.sprite = currentAnimalSprite.sprite;
        animalGameObject = animalObject;
        SetDificulty(gameDificulty);
        ResetFocal();
        ResetTargetArea();
        SetAnimalState(false);
        animalFocusMaterial.SetFloat("_Blur", 10.0f);
        blurred = true;
    }

    private void ResetFocal()
    {
        
        focal.rotateFocal.RandomizeRotation();
        focal.rotateFocal.RandomizeDirection();
        focal.rotateFocal.enabled = true;
        transitioning = false;
    }

    void SetDificulty(MiniGameDificulty dificulty)
    {
        currentDificulty = dificulty;
        foreach (var dif in difficultyAreas)
        {
            if (dificulty == dif.gameDificulty)
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

    public void ResetMiniGame()
    {
        currentIndex = 0;
        successfullHits = 0;
        minigameIsActive = false;
        currentAnimalSprite = null;
        PlayerInformation.instance.playerAnimator.SetBool("UseEquipement", false);
        SetAnimalState(true);
    }

    void SetAnimalState(bool active)
    {
        if (animalGameObject != null)
        {
            if (animalGameObject.TryGetComponent(out SquonkTearManager squonk))
                SquonkManager.instance.SquonkDisappear();
            //IAnimal thisAnimal = animalGameObject.transform.GetComponentInParent<IAnimal>();
            //if (thisAnimal != null)
            //    thisAnimal.SetActiveState(active);
            //animalGameObject.GetComponent<Animator>().speed = active? 1 : 0;
        }
    }

}

