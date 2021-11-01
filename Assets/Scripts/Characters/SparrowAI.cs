using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparrowAI : MonoBehaviour
{
    public float roamingArea;
    float timeToStayAtDestination;
    bool justTookOff;
    public float detectionTimeOutAmount = 3;
    float detectionTimeOutTimer;

    Collider2D currentLandingSpot;
    CharacterFlight flight;
    CharacterWalk walk;
    public Animator animator;
    float chirpTimer;

    bool isSleeping;
    public Transform home;
    DrawZasYDisplacement displacmentZ;
    AnimalSounds sounds;
    bool isRaining;
    bool isOnGround;

    [SerializeField]
    public FlyingState currentState;
    public enum FlyingState
    {
        isAtDestination,
        isFlying,
        isLanding,
        isHopping
    }

    private void Start()
    {
        sounds = GetComponent<AnimalSounds>();
        DayNightCycle.instance.FullHourEventCallBack.AddListener(SetSleepOrWake);
        animator.SetBool("IsLanded", true);
        flight = GetComponent<CharacterFlight>();
        walk = GetComponent<CharacterWalk>();
        timeToStayAtDestination = SetRandomRange(new Vector2(5.0f, 15.0f));
        detectionTimeOutAmount = SetRandomRange(2, 15);

        displacmentZ = home.GetComponent<DrawZasYDisplacement>();


    }

    private void Update()
    {

        switch (currentState)
        {
            case FlyingState.isFlying:

                if (animator.GetBool("IsHopping"))
                    animator.SetBool("IsHopping", false);

                if (animator.GetBool("IsLanded"))
                    animator.SetBool("IsLanded", false);

                if (sounds.mute)
                    sounds.mute = false;

                flight.Move();

                if (justTookOff)
                {
                    detectionTimeOutTimer += Time.deltaTime;
                    if (detectionTimeOutTimer >= detectionTimeOutAmount)
                    {
                        justTookOff = false;

                    }
                }
                


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

                break;




            case FlyingState.isAtDestination:
                if (!isSleeping || !isRaining)
                {
                   

                    chirpTimer -= Time.deltaTime;
                    if (chirpTimer <= 0)
                    {
                        chirpTimer = SetRandomRange(0.1f, 3.5f);
                        animator.SetTrigger("Idle");
                    }

                    timeToStayAtDestination -= Time.deltaTime;
                    if (flight.characterSprite.transform.localPosition.y == 0)
                    {
                        if (timeToStayAtDestination >= 3)
                        {
                            if (Random.Range(0.0f, 1.0f) <= 0.01f)
                            {
                                walk.SetRandomDestination();
                                animator.SetBool("IsHopping", true);
                                currentState = FlyingState.isHopping;
                            }

                        }
                    }

                    if (timeToStayAtDestination <= 3)
                        flight.SetFacingDirection();

                    if (timeToStayAtDestination <= 0)
                    {
                        
                        animator.SetBool("IsLanded", false);
                        justTookOff = true;
                        flight.SetRandomDestination(roamingArea);
                        currentState = FlyingState.isFlying;
                    }
                    if (sounds.mute)
                        sounds.mute = false;

                }
                else
                {
                    if (!sounds.mute)
                        sounds.mute = true;
                }

                break;




            case FlyingState.isLanding:
                
                flight.Move();
                if (Vector2.Distance(transform.position, flight.currentDestination) <= 0.01f)
                {
                    animator.SetBool("IsLanded", true);
                    currentState = FlyingState.isAtDestination;
                }

                break;

            case FlyingState.isHopping:
                
                walk.Move();
                
                if (Vector2.Distance(transform.position, walk.currentDestination) <= 0.01f)
                {
                    animator.SetBool("IsHopping", false);
                    currentState = FlyingState.isAtDestination;

                }

                    break;
        }
    }


    float SetRandomRange(Vector2 minMaxRange)
    {
        return Random.Range(minMaxRange.x, minMaxRange.y);
    }

    float SetRandomRange(float min, float max)
    {
        return Random.Range(min, max);
    }

    public void SetSleepOrWake(int time)
    {
        if (time == 20)
        {

            flight.SetDestination(home.position, displacmentZ.displacedPosition);
            currentState = FlyingState.isLanding;

            isSleeping = true;

        }
        else if (time == 7)
        {
            isSleeping = false;
        }
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<DrawZasYDisplacement>() != null && !justTookOff && collision.CompareTag("OpenSparrowSpot") && currentState == FlyingState.isFlying)
        {
            var temp = collision.GetComponent<DrawZasYDisplacement>();
            flight.SetDestination(temp.transform.position, temp.displacedPosition);
            timeToStayAtDestination = SetRandomRange(5, 30);
            justTookOff = true;
            detectionTimeOutTimer = 0;
            detectionTimeOutAmount = SetRandomRange(2, 15);
            currentLandingSpot = collision;
            currentLandingSpot.tag = "ClosedSparrowSpot";
            currentState = FlyingState.isLanding;

        }
       if (collision.CompareTag("Player"))
        {
            flight.SetRandomDestination(roamingArea);
            currentState = FlyingState.isFlying;
            justTookOff = true;
            detectionTimeOutTimer = 0;
            detectionTimeOutAmount = SetRandomRange(5, 15);
            animator.SetBool("IsLanded", false);
            sounds.SetCrySounds();
            if (currentLandingSpot != null)
            {
                currentLandingSpot.tag = "OpenSparrowSpot";
            }

        }
        /*if (collision.CompareTag("RainStorm") && !isRaining && !isSleeping)
       {
           isRaining = true;
           flight.SetDestination(home.position, displacmentZ.displacedPosition);
           currentState = FlyingState.isLanding;
       }*/
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("RainStorm"))
        {
            if (isRaining && !isSleeping)
            {
                flight.SetRandomDestination(roamingArea);
                currentState = FlyingState.isFlying;
                justTookOff = true;
                detectionTimeOutTimer = 0;
                detectionTimeOutAmount = SetRandomRange(2, 15);
                animator.SetBool("IsFlying", false);

                if (currentLandingSpot != null)
                {
                    currentLandingSpot.tag = "OpenCrowSpot";
                }
            }
            isRaining = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (home != null) 
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(home.position, roamingArea);
        }
        
    }
}
