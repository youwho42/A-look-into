using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class RandomColor : MonoBehaviour
{
    public Color randomColor;

    public SpriteRenderer sprite;
    
    List<float> values = new List<float>();

    private void Start()
    {

        //SetRandomColor();
    }

    float ChooseValue(int min, int max)
    {
        System.Random random = new System.Random();
        //Pick a level randomly
        float r = random.Next(min, max);
        r = r / 1000;
        return r;
        //return Random.Range(min, max);
    }

    public void SetRandomColor()
    {
        values.Add(ChooseValue(0, 150));
        values.Add(ChooseValue(150, 1000));
        values.Add(ChooseValue(500, 1000));
        var rand = new System.Random();
        values = values.OrderBy(c => rand.Next()).ToList();
        randomColor.r = values[0];
        randomColor.g = values[1];
        randomColor.b = values[2];
        randomColor.a = 1;
        sprite.color = randomColor;
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
