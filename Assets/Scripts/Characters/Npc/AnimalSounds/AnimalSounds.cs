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
    public bool overlap;

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
        if (!source.isPlaying && !overlap)
            source.Play();
        if (overlap)
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

    float cawTimer;
    public Vector2 minMaxTimesToCry;
    int timesToCaw;

    public bool isCrying;
    public Vector2 minMaxBetweenCries;
    public bool continuous;
    public bool mute;
    public float maxSpeachPause;

    public AnimationCurve curve;
    public Animator animator;

    private void Start()
    {
        
        source = GetComponent<AudioSource>();
        SetTimesToCry();
        SetFirstCry();
        
    }
    private void Update()
    {
        cawTimer -= Time.deltaTime;
        if(cawTimer <= 0 && !isCrying && !continuous && !mute)
        {
            SetTimesToCry();
            SetCrySounds();
            isCrying = true;
        }
       
    }

   public void SetCrySounds()
    {
        int r = 0;
        if (soundSets.Length > 1)
            r = Random.Range(0, soundSets.Length);

        StartCoroutine(PlaySoundsCo(r, timesToCaw));
    }

    IEnumerator PlaySoundsCo(int AudioSet, int timesToCry)
    {
        int t = 0;
        while (t < timesToCry)
        {
            if (PlaySound(AudioSet))
                t++;
                
            yield return new WaitForSeconds(Random.Range(0.0f, maxSpeachPause));
        }
        isCrying = false;
        SetNextCry();
        
    }

    void SetFirstCry()
    {
        cawTimer = Random.Range(minMaxBetweenCries.y/2, minMaxBetweenCries.y);
    }

    void SetNextCry()
    {
        cawTimer = (curve.Evaluate(Random.Range(0.0f, 1.0f)) * (minMaxBetweenCries.y - minMaxBetweenCries.x)) + minMaxBetweenCries.x;
    }
    void SetTimesToCry()
    {
        var t = (curve.Evaluate(Random.Range(0.0f, 1.0f))) * (minMaxTimesToCry.y - minMaxTimesToCry.x) + minMaxTimesToCry.x;
        
        timesToCaw = (int)t;
    }

    bool PlaySound(int soundSet)
    {
        if (!source.isPlaying)
        {
            int t = Random.Range(0, soundSets[soundSet].clips.Length);
            soundSets[soundSet].SetSource(source, t);
            soundSets[soundSet].Play();
            animator.SetTrigger("Caw");
            
            
            return true;
        }
        return false;
    }
}


