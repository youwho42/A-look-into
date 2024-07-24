using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPersonAnimManager : MonoBehaviour
{
    AudioSource source;
    [SerializeField]
    public SoundSet IndieManiaText;

    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void PlayIndieManiaText()
    {
        if (!source.isPlaying)
        {
            IndieManiaText.SetSource(source, 0);
            IndieManiaText.Play();
        }
    }
}
