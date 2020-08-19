using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BeginFirstGame : MonoBehaviour
{

    public CinemachineVirtualCamera startCam;
    public GameObject player;
    public Teleport teleport;

    public void BeginGame()
    {
        StartCoroutine("BeginGameCo");
    }

    

    IEnumerator BeginGameCo()
    {
        PlayerInput input = player.GetComponent<PlayerInput>();
        teleport.objectToTeleport = player;
        teleport.material = player.GetComponentInChildren<SpriteRenderer>().material;
        teleport.StartTeleport();
        yield return new WaitForSeconds(1.9f);
        startCam.Priority = 0;
        yield return new WaitForSeconds(5f);
        
        input.enabled = true;
    }

}
