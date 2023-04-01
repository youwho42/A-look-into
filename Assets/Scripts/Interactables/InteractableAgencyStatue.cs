using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableAgencyStatue : Interactable
{

    public QI_ItemData agencyItem;
    public int agencyAmount; 
    Material material;
    public SpriteRenderer rend;
    Color initialColor;

    public bool hasBeenActivated;

    public override void Start()
    {
        base.Start();
        material = rend.material;
        initialColor = material.GetColor("_EmissionColor");
        GameEventManager.onTimeHourEvent.AddListener(ResetStatue);
    }

    void ResetStatue(int time)
    {
        if(time == 5)
        {
            material.SetColor("_EmissionColor", initialColor);
            hasBeenActivated = false;
            canInteract = true;
        }
    }

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);

        if(!hasBeenActivated)
            StartCoroutine(InteractCo(interactor));


    }

    IEnumerator InteractCo(GameObject interactor)
    {
        canInteract = false;
        hasBeenActivated = true;

        float startIntensity = 2;
        float endIntinsity = 20;

        float elapsedTime = 0;
        float waitTime = 1.5f;
        while (elapsedTime < waitTime)
        {
            float j = Mathf.Lerp(startIntensity, endIntinsity, (elapsedTime / waitTime));
            SetMaterialColor(j);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        yield return new WaitForSeconds(0.5f);

        startIntensity = 20;
        endIntinsity = -5;

        elapsedTime = 0;
        waitTime = 0.7f;
        while (elapsedTime < waitTime)
        {
            float j = Mathf.Lerp(startIntensity, endIntinsity, (elapsedTime / waitTime));
            SetMaterialColor(j);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        PlayerInformation.instance.playerInventory.AddItem(agencyItem, agencyAmount, false);
        NotificationManager.instance.SetNewNotification($"{agencyItem.Name} {agencyAmount}", NotificationManager.NotificationType.Inventory);
    }

    public void SetMaterialColor(float amount)
    {
        material.SetColor("_EmissionColor", initialColor * amount);
    }

    void PlayInteractSound()
    {
        if (audioManager.CompareSoundNames("PickUp-" + interactSound))
        {
            audioManager.PlaySound("PickUp-" + interactSound);
        }
    }

}
