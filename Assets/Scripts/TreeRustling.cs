using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeRustling : MonoBehaviour, IWindEffect
{
    public SoundSet soundSet;
    AudioSource source;
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void Affect()
    {
        int t = Random.Range(0, soundSet.clips.Length);
        soundSet.SetSource(source, t);
        soundSet.Play();
    }

    
}
