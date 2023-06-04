using System.Collections;
using System.Collections.Generic;

using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class FixingSounds : MonoBehaviour
{
    AudioSource source;
    [SerializeField]
    public SoundSet soundSet;
    float mainVolume;
    int lastSoundIndex = -1;
    bool timing;
    private void Start()
    {
        source = GetComponent<AudioSource>();
        GameEventManager.onVolumeChangedEvent.AddListener(ChangeVolume);
        
    }
    private void OnDisable()
    {
        GameEventManager.onVolumeChangedEvent.RemoveListener(ChangeVolume);
    }
    public void StartSounds()
    {
        StartCoroutine(StartSoundsCo());
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
    IEnumerator StartSoundsCo()
    {
        StartCoroutine(SetTimer(6f));
        PlaySound();
        while(timing)
        {
            yield return new WaitForSeconds(Random.Range(0.2f, 0.5f));
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
        mainVolume = soundSet.volume;
        ChangeVolume();
        soundSet.Play(AudioTrack.Effects);
            
    }

    void ChangeVolume()
    {
        source.volume = mainVolume * PlayerPreferencesManager.instance.GetTrackVolume(AudioTrack.Effects);
    }

}
