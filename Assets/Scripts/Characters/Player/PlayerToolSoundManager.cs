using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerToolSoundManager : MonoBehaviour
{
    //AudioManager audioManager;
    
    AudioSource source;
    float mainVolume;

    [Serializable]
    public struct ToolSounds
    {
        [SerializeField]
        public SoundSet[] soundSets;
        [SerializeField]
        public QI_ItemData[] item;
    }
    [Space]
    public List<ToolSounds> toolSounds = new List<ToolSounds>();

    void Start()
    {
        source = GetComponent<AudioSource>();
    }
    

    

    bool PlaySound(SoundSet[] soundSet)
    {
        if (!source.isPlaying)
        {
            int t = UnityEngine.Random.Range(0, soundSet.Length - 1);
            soundSet[t].SetSource(source, t);
            mainVolume = soundSet[t].volume;
            soundSet[t].Play();

            return true;
        }
        return false;
    }
    public void PlayToolSound()
    {
        if (MiniGameManager.instance.gameStarted)
            return;
        var equiped = EquipmentManager.instance.currentEquipment[(int)EquipmentSlot.Hands];
        foreach (var tool in toolSounds)
        {
            for (int i = 0; i < tool.item.Length; i++)
            {
                if (tool.item[i] == equiped)
                {
                    PlaySound(tool.soundSets);
                }
            }
        }
    }
}
