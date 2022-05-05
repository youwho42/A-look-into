using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenAI : MonoBehaviour, IAnimal
{

    float timeToStayAtDestination;

    GravityItem gravityItem;
    CanReachTileWalk walk;
    public Animator animator;
    
    public Transform home;
    
    static int walking_hash = Animator.StringToHash("IsWalking");
    static int peck_hash = Animator.StringToHash("Peck");

    public float roamingArea;
    
    public float detectionTimeOutAmount = 3;

    bool isPecking;
    
    float peckTimer;

    [SerializeField]
    public CurrentState currentState;

    public enum CurrentState
    {
        isWalking,
        isPecking,
        isAtDestination
    }

    private void Start()
    {

        float randomIdleStart = Random.Range(0, animator.GetCurrentAnimatorStateInfo(0).length);
        animator.Play(0, 0, randomIdleStart);
        walk = GetComponent<CanReachTileWalk>();

    }

   

    private void Update()
    {

        switch (currentState)
        {
            case CurrentState.isWalking:
                walk.Walk();
                isPecking = false;
                animator.SetBool(walking_hash, true);
                if (Vector2.Distance(transform.position, walk.currentDestination) <= 0.01f)
                {
                    animator.SetBool(walking_hash, false);
                    timeToStayAtDestination = SetRandomRange(2.0f, 12.0f);
                    peckTimer = SetRandomRange(0.2f, 6.0f);
                    currentState = CurrentState.isAtDestination;
                }
                
                break;


            case CurrentState.isPecking:

                animator.SetTrigger(peck_hash);
                animator.SetBool(walking_hash, false);
                if (!AnimatorIsPlaying("NewChickenPeck"))
                {
                    //timeToStayAtDestination = SetRandomRange(2.0f, 6.0f);

                    currentState = CurrentState.isAtDestination;
                }
                
                break;


            case CurrentState.isAtDestination:
                peckTimer -= Time.deltaTime;
                
                if (peckTimer <= 0)
                {
                    peckTimer = SetRandomRange(0.2f, 5.0f);
                    currentState = CurrentState.isPecking;
                }
                

                timeToStayAtDestination -= Time.deltaTime;
                if (timeToStayAtDestination <= 0 && !AnimatorIsPlaying("NewChickenPeck"))
                {
                    walk.SetRandomDestination();
                    animator.SetBool(walking_hash, true);
                    currentState = CurrentState.isWalking;
                }
                

                break;
        }
    }

    float SetRandomRange(float min, float max)
    {
        return Random.Range(min, max);
    }

    

    bool AnimatorIsPlaying(string stateName)
    {
        if(animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
        {
            if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1)
                return true;
        }
        return false;
    }

    


    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Worm") && !isPecking)
        {
            isPecking = true;
            currentState = CurrentState.isPecking;
        }
        
    }
}
