using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_PlayAnimationSounds : MonoBehaviour
{
    public SoundSet[] soundSet;
    public AudioSource source;

    

    void Start()
    {
        if (source == null)
            source = GetComponent<AudioSource>();
    }




    public void PlayRandomSoundFromSet(int setIndex)
    {
        int t = Random.Range(0, soundSet[setIndex].clips.Length);
        soundSet[setIndex].SetSource(source, t);
        soundSet[setIndex].Play();
    }

    public void PlaySoundFromSet(int setIndex)
    {
        soundSet[setIndex].SetSource(source, 0);
        soundSet[setIndex].Play();
    }






    

}
