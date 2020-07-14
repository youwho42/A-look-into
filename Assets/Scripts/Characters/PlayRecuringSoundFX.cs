using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRecuringSoundFX : MonoBehaviour
{
    public string soundToPlay;
    public bool playOnStart;
    [Range(2f,100f)]
    public float startDelay = 2f;
    AudioManager audioManager;

    private IEnumerator Start()
    {
        audioManager = AudioManager.instance;
        yield return new WaitForSeconds(startDelay);
        if (playOnStart)
            PlayRecuringSound();
    }
    public void PlayRecuringSound()
    {
        audioManager.PlaySound(soundToPlay);

    }
    public void StopRecuringSound()
    {
        audioManager.StopSound(soundToPlay);

    }


}
