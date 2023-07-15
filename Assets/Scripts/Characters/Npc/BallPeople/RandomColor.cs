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
        float lastA = BallPeopleManager.instance.lastColorA;
        
        float r = 0;
        float d1 = 0;
        do
        {
            r = (float)BallPeopleManager.instance.random.Next(200, 500) / 1000.0f;
            r = (r + lastA) % 1.0f;
            d1 = Mathf.Abs(r - lastA);
           
        }
        while (d1 < 0.2f);


        
        BallPeopleManager.instance.lastColorA = r;


        sprite.color = Random.ColorHSV(r, r, 0.5f, 1.0f, 0.5f, 1.0f, 1f, 1f);
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
