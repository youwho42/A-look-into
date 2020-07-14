using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerWorm : MonoBehaviour
{
    Animator animator;
    public LayerMask wormLayer;
    public Transform peckPoint;
    public float peckPointRadius;
    bool isPecking;
    float timer = 0;
    Worm worm;

    private void Start()
    {
        animator = GetComponent<Animator>();

    }

    private void Update()
    {
        Collider2D hit = Physics2D.OverlapCircle(peckPoint.position, peckPointRadius, wormLayer);
        if (hit != null && !isPecking)
        {
            worm = hit.GetComponent<Worm>();
            worm.ForceWormEmerge();
            animator.SetTrigger("Peck");
            isPecking = true;
            
        }
        if (isPecking)
        {
            timer += Time.deltaTime;
            if (timer >= 2)
            {
                
                isPecking = false;
                timer = 0;
            }
        }
        
    }

    public void EatWorm()
    {
        if (isPecking && worm != null)
        {
            
            worm.WormEaten();
            
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(peckPoint.position, peckPointRadius);
    }
}
