using BezierSolution;
using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsectMiniGameManager : MonoBehaviour, IMinigame
{
    [Serializable]
    public struct TargetArea
    {
        public SpriteRenderer targetAreaSprite;
        public Collider2D targetCollider;
        public DetectAreaHit targetHitDetection;
        public float shrinkSpeed;
    }
    AudioSource source;
    [SerializeField]
    public SoundSet[] soundSets;

    //public GameObject playerCircle;
    public BezierSpline bezierSpline;
    public int points;
    public QI_ItemData animal;
    public SpriteRenderer animalHolder;
    public float areaSize;
    public MiniGameType miniGameType;
    public BezierWalkerWithTime walker;
    GameObject targetGameObject;

    public int maxAttempts;
    public int attemptSteps;
    int currentAttempts;
    int currentAttemptHits;
    int currentIndex;
    bool transitioning;
    bool shrinking;
    public bool hasCompletedMiniGame;
    public List<TargetArea> targetAreas = new List<TargetArea>();
    Color initialIntensity;

   

    private void Start()
    {
        source = GetComponent<AudioSource>();
        initialIntensity = targetAreas[0].targetAreaSprite.material.GetColor("_EmissionColor");
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

    
    void OnMouseClick()
    {
        if (walker == null)
            return;
        
        if (!transitioning)
        {
            if (currentAttempts != maxAttempts)
                StartCoroutine(NextTargetAreaCo(targetAreas[currentIndex].targetHitDetection.isInArea));
        }
    }

    

    IEnumerator GlowOn(int amount, Material material)
    {

        float elapsedTime = 0;
        float waitTime = 0.5f;

        while (elapsedTime < waitTime)
        {
            Color i = Color.Lerp(initialIntensity, initialIntensity * amount, (elapsedTime / waitTime));

            material.SetColor("_EmissionColor", i);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        material.SetColor("_EmissionColor", initialIntensity * amount);
        yield return null;
    }

    void GlowOff()
    {
        foreach (var target in targetAreas)
        {
            target.targetAreaSprite.material.SetColor("_EmissionColor", initialIntensity);
        }
    }
    
    
    IEnumerator NextTargetAreaCo(bool success)
    {
        transitioning = true;
        bool continues = true; 
        if (success)
        {
            currentAttemptHits++;
            PlaySound(0);
            StartCoroutine(GlowOn(20, targetAreas[currentIndex].targetAreaSprite.material));
        }
        else
        {
            PlaySound(1);
            StartCoroutine(GlowOn(-20, targetAreas[currentIndex].targetAreaSprite.material));
            continues = false;
        }
        yield return new WaitForSeconds(1f);
        if (!continues)
        {
            MiniGameManager.instance.EndMiniGame(miniGameType);
            yield return null;
        }
        if (currentIndex < targetAreas.Count - 1)
        {
            currentIndex++;
        }
        else
        {
            currentIndex = 0;
            currentAttempts++;
        }
        if (currentAttemptHits == attemptSteps)
        {

            // Add animal to compendium if not already done
            if (!PlayerInformation.instance.playerAnimalCompendiumDatabase.Items.Contains(animal)){
                PlayerInformation.instance.playerAnimalCompendiumDatabase.Items.Add(animal);
                PlayerInformation.instance.playerStats.AddToAgency(20);
                GameEventManager.onAnimalCompediumUpdateEvent.Invoke();
                NotificationManager.instance.SetNewNotification($"{animal.Name} found", NotificationManager.NotificationType.Compendium);
            }
            
            // Add animal to compendium information
            PlayerInformation.instance.animalCompendiumInformation.AddAnimal(animal.Name);

            // Add recipe revealed if already viewed enough times and you don't have the recipe already
            if(animal.ResearchRecipes.Count > 0)
            {
                int index = PlayerInformation.instance.animalCompendiumInformation.animalNames.IndexOf(animal.Name);
                int amount = PlayerInformation.instance.animalCompendiumInformation.animalTimesViewed[index];
                for (int i = 0; i < animal.ResearchRecipes.Count; i++)
                {
                    if (PlayerInformation.instance.playerRecipeDatabase.CraftingRecipes.Contains(animal.ResearchRecipes[i].recipe))
                        continue;
                    if (amount >= animal.ResearchRecipes[i].RecipeRevealAmount)
                    {
                        PlayerInformation.instance.playerStats.AddToAgency(animal.ResearchRecipes[i].AgencyReward);
                        PlayerCrafting.instance.AddCraftingRecipe(animal.ResearchRecipes[i].recipe);
                        NotificationManager.instance.SetNewNotification($"{animal.ResearchRecipes[i].recipe.Name} recipe learned", NotificationManager.NotificationType.Compendium);
                    }
                }
            }
            
            currentAttemptHits = 0;
            MiniGameManager.instance.EndMiniGame(miniGameType);
        }
        ResetTargetArea(currentIndex);
        yield return null;
    }

    private void ResetTargetArea(int index)
    {
        transitioning = false;
        shrinking = false;
        GlowOff();
        for (int i = 0; i < targetAreas.Count; i++)
        {
            if (index != i)
            {
                targetAreas[i].targetAreaSprite.gameObject.SetActive(false);
            }
            else
            {
                animalHolder.transform.position = bezierSpline[(int)bezierSpline.Count / 4 * i].position;
                targetAreas[i].targetAreaSprite.gameObject.SetActive(true);
            }
        }
    }

    void SetBezierPath()
    {
        for (int i = 0; i < points; i++)
        {
            Vector2 rand = (Vector2)transform.position + new Vector2(UnityEngine.Random.Range(-areaSize, areaSize), UnityEngine.Random.Range(-areaSize, areaSize));
            bezierSpline.InsertNewPointAt(i);
            bezierSpline[i].position = rand;

        }
        bezierSpline.AutoConstructSpline2();
    }
    bool PlaySound(int soundSet)
    {
        if (!source.isPlaying)
        {
            int t = UnityEngine.Random.Range(0, soundSets[soundSet].clips.Length);
            soundSets[soundSet].SetSource(source, t);
            soundSets[soundSet].Play(AudioTrack.Effects);

            return true;
        }
        return false;
    }

    public void SetupMiniGame(QI_ItemData item, GameObject gameObject, MiniGameDificulty gameDificulty) 
    {
        animal = item;
        animalHolder.sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
        UpdateShapeToSprite(animalHolder.GetComponent<PolygonCollider2D>(), animalHolder.sprite);
        targetGameObject = gameObject;

        SetAnimalState(false);


        for (int i = 0; i < targetAreas.Count; i++)
        {
            if (targetAreas[i].targetAreaSprite.gameObject.activeSelf)
            {
                if (targetAreas[i].targetAreaSprite.gameObject.TryGetComponent(out BezierWalkerWithTime newWalker))
                {
                    walker = newWalker;
                }
            }
        }
        
        
        SetBezierPath();
    }

    public static void UpdateShapeToSprite(PolygonCollider2D collider, Sprite sprite)
    {
        // ensure both valid
        if (collider != null && sprite != null)
        {
            // update count
            collider.pathCount = sprite.GetPhysicsShapeCount();

            // new paths variable
            List<Vector2> path = new List<Vector2>();

            // loop path count
            for (int i = 0; i < collider.pathCount; i++)
            {
                // clear
                path.Clear();
                // get shape
                sprite.GetPhysicsShape(i, path);
                // set path
                collider.SetPath(i, path.ToArray());
            }
        }
    }


    public void ResetMiniGame()
    {
        
        bezierSpline.ResetSpline();
        currentAttemptHits = 0;
        currentAttempts = 0;
        currentIndex = 0;
        shrinking = false;
        ResetTargetArea(currentIndex);
        transform.parent.gameObject.SetActive(false);
        
        PlayerInformation.instance.playerAnimator.SetBool("UseEquipement", false);
        SetAnimalState(true);

    }

    void SetAnimalState(bool active)
    {
        if (targetGameObject != null)
        {
            IAnimal thisAnimal = targetGameObject.transform.GetComponentInParent<IAnimal>();
            if (thisAnimal != null)
                thisAnimal.SetActiveState(active);
            targetGameObject.GetComponent<Animator>().speed = active? 1 : 0;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(areaSize*2, areaSize*2, 0));
    }
}

