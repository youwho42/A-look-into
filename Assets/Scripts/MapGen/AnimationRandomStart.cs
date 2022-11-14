using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationRandomStart : MonoBehaviour
{
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        float randomIdleStart = Random.Range(0, animator.GetCurrentAnimatorStateInfo(0).length);
        animator.Play(0, 0, randomIdleStart);
    }

    
}
