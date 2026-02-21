using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour, IPoolPrefab
{
    public SoundSet sound;
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
        //rend.flipX = Random.Range(0.0f, 1.0f) > 0.5f ? true : false;

        if(audioSource != null)
        {
            int r = Random.Range(0, sound.clips.Length);
            audioSource.clip = sound.clips[r];
            audioSource.volume = sound.volume;
            audioSource.pitch = sound.pitch + Random.Range(-sound.randomPitch, sound.randomPitch);
            audioSource.Play();
        }
            
    }


    public void DeactivateObject()
    {
        this.gameObject.SetActive(false);
    }

}
