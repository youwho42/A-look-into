using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DetectPlayerVisibility : MonoBehaviour
{

    public SpriteRenderer playerSprite;

    
    Bounds playerBounds;
    public CurrentGridLocation currentLocation;
    
    private void Start()
    {
        playerBounds = playerSprite.bounds;
        currentLocation = GetComponent<CurrentGridLocation>();
    }

    private void Update()
    {
        if(currentLocation != null)
        {
            var t = currentLocation.GetCurrentGridLocation();

            t.y -= 1;
            t.z += 2;
            if (playerBounds.Intersects(currentLocation.groundMap.GetSprite(t).bounds))
            {
                Debug.Log("ooga booga");
            }
        }
        
    }

}
