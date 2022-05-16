using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishAI : MonoBehaviour, IAnimal
{
    GravityItem gravityItem;
    float timeToStayAtDestination;
    CanReachTileSwim swim;
    Animator animator;
    

    [SerializeField]
    public SwimmingState currentState;


    public enum SwimmingState
    {
        isSwimming,
        isAtDestination
    }

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        gravityItem = GetComponent<GravityItem>();
        swim = GetComponent<CanReachTileSwim>();

        float randomIdleStart = Random.Range(0, animator.GetCurrentAnimatorStateInfo(0).length);
        animator.Play(0, 0, randomIdleStart);

        timeToStayAtDestination = SetTimeToStayAtDestination();
        swim.SetRandomDestination();
        swim.SetRandomDestinationZ();
    }

   

    private void Update()
    {
        

        switch (currentState)
        {
            case SwimmingState.isSwimming:
                
                swim.Swim();
                if (Vector2.Distance(transform.position, swim.currentDestination) <= 0.01f && Vector2.Distance(gravityItem.itemObject.localPosition, swim.currentDestinationZ) <= 0.01f)
                {
                    
                    timeToStayAtDestination = SetTimeToStayAtDestination();
                    
                    currentState = SwimmingState.isAtDestination;
                }


                break;


            


            case SwimmingState.isAtDestination:

                
                timeToStayAtDestination -= Time.deltaTime;
                if (timeToStayAtDestination <= 0)
                {
                    
                    swim.SetRandomDestination();
                    
                    currentState = SwimmingState.isSwimming;

                }
                


                break;


        }
    }

    void SetBoidsState(bool isInBoids)
    {
        if (swim.boid != null)
        {
            swim.boid.inBoidPool = isInBoids;
        }

    }


    float SetTimeToStayAtDestination()
    {
        return Random.Range(0.5f, 10.0f);
    }




    



}
