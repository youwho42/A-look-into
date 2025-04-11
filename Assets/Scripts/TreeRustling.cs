using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeRustling : MonoBehaviour, IWindEffect
{
    public SoundSet soundSet;
    AudioSource source;
    public DrawZasYDisplacement treeCollision;

    public Sprite treeDropppingSprite;

    public float dropRadius;
    public TreeLeavesShake treeLeavesShake;


    void Start()
    {
        source = GetComponent<AudioSource>();
        
    }

    public void Affect(bool canDropItem)
    {
        if (!isActiveAndEnabled)
            return;
        int t = Random.Range(0, soundSet.clips.Length);
        soundSet.SetSource(source, t);
        source.volume = soundSet.volume;
        soundSet.Play();
        treeLeavesShake.ShakeLeaves();
        if (canDropItem)
            DropDropping();
    }

    void DropDropping()
    {
        int r = Random.Range(1, 6);
        for (int i = 0; i < r; i++)
        {
            var pos = Random.insideUnitCircle * dropRadius;
            var t = TreeDroppingManager.instance.GetDropping();
            t.SetDropping(transform.position + (Vector3)pos, treeCollision.positionZ, treeDropppingSprite);
        }

    }
    
}
