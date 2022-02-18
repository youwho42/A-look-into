using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeAI : MonoBehaviour
{
    public float roamingArea;
    float timeToStayAtDestination;
    bool justTookOff;
    public float detectionTimeOutAmount = 3;
    float detectionTimeOutTimer;
    public CircleCollider2D captureCollider;
    Collider2D currentFlower;
    CharacterFlight flight;
    public Animator animator;
    bool isSleeping;
    bool isRaining;
    public Transform home;
    public GameObject homeObject;
    DrawZasYDisplacement displacmentZ;
    RandomBeeSounds beeSounds;
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
        beeSounds = GetComponent<RandomBeeSounds>();
        float randomIdleStart = Random.Range(0, animator.GetCurrentAnimatorStateInfo(0).length);
        animator.Play(0, 0, randomIdleStart);
        flight = GetComponent<CharacterFlight>();
        
        //displacmentZ = home.GetComponent<DrawZasYDisplacement>();
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

                if (!beeSounds.isMakingSound)
                    beeSounds.PlaySound();

                flight.Move();
                if (justTookOff)
                {

                    if (currentFlower != null)
                    {
                        currentFlower.tag = "OpenFlower";
                    }

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
                captureCollider.offset = flight.characterSprite.localPosition;
                break;


            case FlyingState.isLanding:

                if (!beeSounds.isMakingSound)
                    beeSounds.PlaySound();

                flight.Move();
                if (Vector2.Distance(transform.position, flight.currentDestination) <= 0.001f && Vector2.Distance(flight.characterSprite.localPosition, flight.destinationZ) <= 0.001f)
                {
                    timeToStayAtDestination = SetTimeToStayAtDestination();
                    animator.SetBool("IsLanded", true);
                    currentState = FlyingState.isAtDestination;
                }
                captureCollider.offset = flight.characterSprite.localPosition;
                break;


            case FlyingState.isAtDestination:

                if (!isSleeping || !isRaining)
                {
                    timeToStayAtDestination -= Time.deltaTime;
                    if (timeToStayAtDestination <= 0)
                    {
                        /*if(currentFlower != null)
                            currentFlower.GetComponent<EntityReproduction>().AllowForReproduction();*/
                        flight.SetRandomDestination(roamingArea);
                        currentState = FlyingState.isFlying;
                        animator.SetBool("IsLanded", false);
                    }
                }
                if(beeSounds.isMakingSound)
                    beeSounds.StopSound();

                break;
        }
    }



    float SetTimeToStayAtDestination()
    {
        return Random.Range(0.5f, 10.0f);
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

    public void SetHome(Transform location)
    {
        flight = GetComponent<CharacterFlight>();
        home = location;
        flight.centerOfActiveArea = home;
        displacmentZ = home.GetComponent<DrawZasYDisplacement>();
        
    }


    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<DrawZasYDisplacement>() != null && !justTookOff && collision.CompareTag("OpenFlower") && currentState == FlyingState.isFlying)
        {
            var temp = collision.GetComponent<DrawZasYDisplacement>();
            flight.SetDestination(temp.transform.position, temp.displacedPosition);
            currentState = FlyingState.isLanding;
            justTookOff = true;
            detectionTimeOutTimer = 0;
            currentFlower = collision;
            currentFlower.tag = "ClosedFlower";
        }
        if (collision.CompareTag("Player"))
        {
            flight.SetRandomDestination(roamingArea * 2);
            currentState = FlyingState.isFlying;
            justTookOff = true;
            detectionTimeOutTimer = 0;
            animator.SetBool("IsLanded", false);
            if (currentFlower != null)
            {
                currentFlower.tag = "ClosedFlower";
            }
        }
        if (collision.CompareTag("RainStorm") && !isRaining)
        {
            isRaining = true;
            flight.SetDestination(home.position, displacmentZ.displacedPosition);
            currentState = FlyingState.isLanding;
        }
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

                animator.SetBool("IsLanded", false);
                if (currentFlower != null)
                {
                    currentFlower.tag = "ClosedFlower";
                }
            }
            isRaining = false;
        }
    }
}
