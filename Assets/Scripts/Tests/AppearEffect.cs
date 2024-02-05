using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearEffect : MonoBehaviour
{
    public Sound appearSound;
    AudioSource audioSource;
    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
        appearSound.SetSource(audioSource);
        Invoke("PlaySound", 0.05f);
    }
    public void PlaySound()
    {
        if (audioSource != null)
        {
            appearSound.Play();
        }
    }

    public void Disable()
    {
        Destroy(gameObject);
    }

}
