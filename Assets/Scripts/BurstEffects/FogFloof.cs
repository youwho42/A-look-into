using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogFloof : MonoBehaviour
{
    public SpriteRenderer sprite;

    public void Fade(float amount)
    {
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, amount);
        
    }
   
   

}
