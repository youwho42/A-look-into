using Klaxon.StatSystem;
using System.Collections.Generic;
using UnityEngine;

public class FlumpOoze : MonoBehaviour
{
    public SpriteRenderer oozeSprite;
    public PolygonCollider2D coll;
    public List<Sprite> oozeSprites = new List<Sprite>();
    public SpriteRenderer stinkTrail;

    public SoundSet landSoundSet;
    AudioSource source;

    public void SetOoze(Transform parent, Color color, bool fromSave)
    {
        oozeSprite.sprite = oozeSprites[Random.Range(0, oozeSprites.Count-1)];
        oozeSprite.color = color;
        oozeSprite.flipX = Random.value < 0.5f;
        stinkTrail.color = color;
        stinkTrail.flipX = Random.value < 0.5f;
        float size = Random.Range(0.7f, 1.3f);
        stinkTrail.transform.localScale = new Vector3(size, size, size);
        var pointsList = new List<Vector2>();
        oozeSprite.sprite.GetPhysicsShape(0, pointsList);
        coll.points = pointsList.ToArray();
        if(!fromSave)
            PlaySound();
        if(parent.parent.TryGetComponent(out FlumpOozeContainer container))
        {
            container.SetOccupiedTile(transform.position, this);
        }
    }

    void PlaySound()
    {
        if(source == null)
            source = GetComponent<AudioSource>();
        int t = Random.Range(0, landSoundSet.clips.Length);
        landSoundSet.SetSource(source, t);
        landSoundSet.Play();
    }


}
