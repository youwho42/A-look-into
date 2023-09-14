using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class AnimalAnimatorChoice : MonoBehaviour
{
    public List<AnimatorController> animators = new List<AnimatorController>();
    Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
        int r = Random.Range(0, animators.Count);
        animator.runtimeAnimatorController = animators[r];
    }
}
