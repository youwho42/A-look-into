using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableColliderOffScreen : MonoBehaviour
{

    Collider2D coll;
    private void Start()
    {
        coll = GetComponent<Collider2D>();
        if(coll == null)
            coll = GetComponentInParent<Collider2D>();
        coll.enabled = false;
    }

    private void OnBecameVisible()
    {
        coll.enabled = true;
    }

    private void OnBecameInvisible()
    {
        coll.enabled = false;
    }

}
