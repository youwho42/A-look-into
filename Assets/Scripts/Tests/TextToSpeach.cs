using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Audio;
using System.Collections;


public class TextToSpeach : MonoBehaviour
{
    public static TextToSpeach instance;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(instance);
    }


    public Transform alphabetHolder;
    public Dictionary<char, Sound> alphabet = new Dictionary<char, Sound>();
    public AudioMixerGroup mixerGroup;
    public AudioMixer audioMixer;
    public string testString;
    [Range(1.2f, 2.0f)]
    public float testPitch = 1.75f;
    List<AudioClip> currentClips = new List<AudioClip>();
    private void Start()
    {
        GatherLetters();
        foreach (var so in alphabet)
        {
            if (so.Value == null)
                continue;
            GameObject _go = new GameObject("Sound_" + so.Key.ToString());

            _go.transform.parent = alphabetHolder;
            var source = _go.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = mixerGroup;
            source.clip = so.Value.clip;
            source.playOnAwake = false;
            source.loop = false;

            so.Value.source = source;
        }
       
    }
    
    void GatherLetters()
    {
        alphabet.Clear();
        var sounds = Resources.LoadAll<AudioClip>("Sounds/");
        for (int i = 0; i < sounds.Length; i++)
        {
            var l = (char)(97 + i);
            var s = new Sound();
            s.clip = sounds[i];
            s.loop = false;
            s.overlap = true;
            s.volume = 0.25f;
            s.pitch = 1.5f;
            s.randomVolume = 0.0f;
            s.randomPitch = 0.2f;
            s.mixerGroup = mixerGroup;
            alphabet.Add(l, s);
        }
    }

    [ContextMenu("Test String")]
    void Test()
    {
        StartCoroutine(PlayString(testString, testPitch));
    }

    public void ConvertToSpeach(string text, float pitch)
    {
        StartCoroutine(PlayString(text, pitch));
    }
    public void StopSpeach()
    {
        StopAllCoroutines();
    }

    IEnumerator PlayString(string text, float pitch) 
    {
        audioMixer.SetFloat("VoicesPitch", pitch);
        var s = text.ToLower();
        foreach (char c in s)
        {
            if (c == (char)39)
                continue;
            if(!alphabet.ContainsKey(c))
                yield return new WaitForSeconds(UnityEngine.Random.Range(0.06f, 0.14f));
            else
            {
                alphabet[c].Play();
                currentClips.Add(alphabet[c].clip);
                yield return new WaitForSeconds(alphabet[c].clip.length * 0.04f);
            }
        }

        yield return null;

    }
}
