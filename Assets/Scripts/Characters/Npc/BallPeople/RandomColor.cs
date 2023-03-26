using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class RandomColor : MonoBehaviour
{
    public Color randomColor;

    public SpriteRenderer sprite;
    
    public void SetRandomColor()
    {
        sprite.color = Random.ColorHSV(0.0f, 1.0f, 0.5f, 1.0f, 0.5f, 1.0f, 1f, 1f);
        randomColor = sprite.color;
    }

    public void SetColor(float r, float g, float b)
    {
        randomColor.r = r;
        randomColor.g = g;
        randomColor.b = b;
        randomColor.a = 1;
        sprite.color = randomColor;
    }
}
