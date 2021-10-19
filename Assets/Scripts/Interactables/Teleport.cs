using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : Interactable
{
    public Transform teleportTwin;
    [HideInInspector]
    public Material material;
    [HideInInspector]
    public GameObject objectToTeleport; 
    public int teleportTwinLevel;
    

    bool isInTeleportRange;
    [HideInInspector]
    public bool isIncoming;

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);
        material = interactor.GetComponentInChildren<SpriteRenderer>().material;
        objectToTeleport = interactor.gameObject;
        isInTeleportRange = true;
        StartTeleport();
    }

    public void StartTeleport()
    {
        StartCoroutine("TeleportCo");
    }

    IEnumerator TeleportCo()
    {
        teleportTwin.GetComponent<Teleport>().isIncoming = true;
        Material usingMaterial = material;
        GameObject activeTeleport = objectToTeleport;
        activeTeleport.GetComponent<Playermovement>().enabled = false;
        //float timePercentage = 0f;
        //float fadeTime = 2f;

        //while (timePercentage < 1f)
        //{
        //    timePercentage += Time.deltaTime / fadeTime;
        //    float x = Mathf.Lerp(1f, 0f, timePercentage);
        //    usingMaterial.SetFloat("_Fade", x);

        //    yield return null;
        //}
        DissolveEffect.instance.StartDissolve(usingMaterial, 2f, false);
        yield return new WaitForSeconds(2f);
        activeTeleport.SetActive(false);
        activeTeleport.transform.position = new Vector3(teleportTwin.position.x, teleportTwin.position.y, teleportTwinLevel);
        activeTeleport.GetComponent<PlayerLevelChange>().UpdatePlayerLocation();
        SetCollisionLayers.instance.SetCollisionLayer();
        yield return new WaitForSeconds(1f);

        activeTeleport.SetActive(true);

        //timePercentage = 0f;

        //while (timePercentage < 1f)
        //{
        //    timePercentage += Time.deltaTime / fadeTime;
        //    float x = Mathf.Lerp(0f, 1f, timePercentage);
        //    usingMaterial.SetFloat("_Fade", x);

        //    yield return null;

        //}

        DissolveEffect.instance.StartDissolve(usingMaterial, 2f, true);
        yield return new WaitForSeconds(2f);
        if (isIncoming && isInTeleportRange)
        {
            isIncoming = false;
        }
        material = null; ;
        objectToTeleport = null;
        isInTeleportRange = false;
        activeTeleport.GetComponent<Playermovement>().enabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, teleportTwin.position);
    }
}
