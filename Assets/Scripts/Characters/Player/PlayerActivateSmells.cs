using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActivateSmells : MonoBehaviour
{
    
    bool active;

    List<SmellGenerator> smellGenerators = new List<SmellGenerator>();

    [SerializeField] private InputActionReference holdButton;

    
    public Transform noseKnowsObject;
    Transform noseKnows;
    SpriteRenderer noseSprite;
    Material noseMaterial;

    PlayerInformation player;
    float currentScale;
    float scaleSpeed = 10f;
    Vector2 minMaxNoseScale = new Vector2(2, 15);
    bool canSmell;

    int activatedSmells;
   


    public void Start()
    {
        noseKnows = CreateNose();
        noseKnows.gameObject.SetActive(false);
        noseSprite = noseKnows.GetComponentInChildren<SpriteRenderer>();
        noseMaterial = noseSprite.material;
        currentScale = 2;
        canSmell = true;
    }


    private void OnEnable()
    {
        
        holdButton.action.started += OnHoldButtonPerformed;
        
        player = PlayerInformation.instance;
       
    }

    private void OnDisable()
    {
        holdButton.action.started -= OnHoldButtonPerformed;
        
    }

    private void OnHoldButtonPerformed(InputAction.CallbackContext context)
    {
        if (UIScreenManager.instance.inMainMenu || UIScreenManager.instance.GetCurrentUI() != UIScreenType.None || !canSmell)
            return;

        ActivateSmells();
        
    }

    private void OnHoldButtonCanceled(InputAction.CallbackContext context)
    {
        DeactivateSmells();
        
    }

    private void Update()
    {
        
        if (active)
            noseKnows.position = player.player.position;
        

    }

    

    public void ActivateSmells()
    {
        
        smellGenerators = GetSmells();
        if (smellGenerators.Count <= 0)
        {
            active = false;
            return;
        }
        holdButton.action.canceled += OnHoldButtonCanceled;
        foreach (var smell in smellGenerators)
        {
            smell.StartSmells();
        }
        activatedSmells++;
        active = true;
        StopAllCoroutines();
        noseKnows.gameObject.SetActive(true);
        UIScreenManager.instance.PreventPlayerInputs(true);
        StartCoroutine(GrowNose());
        player.animatePlayerScript.ToggleBreatheAnimation(true);

    }
    void DeactivateSmells()
    {
        holdButton.action.canceled -= OnHoldButtonCanceled;
        UIScreenManager.instance.PreventPlayerInputs(false);
        StopAllCoroutines();

        foreach (var smell in smellGenerators)
        {
            smell.StopSmells();
        }
        smellGenerators.Clear();
        StartCoroutine(ShrinkNose());
    }

    IEnumerator GrowNose()
    {
        AudioManager.instance.StopSound("BreatheOut");
        AudioManager.instance.PlaySound("BreatheIn");
        while (currentScale < minMaxNoseScale.y)
        {
            player.animatePlayerScript.SetBreateState((currentScale - minMaxNoseScale.x) / (minMaxNoseScale.y - minMaxNoseScale.x));
            currentScale += (scaleSpeed - player.statHandler.GetStatMaxModifiedValue("Sniffer")) * activatedSmells * Time.deltaTime;
            noseMaterial.SetFloat("_FisheyeIntensity", currentScale);
            yield return null;
        }
        StartCoroutine(ColorNose());
        yield return new WaitForSeconds(2.0f);
        DeactivateSmells();

    }

    IEnumerator ColorNose()
    {
        canSmell = false;
        holdButton.action.canceled -= OnHoldButtonCanceled;
        noseSprite.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        noseSprite.color = Color.white;
    }

    IEnumerator ShrinkNose()
    {
        
        AudioManager.instance.StopSound("BreatheIn");
        AudioManager.instance.PlaySound("BreatheOut");
        player.animatePlayerScript.ToggleBreatheAnimation(false);
        while (currentScale > minMaxNoseScale.x)
        {
            player.animatePlayerScript.SetBreateState((currentScale - minMaxNoseScale.x) / (minMaxNoseScale.y - minMaxNoseScale.x));
            currentScale -= scaleSpeed * Time.deltaTime;
            noseMaterial.SetFloat("_FisheyeIntensity", currentScale);
            yield return null;
        }
        canSmell = true;
        activatedSmells = 0;
        player.animatePlayerScript.SetBreateState(0);
        noseKnows.gameObject.SetActive(false);
        active = false;
    }


    List<SmellGenerator> GetSmells()
    {
        smellGenerators.Clear();
        var wind = WindManager.instance;
        var player = PlayerInformation.instance;
        List<SmellGenerator> smells = new List<SmellGenerator>();
        var hits = Physics2D.OverlapCircleAll(player.player.position, 0.3336f);
        if(hits.Length > 0)
        {
            foreach (var item in hits)
            {
                if(item.TryGetComponent(out SmellGenerator smell))
                {
                    if (smell.transform.position.z != player.player.position.z)
                        continue;
                    var windDirection = wind.GetWindDirectionFromPosition(smell.transform.position).normalized;
                    var smellDirection = ((Vector2)player.player.position - (Vector2)smell.transform.position).normalized;
                    float dot = Vector2.Dot(windDirection, smellDirection);
                    if(dot > 0.5f)
                        smells.Add(smell);
                }
                    
            }
        }
        return smells;
    }


    Transform CreateNose()
    {
        if(player==null)
            player = PlayerInformation.instance;

        Transform nose = Instantiate(noseKnowsObject, transform);
        nose.transform.position = player.player.position;

        nose.gameObject.SetActive(true);
        
        return nose;
    }

    

}
