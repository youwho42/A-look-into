using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public enum SoundType
{
    PineTree,
    SquareleafTree,
    ColumnarTree,
    Bush,
    DeadTree,
    Rock,
    Clay,
    Butterfly,
    Mouse,
    Sparrow,
    Squirrel,
    Bee,
    Firefly,
    Seaweed,
    Crow,
    Chicken,
    Pigeon

}
public class MusicGenerator : MonoBehaviour
{

    public static MusicGenerator instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
                
    }
    [Serializable]
    public class SoundAssociations
    {
        public SoundType type;
        public Sound[] sounds;
        public int maxTypeAmount;
        public float maxVolume;
        public bool syncsToTick;
        [HideInInspector]
        public bool isPlaying;
        [HideInInspector]
        public bool isStopping;
        [HideInInspector]
        public AudioSource currentSource;
        [HideInInspector]
        public float currentVolume;
    }

    public AudioMixerGroup mixerGroup;
    public List<SoundAssociations> soundAssociations = new List<SoundAssociations>();
    Dictionary<SoundType, int> possibleSounds = new Dictionary<SoundType, int>();
    Queue<AudioSource> audioSourcesQueue = new Queue<AudioSource>();
    [Range(1, 2)]
    public int soundQueueBeat;

    

    private void Start()
    {
        AudioListener.volume = 0;
        GameEventManager.onTimeTickEvent.AddListener(PlaySoundQueue);
        
        for (int i = 0; i < Enum.GetValues(typeof(SoundType)).Length; i++)
        {
            possibleSounds.Add((SoundType)i, 0);
        }

        GameObject go = new GameObject("Music");
        go.transform.parent = transform;
        for (int i = 0; i < soundAssociations.Count; i++)
        {
            foreach (var sound in soundAssociations[i].sounds)
            {
                GameObject _go = new GameObject("Sound_" + i + "-" + sound.clip.name);
                _go.transform.parent = go.transform;
                sound.SetSource(_go.AddComponent<AudioSource>());
                sound.source.outputAudioMixerGroup = mixerGroup;
            }
        }
        StartCoroutine(FadeInOnStart());
    }

    private void OnDisable()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(PlaySoundQueue);
    }

    public void AddToDictionary(SoundType type)
    {

        if (possibleSounds[type] > 20)
            return;
        possibleSounds[type] += 1;
    }

    public void RemoveFromDictionary(SoundType type)
    {
        if (possibleSounds[type] <= 0)
            return;
        possibleSounds[type] -= 1;

    }



    private void Update()
    {
        
        foreach (var item in possibleSounds)
        {
            if(item.Value > 0)
            {
                for (int i = 0; i < soundAssociations.Count; i++)
                {
                    if(item.Key == soundAssociations[i].type)
                    {
                        StartSound(item.Value, soundAssociations[i]);
                    }
                }
            }
            else
            {
                for (int i = 0; i < soundAssociations.Count; i++)
                {
                    if (item.Key == soundAssociations[i].type && soundAssociations[i].isPlaying)
                    {
                        StartCoroutine(FadeOutOnStop(soundAssociations[i]));
                    }
                }
            }
        }
    }

    void StartSound(int itemValue, SoundAssociations soundAssociation)
    {
        if(soundAssociation.isStopping)
            soundAssociation.isStopping = false;

        float max = GetNewVolume(itemValue, soundAssociation);
        if (soundAssociation.currentSource != null) { 
            soundAssociation.currentSource.volume = max;
        }
            

        int t = UnityEngine.Random.Range(0, soundAssociation.sounds.Length);

        if (!soundAssociation.isPlaying)
        {
            PlaySound(soundAssociation, t, max);
            soundAssociation.isPlaying = true;
        }
        else if (soundAssociation.isPlaying && !soundAssociation.currentSource.isPlaying && Time.timeScale != 0)
        {
            PlaySound(soundAssociation, t, max);
        }


    }

    void PlaySound(SoundAssociations soundAssociation, int index, float vol)
    {
        soundAssociation.sounds[index].source.volume = vol;
        soundAssociation.currentSource = soundAssociation.sounds[index].source;
        if (!soundAssociation.syncsToTick)
            soundAssociation.sounds[index].source.Play();
        else
            audioSourcesQueue.Enqueue(soundAssociation.currentSource);
    }

    float GetNewVolume(int itemValue, SoundAssociations soundAssociation)
    {
        int q = itemValue > soundAssociation.maxTypeAmount ? soundAssociation.maxTypeAmount : itemValue;
        float max = q;
        max = NumberFunctions.RemapNumber(max, 0, soundAssociation.maxTypeAmount, 0.01f, soundAssociation.maxVolume);
        return max;
    }


    void PlaySoundQueue(int tick)
    {
        if(tick % soundQueueBeat == 0)
        {
            while (audioSourcesQueue.Count > 0)
            {
                var a = audioSourcesQueue.Dequeue();
                a.Play();
            }
        }
        
    }
    

    IEnumerator FadeOutOnStop(SoundAssociations soundAssociation)
    {
        if (soundAssociation.currentSource.isPlaying)
        {
            soundAssociation.isStopping = true;
            float startVolume = soundAssociation.currentSource.volume;
            float elapsedTime = 0;
            float waitTime = 5;
            float vol = 0;
            while (elapsedTime < waitTime && soundAssociation.isStopping)
            {
                vol = Mathf.Lerp(startVolume, 0, (elapsedTime / waitTime));
                elapsedTime += Time.deltaTime;
                soundAssociation.currentSource.volume = vol;

                yield return null;
            }

            soundAssociation.currentSource.Stop();
            soundAssociation.currentSource.volume = startVolume;
            soundAssociation.isPlaying = false;
        }

    }

    IEnumerator FadeInOnStart()
    {

        float elapsedTime = 0;
        float waitTime = 15;
        float vol = 0;
        while (elapsedTime < waitTime)
        {
            vol = Mathf.Lerp(0, 1, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;
            AudioListener.volume = vol;
            
            yield return null;
        }
    }


}
