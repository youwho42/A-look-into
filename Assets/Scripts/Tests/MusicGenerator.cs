using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    Animal

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
    public struct SoundAssociations
    {
        public SoundType type;
        public Sound sound;
        public int maxTypeAmount;
        public float maxVolume;
    }

    [Range(0.0f, 1.0f)]
    public float mainVolume;
    public List<SoundAssociations> soundAssociations = new List<SoundAssociations>();
    Dictionary<SoundType, int> possibleSounds = new Dictionary<SoundType, int>();

    private void Start()
    {
        
        for (int i = 0; i < Enum.GetValues(typeof(SoundType)).Length; i++)
        {
            possibleSounds.Add((SoundType)i, 0);
        }

        GameObject go = new GameObject("Audio");
        go.transform.parent = transform;
        for (int i = 0; i < soundAssociations.Count; i++)
        {
            GameObject _go = new GameObject("Sound_" + i + "-" + soundAssociations[i].sound.name);
            _go.transform.parent = go.transform;
            soundAssociations[i].sound.SetSource(_go.AddComponent<AudioSource>());
        }
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
        if (mainVolume == 0)
            return;
        foreach (var item in possibleSounds)
        {
            if(item.Value > 0)
            {
                for (int i = 0; i < soundAssociations.Count; i++)
                {
                    if(item.Key == soundAssociations[i].type)
                    {
                        int q = item.Value > soundAssociations[i].maxTypeAmount ? soundAssociations[i].maxTypeAmount : item.Value;
                        float max = q;
                        max = MapNumber.Remap(max, 1, soundAssociations[i].maxTypeAmount, 0.01f, soundAssociations[i].maxVolume);
                        soundAssociations[i].sound.source.volume = max * mainVolume;
                        if (!soundAssociations[i].sound.source.isPlaying)
                            soundAssociations[i].sound.Play();
                    }
                }
            }
            else
            {
                for (int i = 0; i < soundAssociations.Count; i++)
                {
                    if (item.Key == soundAssociations[i].type)
                    {
                        if (soundAssociations[i].sound.source.isPlaying)
                            soundAssociations[i].sound.Stop();
                    }
                }
            }
        }
    }


   
}
