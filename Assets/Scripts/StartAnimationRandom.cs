using UnityEngine;

public class StartAnimationRandom : MonoBehaviour
{
    public Animator animator;
    private void Start()
    {
        float randomIdleStart = Random.Range(0, animator.GetCurrentAnimatorStateInfo(0).length);
        animator.Play(0, 0, randomIdleStart);
    }
}
