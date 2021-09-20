using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour, IPooledObject
{
    AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

    }
    public void OnObjectSpawn()
    {
        audioSource.Play();
    }
}
