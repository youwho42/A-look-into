using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class FireflyAI : MonoBehaviour, IAnimal
{
    GravityItem gravityItem;
    public float roamingArea;
    
    public SpriteRenderer lightMaterial;



    CanReachTileFlight flight;
    public Animator animator;
    bool isSleeping;
    
    public DrawZasYDisplacement home;

    
    bool isRaining;

    static int landed_hash = Animator.StringToHash("IsLanded");


    [SerializeField]
    public FlyingState currentState;

    public enum FlyingState
    {
        isFlying,
        isLanding,
        isAtDestination
    }

    private void Start()
    {

        GameEventManager.onTimeHourEvent.AddListener(SetSleepOrWake);


        float randomIdleStart = Random.Range(0, animator.GetCurrentAnimatorStateInfo(0).length);
        animator.Play(0, 0, randomIdleStart);
        flight = GetComponent<CanReachTileFlight>();
        flight.centerOfActiveArea = home;
        SetSleepOrWake(RealTimeDayNightCycle.instance.hours);
       
    }
    private void OnDestroy()
    {
        GameEventManager.onTimeHourEvent.RemoveListener(SetSleepOrWake);

    }
    private void Update()
    {
        

        switch (currentState)
        {
            case FlyingState.isFlying:

                

                flight.Fly();
                
                
                
                
                
                break;


            case FlyingState.isLanding:

                

                flight.Fly();
                if (Vector2.Distance(transform.position, flight.currentDestination) <= 0.001f)
                {
                    
                    
                    currentState = FlyingState.isAtDestination;
                }
                
                break;


            case FlyingState.isAtDestination:

                lightMaterial.enabled = false;
                
                break;
        }
    }

    



    public void SetSleepOrWake(int time)
    {
       
        if (time >= 4 && time < 20)
        {
            
            flight.SetDestination(home, true);
            isSleeping = true;
            if(currentState != FlyingState.isAtDestination)
                currentState = FlyingState.isLanding;

        }
        if (time == 20)
        {
            
            isSleeping = false;
            flight.SetRandomDestination();
            lightMaterial.enabled = true;
            currentState = FlyingState.isFlying;
            
        }
    }

}
