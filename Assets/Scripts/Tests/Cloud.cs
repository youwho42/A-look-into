using UnityEngine;
using System.Collections.Generic;


public class Cloud : MonoBehaviour
{


    private void OnTriggerEnter2D(Collider2D collision)
    {
        var other = collision.GetComponentInChildren<TreeShadows>();
        if(other != null)
        {
            //other.isUnderCloud = true;
            other.shadowTransform.gameObject.SetActive(false);
            other.nightShadows.gameObject.SetActive(false);
            
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        var other = collision.GetComponentInChildren<TreeShadows>();
        if (other != null)
        {
            //other.isUnderCloud = false;
            other.shadowTransform.gameObject.SetActive(true);
            other.nightShadows.gameObject.SetActive(true);
            other.SetShadows(other.shadowUpdateTick);
            other.CheckForLights();
        }
    }
}
