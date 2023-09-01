using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerAnimationSoundManager : MonoBehaviour
{
    public List<string> soundToPlay = new List<string>();
    AudioManager audioManager;
    public LayerMask groundSoundsLayer;
    public Vector3 offset;

    AudioSource source;

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
        audioManager = AudioManager.instance;
        source = GetComponent<AudioSource>();
    }

    bool PlaySound(SoundSet[] soundSet)
    {
        if (!source.isPlaying)
        {
            int t = UnityEngine.Random.Range(0, soundSet.Length-1);
            soundSet[t].SetSource(source, t);
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

    
    public void PlayFootstepSound()
    {
        bool right = PlayerInformation.instance.playerController.facingRight;
        Vector3 newOff = right ? offset : -offset;
        Collider2D hit = Physics2D.OverlapCircle(transform.position + newOff , .01f, groundSoundsLayer);
        PlayFootstep(hit);

    }


    void PlayFootstep(Collider2D hit)
    {
        if (hit != null)
        {

            for (int i = 0; i < soundToPlay.Count; i++)
            {
                if (soundToPlay[i] == hit.GetComponent<WalkableSound>().walkableSoundName)
                {
                    audioManager.PlaySound(soundToPlay[i]);
                    return;
                }
            }
        }
        audioManager.PlaySound(soundToPlay[0]);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + offset, 0.01f);
        Gizmos.DrawWireSphere(transform.position - offset, 0.01f);
    }
}
