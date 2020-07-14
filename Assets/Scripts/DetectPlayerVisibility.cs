using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectPlayerVisibility : MonoBehaviour
{

    public SpriteRenderer rend;


    private void Update()
    {
        if (!rend.isVisible)
        {
            Debug.Log("Sprite invisible");
        }
    }



}
