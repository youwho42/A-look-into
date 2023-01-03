using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_AnimationToggle : MonoBehaviour
{

    public Animator animator;
    public AnimateNPC animateNPC;

    private void OnBecameInvisible()
    {
        animateNPC.enabled = false;
        animator.enabled = false;
    }
    private void OnBecameVisible()
    {
        animateNPC.enabled = true;
        animator.enabled = true;
    }

}
