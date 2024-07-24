using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokingMiniGame : MonoBehaviour, IMinigame
{
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


    Color initialIntensity;
    public MiniGameType miniGameType;
    public List<DificultyArea> dificultyAreas = new List<DificultyArea>();
    public SimpleRotate basePositionRotate;
    public Transform startStickBone;
    public DetectAreaHit detectAreaHit;

    public Transform endStick;
    public Transform startStick;
    MiniGameDificulty currentDificulty;
    DificultyArea currentArea;
    public SpriteRenderer backgroundIllumination;
    Material bgIlluminationMaterial;
    bool minigameIsActive;
    PokableItem currentPokable;

    [ColorUsage(true, true)]
    public Color successEmission;
    [ColorUsage(true, true)]
    public Color failEmission;

    Vector3 endOffset = new Vector3(0, -0.07f, 0);

    private void Start()
    {
       
        source = GetComponent<AudioSource>();
        bgIlluminationMaterial = backgroundIllumination.material;
        initialIntensity = bgIlluminationMaterial.GetColor("_EmissionColor");
        
    }
    void OnEnable()
    {
        GameEventManager.onMinigameMouseClickEvent.AddListener(OnMouseClick);
    }
    void OnDisable()
    {
        GameEventManager.onMinigameMouseClickEvent.RemoveListener(OnMouseClick);

    }

    public void SetupMiniGame(PokableItem pokable, MiniGameDificulty gameDificulty)
    {
        SetBase();
        currentPokable = pokable;
        SetDificulty(gameDificulty);
        minigameIsActive = true;
        endStick.gameObject.SetActive(false);
    }
   
    public void SetupMiniGame(QI_ItemData item, GameObject gameObject, MiniGameDificulty gameDificulty) { }

    void SetDificulty(MiniGameDificulty gameDificulty)
    {
        currentDificulty = gameDificulty;
        foreach (var variant in dificultyAreas)
        {
            if(variant.gameDificulty == gameDificulty)
            {
                variant.dificultyAreaGameObject.gameObject.SetActive(true);
                variant.rotate.enabled = true;
                variant.rotate.RandomizeDirection();
                variant.collider.enabled = true;
                currentArea = variant;
            }
            else
            {
                variant.dificultyAreaGameObject.gameObject.SetActive(false);
                variant.rotate.enabled = false;
                variant.collider.enabled = false;
            }
        }
    }

    public void ResetMiniGame()
    {
        
        GlowOff(bgIlluminationMaterial);
        
        currentPokable = null;
        minigameIsActive = false;
        endStick.gameObject.SetActive(false);
    }

    void OnMouseClick()
    {
        if (!minigameIsActive)
            return;
        if (!detectAreaHit.isInArea)
        {
            PlaySound(0);
            endStick.gameObject.SetActive(true);
            endStick.transform.position = startStickBone.transform.position + endOffset;
            StartCoroutine(RotateCo(0.2f));
            currentArea.rotate.enabled = false;
            StartCoroutine(GlowOn(bgIlluminationMaterial, successEmission));
            if (currentPokable != null)
                currentPokable.PokeItemSuccess();
        }
        else
        {
            PlaySound(1);
            currentArea.rotate.enabled = false;
            StartCoroutine(GlowOn(bgIlluminationMaterial, failEmission));
            if (currentPokable != null)
                currentPokable.PokeItemFail();
        }

        Invoke("EndMinigame", 0.95f);

    }

    void EndMinigame()
    {
        MiniGameManager.instance.EndMiniGame(miniGameType);
    }


    void SetBase()
    {
        basePositionRotate.RandomizeRotation();
        var rot = basePositionRotate.transform.eulerAngles.z;
        var dif = rot - 90;
        startStickBone.transform.eulerAngles = new Vector3(0, 0, rot - dif);
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
            if (detectAreaHit.isInArea)
                backgroundIllumination.color = i;
            yield return null;
        }

        materialToSet.SetColor("_EmissionColor", color);
        yield return null;
    }

    void GlowOff(Material materialToSet)
    {
        backgroundIllumination.color = Color.white;
        materialToSet.SetColor("_EmissionColor", initialIntensity);
    }


    IEnumerator RotateCo(float time)
    {

        
        float elapsedTime = 0;
        float waitTime = time;
        Vector3 currentRotation = currentArea.rotate.transform.eulerAngles;
        while (elapsedTime < waitTime)
        {
            currentRotation.z = Mathf.Lerp(currentRotation.z, basePositionRotate.transform.eulerAngles.z, (elapsedTime / waitTime));

            currentArea.rotate.transform.eulerAngles = currentRotation;
            elapsedTime += Time.deltaTime;

            yield return null;
        }


        yield return null;
    }


    void PlaySound(int soundSet)
    {

        int t = UnityEngine.Random.Range(0, soundSets[soundSet].clips.Length);
        soundSets[soundSet].SetSource(source, t);
        soundSets[soundSet].Play();

    }
}
