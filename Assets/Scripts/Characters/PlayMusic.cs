using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusic : MonoBehaviour
{
    public List<string> soundToPlay = new List<string>();
    public bool playOnStart;
    [Range(2f, 100f)]
    public float startDelay = 2f;
    AudioManager audioManager;
    int currentIndex;
    List<string> playedSongs = new List<string>();

    private IEnumerator Start()
    {
        audioManager = AudioManager.instance;
        yield return new WaitForSeconds(startDelay);
        if (playOnStart)
            PlayRecuringSound();
    }
    public void PlayRecuringSound()
    {
        //while (true)
        //{
        //    if (playedSongs.Count > 0)
        //    {
        //        if (audioManager.IsPlaying(playedSongs[playedSongs.Count - 1]))
        //            return;
        //    }
            
        //    for (int i = 0; i < soundToPlay.Count; i++)
        //    {
        //        if (currentIndex <= i)
        //            continue;
        //        audioManager.PlaySound(soundToPlay[i]);
        //        playedSongs.Add(soundToPlay[i]);
        //    }
        //    if(playedSongs.Count == soundToPlay.Count && !audioManager.IsPlaying(playedSongs[playedSongs.Count - 1]))
        //    {
        //        playedSongs.Clear();
        //    }
        //}
        
        

    }
    public void StopRecuringSound()
    {
        //audioManager.StopSound(playedSongs[playedSongs.Count - 1]);

    }
}
