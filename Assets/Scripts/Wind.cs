using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour, IPoolPrefab
{
    public Transform windSprite;

    public SoundSet sound;
    AudioSource audioSource;
    
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    
    public void OnObjectSpawn()
    {
        var dir = WindManager.instance.GetWindDirectionFromPosition(transform.position).normalized;

        var d = dir.x < 0 ? Vector3.one : new Vector3(-1,1,1);
        windSprite.localScale = d;

        float angle = Vector2.SignedAngle(d.x == 1 ? Vector2.left : Vector2.right, dir);
        windSprite.rotation = Quaternion.Euler(0f, 0f, angle);

        if (audioSource != null)
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
