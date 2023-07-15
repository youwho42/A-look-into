using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Playables;
using TMPro;

public class StartGame : MonoBehaviour
{

    public CinemachineVirtualCamera startCam;
    public GameObject player;
    public SpriteRenderer shadow;
    public Transform introPosition, startPosition;
    public PlayableDirector director;
    PlayerInputController input;
    Material material;
    
    
    private void Start()
    {
        input = player.GetComponent<PlayerInputController>();
        material = player.GetComponentInChildren<SpriteRenderer>().material;
        material.SetFloat("_Fade", 0);
        
        player.transform.position = introPosition.position;
        director.Stop();
        
    }

    
    // Start the beginning game timeline and make the player appear
    public void StartBeginningTimeline()
    {
        player.transform.position = startPosition.position;
        director.Play();
    }


    public void BeginGame(bool isNewGame)
    {
        
        if (isNewGame)
            StartCoroutine("BeginNewGameCo");
        else
            StartCoroutine("BeginLoadGameCo");
    }

    IEnumerator BeginNewGameCo()
    {
        
        float timeToWait = 2f;
        
        
        yield return new WaitForSeconds(3f);
        DissolveEffect.instance.StartDissolve(material, timeToWait, true);
        shadow.enabled = true;
        startCam.Priority = 0;
        yield return new WaitForSeconds(4f);
        input.enabled = true;
    }
    IEnumerator BeginLoadGameCo()
    {

        float timeToWait = 2f;

        yield return new WaitForSeconds(3f);
        DissolveEffect.instance.StartDissolve(material, timeToWait, true);
        shadow.enabled = true;
        startCam.Priority = 0;
        yield return new WaitForSeconds(4f);
        
    }

    

    

    
}
