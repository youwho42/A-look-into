using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    protected AudioManager audioManager;
    public string interactSound = "Default";
    public string interactVerb = "whaaAA???";

    public float bounceCost;
    public float agencyReward;
    public float agencyCost;
    public bool canInteract = true;

    protected bool hasInteracted;
    protected PlayerInformation playerInformation;

    public virtual void Start()
    {
        audioManager = AudioManager.instance;
        playerInformation = PlayerInformation.instance;
    }

    public virtual void Interact(GameObject interactor)
    {
        if (hasInteracted)
            return;

        

        hasInteracted = true;

        // The rest happens in child script...
    }

    


}
