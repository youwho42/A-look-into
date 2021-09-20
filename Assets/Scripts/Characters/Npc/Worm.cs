using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm : MonoBehaviour
{

    Animator animator;
    public GameObject worm;

    
    CircleCollider2D peckCollider;

    float emergeTime;
    float wiggleTime;
    public Vector2 minMaxBetweenWiggles;
    bool isEmerged;
    bool isEaten;

    private void Start()
    {
        animator = worm.GetComponent<Animator>();
        peckCollider = GetComponent<CircleCollider2D>();
        
        isEmerged = false;
        ResetWiggleTime();
        worm.SetActive(false);
    }

   
    private void Update()
    {
        if(!isEmerged && !isEaten)
        {
            emergeTime -= Time.deltaTime;
            if(emergeTime <= 0)
            {
                ForceWormEmerge();
            }
        }
        if (isEaten)
        {
            emergeTime -= Time.deltaTime;
            if (emergeTime <= 0)
            {
                ResetWorm();
            }
        }
    }

    private void ResetWiggleTime()
    {
        emergeTime = Random.Range(minMaxBetweenWiggles.x, minMaxBetweenWiggles.y) * 3f;
        wiggleTime = Random.Range(minMaxBetweenWiggles.x, minMaxBetweenWiggles.y);
    }

    public void ForceWormEmerge()
    {
        if (!isEmerged)
        {
            StartCoroutine(StartWormAnimationCo());
        }
    }
        
    public void WormEaten()
    {
        peckCollider.enabled = false;
        worm.SetActive(false);
        isEaten = true;
        emergeTime = 60.0f;
        
    }

    void ResetWorm()
    {
        peckCollider.enabled = true;
        
        ResetWiggleTime();
        isEaten = false;
    }

    IEnumerator StartWormAnimationCo()
    {
        isEmerged = true;
        worm.SetActive(true);
        yield return new WaitForSeconds(wiggleTime);
        animator.SetTrigger("Hide");
        yield return new WaitForSeconds(3f);
        worm.SetActive(false);
        ResetWiggleTime();
        isEmerged = false;
    }
  
   
}
