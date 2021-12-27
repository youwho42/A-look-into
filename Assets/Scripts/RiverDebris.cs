using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverDebris : MonoBehaviour
{
    public float timeInWorld=30f;
    private void Start()
    {
        Invoke("DestroyDebris", timeInWorld);
    }

    void DestroyDebris()
    {
        Destroy(gameObject);
    }
}
