using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeDropOnHeight : MonoBehaviour
{
    SpriteRenderer render;
    PurpleFireSheet fireSheet;


    private void Start()
    {
        fireSheet = GetComponentInParent<PurpleFireSheet>();
        render = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        float a = transform.localPosition.z;
        a = NumberFunctions.RemapNumber(a, 0, fireSheet.maxHeight, 1, 0);
        render.color = new Color(render.color.r, render.color.g, render.color.b, a);
    }
}
