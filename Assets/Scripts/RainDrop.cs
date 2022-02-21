using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainDrop : MonoBehaviour, IPoolPrefab
{
   
    public Animator animator;
    ItemGravity gravity;
    void Start()
    {
        gravity = GetComponent<ItemGravity>();
        
    }

    void Update()
    {
        if (gravity.isGrounded && animator.GetCurrentAnimatorStateInfo(0).IsName("RainFall"))
        {
            animator.SetTrigger("HitGround");
            
        }
            

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("RainHit"))
        {
            if (!AnimatorIsPlaying())
            {
                DeactivateObject();
            }
        }


    }
    bool AnimatorIsPlaying() 
    { 
        return animator.GetCurrentAnimatorStateInfo(0).length > animator.GetCurrentAnimatorStateInfo(0).normalizedTime; 
    }

    public void DeactivateObject()
    {
        
        /*if (gravity != null)
            gravity.ResetStartingPosition(10);*/
        this.gameObject.SetActive(false);
       
    }

    public void OnObjectSpawn()
    {
        
    }
}
