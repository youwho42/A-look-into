using Klaxon.UndertakingSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InteractableTempleStation : Interactable
{
    public GameObject purpleRain;
    public ParticleSystem particleEffect;
    public List<GameObject> firesToLight = new List<GameObject>();
    [HideInInspector]
    public bool isActivated;
    public Tilemap fissureMap;

    public List<Vector3Int> fissurePositions = new List<Vector3Int>();

    CompleteTaskOnInteraction taskOnInteraction;

    public override void Start()
    {
        base.Start();
        taskOnInteraction = GetComponent<CompleteTaskOnInteraction>();
    }

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);
        if (!isActivated)
        {
            if (InteractCostReward())
            {
                
                //SetTempleFireAndRainStates(true);
                StartCoroutine(LightFiresCo(true));
            }
        }
        
    }

    IEnumerator LightFiresCo(bool lit)
    {
        canInteract = false;
        isActivated = lit;

        //particle effect
        particleEffect.Play();

        yield return new WaitForSeconds(1f);

        // Set drops to hide at top
        var allDrops = purpleRain.GetComponentsInChildren<HideDrop>();
        foreach (var drop in allDrops)
        {
            drop.StartFade();
        }

        yield return new WaitForSeconds(5f);

        // purple rain stops
        Destroy(purpleRain);

        // close fissure
        for (int i = 0; i < fissurePositions.Count; i++)
        {
            fissureMap.SetTile(fissurePositions[i], null);

        }

        // light flames
        foreach (var fire in firesToLight)
        {
            fire.SetActive(lit);
        }

        // Complete task/undertaking if there is one
        if (taskOnInteraction.task.undertaking != null)
            taskOnInteraction.CompleteTask();

    }

    public void SetTempleFireAndRainStates(bool lit)
    {
        isActivated = lit;
        canInteract = !lit;
        foreach (var fire in firesToLight)
        {
            fire.SetActive(lit);
        }
        if (lit)
        {
            for (int i = 0; i < fissurePositions.Count; i++)
            {
                fissureMap.SetTile(fissurePositions[i], null);
            }
            Destroy(purpleRain);
        }
            
    }

    bool InteractCostReward()
    {
        float agency = playerInformation.playerStats.playerAttributes.GetAttributeValue("Agency");
        if (agency >= agencyCost)
            return true;

        NotificationManager.instance.SetNewNotification($"{agencyCost} Agency needed", NotificationManager.NotificationType.Warning);
        return false;
    }


    private void OnDrawGizmosSelected()
    {
        if (fissurePositions.Count < 0)
            return;

        for (int i = 0; i < fissurePositions.Count; i++)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(fissureMap.GetCellCenterWorld(fissurePositions[i]), 0.2f);
        }
    }
}
