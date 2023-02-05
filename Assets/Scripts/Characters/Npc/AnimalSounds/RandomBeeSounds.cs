using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBeeSounds : MonoBehaviour
{
    public AudioClip[] clips;
    ButterflyAI stateMachine;
    public bool isMakingSound;

    AudioSource source;
    float mainVolume;
    private void Start()
    {
        //stateMachine = GetComponent<ButterflyAI>();
        source = GetComponent<AudioSource>();
        source.clip = clips[Random.Range(0, clips.Length)];
        mainVolume = source.volume;
        ChangeVolume();
        source.PlayDelayed(Random.Range(0.0f, 1.0f));
        isMakingSound = true;
        GameEventManager.onVolumeChangedEvent.AddListener(ChangeVolume);
    }
    private void OnDisable()
    {
        GameEventManager.onVolumeChangedEvent.RemoveListener(ChangeVolume);
    }

    void ChangeVolume()
    {
        source.volume = mainVolume * PlayerPreferencesManager.instance.GetTrackVolume(AudioTrack.Animals);
    }

    //public void PlaySound()
    //{
    //    source.Play();
    //    isMakingSound = true;
    //}

    //public void StopSound()
    //{
    //    source.Stop();
    //    isMakingSound = false;
    //}

}
