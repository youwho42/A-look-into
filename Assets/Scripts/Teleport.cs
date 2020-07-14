using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Transform teleportTwin;
    Material material;
    GameObject objectToTeleport;
    public int teleportTwinLevel;
    

    bool isInTeleportRange;
    public bool isIncoming;
   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            material = collision.GetComponentInChildren<SpriteRenderer>().material;
            objectToTeleport = collision.gameObject;
            isInTeleportRange = true;
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            material = null; ;
            objectToTeleport = null;
            isInTeleportRange = false;
            
        }
    }

    private void Update()
    {
        if (isInTeleportRange && !isIncoming)
        {

            if (Input.GetKeyDown(KeyCode.F))
            {
                StartCoroutine("TeleportCo");
            }

        }
        if (isIncoming && isInTeleportRange)
        {


            isIncoming = false;
        }
    }

    

    IEnumerator TeleportCo()
    {
        teleportTwin.GetComponent<Teleport>().isIncoming = true;
        Material usingMaterial = material;
        GameObject activeTeleport = objectToTeleport;
        activeTeleport.GetComponent<Playermovement>().enabled = false;
        float timePercentage = 0f;
        float fadeTime = 2f;
        
        while (timePercentage < 1f)
        {
            timePercentage += Time.deltaTime / fadeTime;
            float x = Mathf.Lerp(1f, 0f, timePercentage);
            usingMaterial.SetFloat("_Fade", x);

            yield return null;
        }
        activeTeleport.SetActive(false);
        activeTeleport.transform.position = new Vector3(teleportTwin.position.x, teleportTwin.position.y, teleportTwinLevel);
        activeTeleport.GetComponent<PlayerLevelChange>().InitializePlayerLocation();
        SetCollisionLayers.instance.SetCollisionLayer();
        yield return new WaitForSeconds(1f);

        activeTeleport.SetActive(true);

        timePercentage = 0f;
        
        while (timePercentage < 1f)
        {
            timePercentage += Time.deltaTime / fadeTime;
            float x = Mathf.Lerp(0f, 1f, timePercentage);
            usingMaterial.SetFloat("_Fade", x);

            yield return null;

        }
        activeTeleport.GetComponent<Playermovement>().enabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, teleportTwin.position);
    }
}
