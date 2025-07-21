using UnityEngine;
using System.Collections.Generic;


public class SmallItemUnderShadow : MonoBehaviour
{
    public Transform mainItem;
    
    public Transform shadowHolder;

    public Vector3 shadowOffset;


    private void Start()
    {
        shadowHolder.localPosition = shadowOffset;
        float rot = Random.Range(0.0f, 360.0f);
        RotateSprites(rot);
        
    }

    private void RotateSprites(float rot)
    {
        var rotation = mainItem.transform.eulerAngles;
        rotation.z = rot;
        mainItem.eulerAngles = rotation;
        shadowHolder.eulerAngles = rotation;
    }

}
