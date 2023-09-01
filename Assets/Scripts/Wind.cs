using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour, IPoolPrefab
{
    
    AudioSource audioSource;
    float mainVolume;
    SpriteRenderer rend;
    void Start()
    {
        
        audioSource = GetComponent<AudioSource>();
        mainVolume = audioSource.volume;
    }
    void OnEnable()
    {
        rend = GetComponent<SpriteRenderer>();
    }
    public void OnObjectSpawn()
    {
        rend.flipX = Random.Range(0.0f, 1.0f) > 0.5f ? true : false;

        if(audioSource != null)
        {
            audioSource.volume = mainVolume;
            audioSource.Play();
        }
            
    }

   
    public void DeactivateObject()
    {
        this.gameObject.SetActive(false);
    }

}
