using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundSet
{
    
    public AudioClip[] clips;

    [Range(0.0f, 1.0f)]
    public float volume = 0.7f;

    [Range(0.5f, 1.5f)]
    public float pitch = 1.0f;

    [Range(0.0f, 0.5f)]
    public float randomVolume = 0.1f;

    [Range(0.0f, 0.5f)]
    public float randomPitch = 0.1f;

    AudioSource source;

    public void SetSource(AudioSource _source, int randomClip)
    {
        source = _source;
        source.clip = clips[randomClip];
        source.playOnAwake = false;
    }

    public void Play()
    {
        source.volume = volume * (1 + Random.Range(-randomVolume / 2, randomVolume / 2));
        source.pitch = pitch * (1 + Random.Range(-randomPitch / 2, randomPitch / 2));
        if (!source.isPlaying)
            source.Play();
    }

    public void Stop()
    {
        source.Stop();
    }
}








public class AnimalSounds : MonoBehaviour
{

    AudioSource source;
    [SerializeField]
    public SoundSet[] soundSets;

    float crowTimer;
    int timesToCrow;

    bool isCrying;
    public Vector2 minMaxBetweenCries;
    public bool continuous;
    public bool mute;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        crowTimer = Random.Range(minMaxBetweenCries.x, minMaxBetweenCries.y);
    }
    private void Update()
    {
        crowTimer -= Time.deltaTime;
        if(crowTimer <= 0 && !isCrying && !continuous && !mute)
        {
            timesToCrow = Random.Range(1, 4);
            SetCrySounds();
            isCrying = true;
        }
        if (continuous)
        {

        }
    }

   public void SetCrySounds()
    {
        int r = 0;
        if (soundSets.Length > 1)
            r = Random.Range(0, soundSets.Length);

        StartCoroutine(PlaySoundsCo(r, timesToCrow));
    }

    IEnumerator PlaySoundsCo(int AudioSet, int timesToCry)
    {
        int t = 0;
        while (t < timesToCry)
        {
            if (PlaySound(AudioSet))
                t++;
            yield return null;
        }
        isCrying = false;
        crowTimer = Random.Range(minMaxBetweenCries.x, minMaxBetweenCries.y);
    }

    bool PlaySound(int soundSet)
    {
        if (!source.isPlaying)
        {
            int t = Random.Range(0, soundSets[soundSet].clips.Length);
            soundSets[soundSet].SetSource(source, t);
            soundSets[soundSet].Play();
            return true;
        }
        return false;
    }
}


