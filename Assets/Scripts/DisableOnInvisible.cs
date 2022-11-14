using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnInvisible : MonoBehaviour
{

    public bool isVisible;
    
    void OnBecameVisible()
    {
        isVisible = true;
        
    }
    void OnBecameInvisible()
    {
        isVisible = false;
        
    }

}
