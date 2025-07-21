using UnityEngine;
using System.Collections.Generic;


public class FootstepObject : MonoBehaviour
{
    int footstepTimer;
    public float FootstepTimer { get { return footstepTimer; } }
    float maxTimer;
    public SpriteRenderer footstepSprite;

    
   
    public void SetNewTimer(float amount)
    {
        maxTimer = amount;
        footstepTimer = 0;
        footstepSprite.color = Color.white;
    }

    public void TimerTick()
    {
        footstepTimer++;
        var c = footstepSprite.color;
        float a = Mathf.Lerp(1.0f, 0.0f, footstepTimer / maxTimer);
        c.a = a;
        footstepSprite.color = c;

    }


   
}
