using Klaxon.Interactable;
using QuantumTek.QuantumInventory;
using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class PaintingIngredient
{
    public QI_ItemData item;
    public bool isPhysicalItem;
    [ConditionalHide("isPhysicalItem", true)]
    public int amount;

    [HideInInspector]
    public bool complete;
    [HideInInspector]
    public bool activated;
}

public class RestorePainting : MonoBehaviour
{

    public PaintingSO painting;
    public List<PaintingIngredient> ingredients = new List<PaintingIngredient>();
    [SerializeField]
    private SpriteRenderer worldSprite;
    [SerializeField]
    private Sprite unfinishedSprite;
    [SerializeField]
    private Sprite finishedSprite;
    [HideInInspector]
    public InteractablePainting interactablePainting;

    public NavigationNode paintingNode;

    


    private void Start()
    {
        interactablePainting = GetComponent<InteractablePainting>();
        foreach (var item in ingredients)
        {
            item.complete = false;
            item.activated = false;
        }
        worldSprite.sprite = unfinishedSprite;
    }
    
    public bool GetIsFinished()
    {
        bool finished = true;
        foreach (var item in ingredients)
        {
            if(!item.activated)
            {
                finished = false;
                break;
            }
        }
        if (finished) 
            SetFinishedWorldSprite();

        interactablePainting.hasLongInteract = !finished;
        
            
        return finished;
    }

    public int GetPaintingLayer(QI_ItemData item)
    {
        for (int i = 0; i < painting.paintingLayers.Count; i++)
        {
            if (painting.paintingLayers[i].itemNeeded == item)
                return i;
        }
        return -1;
    }

    public void SetFinishedWorldSprite()
    {

        worldSprite.sprite = finishedSprite;
    }
}
