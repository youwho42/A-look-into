using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectAreaHit : MonoBehaviour
{
    
    public bool isInArea;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isInArea = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isInArea = false;
    }

}
