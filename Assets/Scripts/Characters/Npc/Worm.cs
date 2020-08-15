using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm : MonoBehaviour
{

    Animator animator;
    public GameObject worm;

    
    CircleCollider2D collider2D;

    float emergeTime;
    float wiggleTime;
    public Vector2 minMaxBetweenWiggles;
    bool isEmerged;
    bool isEaten;

    private void Start()
    {
        animator = worm.GetComponent<Animator>();
        collider2D = GetComponent<CircleCollider2D>();
        
        isEmerged = false;
        ResetWiggleTime();
        worm.SetActive(false);
    }

    // Start is called before the first frame update
    //IEnumerator Start()
    //{
    //    ResetWiggleTime();
    //    animator = worm.GetComponent<Animator>();
    //    worm.SetActive(false);


    //    while (true)
    //    {

    //        Debug.Log("FirstWhile");
    //        while (emergeTime > 0)
    //        {
    //            Debug.Log("secondWhile");
    //            emergeTime -= Time.deltaTime;
    //            yield return new WaitForEndOfFrame();
    //        }

    //        ForceWormEmerge();
    //        yield return new WaitForSeconds(wiggleTime + 3f);
    //    }
    //}

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
        collider2D.enabled = false;
        worm.SetActive(false);
        isEaten = true;
        emergeTime = 60.0f;
        
    }

    void ResetWorm()
    {
        collider2D.enabled = true;
        
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
