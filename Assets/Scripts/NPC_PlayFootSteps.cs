using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_PlayFootSteps : MonoBehaviour
{
    public SoundSet soundSet;
    AudioSource source;
    float mainVolume;
    void Start()
    {
        source = GetComponent<AudioSource>();
        mainVolume = source.volume;
    }

    public void PlayFootStep()
    {
        int t = Random.Range(0, soundSet.clips.Length);

        soundSet.SetSource(source, t);
        source.volume = mainVolume * PlayerPreferencesManager.instance.GetTrackVolume(AudioTrack.Effects);
        soundSet.Play(AudioTrack.Effects);
    }

    
}
