using Klaxon.GOAD;
using Klaxon.Interactable;
using System.Collections.Generic;
using UnityEngine;

public class RestoreSculpture : MonoBehaviour
{
    public SculptureSO sculpture;
    public List<PaintingIngredient> ingredients = new List<PaintingIngredient>();
    
    public InteractablePainting interactablePainting;

    public NavigationNode reachNode;
    public NavigationNode fixNode;
    public int ticks;
    [HideInInspector]
    public bool isCompleted;

    public GameObject sculptureCovering;
    public GameObject sculptureTablette;
    public ParticleSystem fixParticles;
    public FixingSounds fixSounds;

    public bool hasWorldConditionToFix;
    [ConditionalHide("hasWorldConditionToFix", true)]
    public GOAD_ScriptableCondition worldConditionNeeded;
    private void Start()
    {
        GetIsFinished();
    }

    
    public bool GetIsFinished()
    {
        bool finished = true;
        foreach (var item in ingredients)
        {
            if (!item.activated)
            {
                finished = false;
                break;
            }
        }

        SetFinished(finished);

        return finished;
    }

    void SetFinished(bool finished)
    {
        isCompleted = finished;
        sculptureCovering.SetActive(!finished);
        sculptureTablette.SetActive(finished);
        if (interactablePainting == null)
            interactablePainting = GetComponent<InteractablePainting>();
        interactablePainting.canInteract = !finished;
    }
}
