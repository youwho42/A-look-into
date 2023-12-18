using System.Collections.Generic;
using UnityEngine;

public class AnimalAnimatorChoice : MonoBehaviour
{
    public List<RuntimeAnimatorController> animators = new List<RuntimeAnimatorController>();
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        int r = Random.Range(0, animators.Count);
        animator.runtimeAnimatorController = animators[r];
    }
}
