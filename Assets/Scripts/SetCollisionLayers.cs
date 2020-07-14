using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SetCollisionLayers : MonoBehaviour
{
    public static SetCollisionLayers instance;
    public List<Tilemap> collisionTilemap = new List<Tilemap>();
    private PlayerLevelChange player;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        } else
        {
            instance = this;
        }
        player = FindObjectOfType<PlayerLevelChange>();
        //player.playerChangeLevelEvent.AddListener(SetCollisionLayer);
    }

    public void SetCollisionLayer()
    {
        for (int i = 0; i < collisionTilemap.Count; i++)
        {
            if (i == player.transform.position.z - 1)
            {
                collisionTilemap[i].gameObject.SetActive(true);
            }
            else
            {
                collisionTilemap[i].gameObject.SetActive(false);
            }
        }
    }
    //private void OnDisable()
    //{
    //    player.playerChangeLevelEvent.RemoveListener(SetCollisionLayer);
    //}
}
