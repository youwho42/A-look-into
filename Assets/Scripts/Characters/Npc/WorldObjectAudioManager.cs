using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObjectAudioManager : MonoBehaviour
{
    [SerializeField]
    Sound[] sounds;
    public Vector3 sound3dOffset;
    
    private void Start()
    {
        GameObject audioHolder = new GameObject("Audio");
        audioHolder.transform.parent = transform;
        audioHolder.transform.localPosition = sound3dOffset;
        for (int i = 0; i < sounds.Length; i++)
        {
            GameObject _go = new GameObject("Sound_" + i + "-" + sounds[i].name);
            _go.transform.parent = audioHolder.transform;
            _go.transform.localPosition = Vector3.zero;
            sounds[i].SetSource(_go.AddComponent<AudioSource>());
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
        Debug.LogWarning("WorldObjectAudioManager: No sound found named " + _name + "!");
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
        Debug.LogWarning("WorldObjectAudioManager: No sound found named " + _name + "!");
    }

    public bool CompareSoundNames(string soundName)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == soundName)
            {
                return true;
            }
        }
        return false;
    }
}
