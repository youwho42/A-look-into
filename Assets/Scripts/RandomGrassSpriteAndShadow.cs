using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGrassSpriteAndShadow : MonoBehaviour
{
    [System.Serializable]
    public struct Grass
    {
        public Sprite grass;
        public Sprite shadow;
    }

    public SpriteRenderer mainGrass;
    public SpriteRenderer mainShadow;

    public List<Grass> grassList = new List<Grass>();

    private void Start()
    {
        SetRandomSprites();
    }

    private void SetRandomSprites()
    {
        int i = UnityEngine.Random.Range(0, grassList.Count);
        mainGrass.sprite = grassList[i].grass;
        mainShadow.sprite = grassList[i].shadow;
    }
}
