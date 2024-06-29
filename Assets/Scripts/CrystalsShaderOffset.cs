using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalsShaderOffset : MonoBehaviour
{
    SpriteRenderer sprite;
    private void Start()
    {
        
        sprite = GetComponent<SpriteRenderer>();
        var mat = sprite.material;
        mat.SetFloat("_Offset", Random.Range(0f, 100f));
    }
}
