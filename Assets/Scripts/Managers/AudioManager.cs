using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound 
{
    public string name;
    public AudioClip clip;

    public bool loop;
    public bool overlap;

    [Range(0.0f, 1.0f)]
    public float volume = 0.7f;

    [Range(0.5f, 5.0f)]
    public float pitch = 1.0f;

    [Range(0.0f, 0.5f)]
    public float randomVolume = 0.1f;

    [Range(0.0f, 0.5f)]
    public float randomPitch = 0.1f;

    public AudioMixerGroup mixerGroup;
    public AudioSource source;
    [Space]
    [Header("3D Properties")]
    public bool is3D;
    public float minDistance = 0;
    public float maxDisance = 5;


    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.clip = clip;
        source.loop = loop;
        source.playOnAwake = false;
        if (is3D)
        {
            source.spatialBlend = 1;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.dopplerLevel = 0;
            source.minDistance = minDistance;
            source.maxDistance = maxDisance;
        }
    }

    public void Play()
    {
        source.volume = volume * (1 + Random.Range(-randomVolume / 2, randomVolume / 2));
        source.pitch = pitch * (1 + Random.Range(-randomPitch / 2, randomPitch / 2));
        if(!source.isPlaying && !overlap)
            source.Play();
        if(overlap)
            source.Play();
    }

    public void Stop()
    {
        source.Stop();
    }
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField]
    Sound[] sounds;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GameObject go = new GameObject("Audio");
        go.transform.parent = transform;

        for (int i = 0; i < sounds.Length; i++)
        {
            GameObject _go = new GameObject("Sound_" + i + "-" + sounds[i].name);
            _go.transform.parent = go.transform;
            sounds[i].SetSource(_go.AddComponent<AudioSource>());
            sounds[i].source.outputAudioMixerGroup = sounds[i].mixerGroup;
        }
    }
    public bool IsPlaying(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                bool soundIsPlaying;
                return soundIsPlaying = sounds[i].source.isPlaying;
            }

        }
        return false;
    }

    public void PlaySoundWithDelay(string _name, float _time)
    {
        StartCoroutine(PlaySoundDelayCo(_name, _time));
    }
    IEnumerator PlaySoundDelayCo(string _name, float _time)
    {
        yield return new WaitForSeconds(_time);
        PlaySound(_name);
    }

    public void PlaySound(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].Play();
                return;
            }
        }
        Debug.LogWarning("AudioManager: No sound found named " + _name + "!");
    }
    public void StopSound(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].Stop();
                return;
            }
        }
        Debug.LogWarning("AudioManager: No sound found named " + _name + "!");
    }

    public bool CompareSoundNames(string soundName)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if(sounds[i].name == soundName)
            {
                return true;
            }
        }
        return false;
    }

    public void StartAquiredAudio()
    {
        int num = Random.Range(1, 7);
        PlaySound($"UndertakingAdd{num}");
        //StartCoroutine("AquiredAudioCo");
    }
    IEnumerator AquiredAudioCo()
    {
        int t = 0;
        int last = 0;
        while (t < 2)
        {
            int num = Random.Range(1, 5);
            if(num != last)
            {
                last = num;
                PlaySound($"UndertakingAdd{num}");
                t++;
                yield return new WaitForSeconds(0.2f);
            }
            yield return null;
        }
        yield return null;
    }
    IEnumerator TaskAudioCo()
    {
        yield return null;
    }
    IEnumerator CompleteAudioCo()
    {
        yield return null;
    }
}
