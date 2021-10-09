using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour, IPoolPrefab
{
    Animator animator;
    AudioSource audioSource;
    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

    }
    public void OnObjectSpawn()
    {
        if(audioSource != null)
            audioSource.Play();
    }

    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            DeactivateObject();
        }
    }
    public void DeactivateObject()
    {
        this.gameObject.SetActive(false);
    }

}
