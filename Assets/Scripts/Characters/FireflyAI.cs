using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class FireflyAI : MonoBehaviour
{
    public float roamingArea;
    
    public CircleCollider2D captureCollider;
    public SpriteRenderer lightMaterial;



    CharacterFlight flight;
    public Animator animator;
    bool isSleeping;
    public Transform home;
    DrawZasYDisplacement displacmentZ;

    
    

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

        DayNightCycle.instance.FullHourEventCallBack.AddListener(SetSleepOrWake);
        

        float randomIdleStart = Random.Range(0, animator.GetCurrentAnimatorStateInfo(0).length);
        animator.Play(0, 0, randomIdleStart);
        flight = GetComponent<CharacterFlight>();

        displacmentZ = home.GetComponent<DrawZasYDisplacement>();
       if(DayNightCycle.instance.hours < 20 || DayNightCycle.instance.hours > 22)
        {
            flight.SetDestination(home.position, displacmentZ.displacedPosition);
            currentState = FlyingState.isLanding;
            
            isSleeping = true;
        }
        else
        {
            isSleeping = false;
            flight.SetRandomDestination(roamingArea);
            currentState = FlyingState.isFlying;
        }
    }

    private void Update()
    {

        switch (currentState)
        {
            case FlyingState.isFlying:

                

                flight.Move();
                
                if (Vector2.Distance(transform.position, flight.currentDestination) <= 0.01f)
                {
                    if (!isSleeping)
                    {
                        flight.SetRandomDestination(roamingArea);
                    }
                    else
                    {
                        flight.SetDestination(home.position, displacmentZ.displacedPosition);
                        currentState = FlyingState.isLanding;
                    }

                }
                captureCollider.offset = flight.characterSprite.localPosition;
                break;


            case FlyingState.isLanding:

                

                flight.Move();
                if (Vector2.Distance(transform.position, flight.currentDestination) <= 0.001f)
                {
                    
                    
                    currentState = FlyingState.isAtDestination;
                }
                captureCollider.offset = flight.characterSprite.localPosition;
                break;


            case FlyingState.isAtDestination:

                lightMaterial.enabled = false;
                
                break;
        }
    }

    



    public void SetSleepOrWake(int time)
    {
        if (time == 22)
        {

            flight.SetDestination(home.position, displacmentZ.displacedPosition);
            currentState = FlyingState.isLanding;

            isSleeping = true;

        }
        else if (time == 20)
        {
            isSleeping = false;
            flight.SetRandomDestination(roamingArea);
            currentState = FlyingState.isFlying;
            lightMaterial.enabled = true;
        }
    }

}
