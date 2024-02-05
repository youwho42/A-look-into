using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class FixingSounds : MonoBehaviour
{
    public AudioSource source;
    [SerializeField]
    public SoundSet soundSet;
    public float soundDuration = 6f;
    [Range(0f, 1f)]
    public float minSoundGap = 0.2f;
    [Range(0f, 1f)]
    public float maxSoundGap = 0.5f;
    public AudioMixerGroup mixerGroup;
    int lastSoundIndex = -1;
    bool timing;
    bool playSounds;
    //private void Start()
    //{
    //    SetSource();
    //}
    //private void OnEnable()
    //{
    //    SetSource();
    //}

    //private void SetSource()
    //{
    //    source = GetComponent<AudioSource>();
    //}

    public void StartSoundsWithTimer()
    {
        StopAllCoroutines();
        StartCoroutine(StartSoundsOnTimerCo());
    }
    public void StartSoundsNoTimer()
    {
        playSounds = true;
        StopAllCoroutines();
        StartCoroutine(StartSoundsNoTimerCo());
    }
    public void StopSoundsNoTimer()
    {
        playSounds = false;
    }

    IEnumerator SetTimer(float maxTime)
    {
        timing = true;
        float time = 0;
        while (time < maxTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        timing = false;
        yield return null;
    }
    IEnumerator StartSoundsOnTimerCo()
    {
        StartCoroutine(SetTimer(soundDuration));
        PlaySound();
        while(timing)
        {
            yield return new WaitForSeconds(Random.Range(minSoundGap, maxSoundGap));
            PlaySound();
            yield return null;
        }
        
    }

    IEnumerator StartSoundsNoTimerCo()
    {
        
        PlaySound();
        while (playSounds)
        {
            yield return new WaitForSeconds(Random.Range(minSoundGap, maxSoundGap));
            PlaySound();
            yield return null;
        }

    }

    void PlaySound()
    {
        
        int t = Random.Range(0, soundSet.clips.Length);
        if(t==lastSoundIndex)
        {
            PlaySound();
            return;
        }
        lastSoundIndex = t;
        soundSet.SetSource(source, t);
        soundSet.source.outputAudioMixerGroup = mixerGroup;
        soundSet.Play();
            
    }

       
}
