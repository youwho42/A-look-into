using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButterflyAI : MonoBehaviour, IAnimal
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
    EntityReproduction reproduction;
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
        reproduction = GetComponent<EntityReproduction>();
        float randomIdleStart = Random.Range(0, animator.GetCurrentAnimatorStateInfo(0).length);
        animator.Play(0, 0, randomIdleStart);
        flight = GetComponent<CharacterFlight>();
        currentState = FlyingState.isFlying;
        
    }

    private void Update()
    {

        switch (currentState)
        {
            case FlyingState.isFlying:

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
                    flight.SetRandomDestination(roamingArea);
                    
                }
                captureCollider.offset = flight.characterSprite.localPosition;
                break;


            case FlyingState.isLanding:

                flight.Move();
                if (Vector2.Distance(transform.position, flight.currentDestination) <= 0.001f)
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
                        if (currentFlower != null)
                            currentFlower.GetComponentInParent<EntityReproduction>().AllowForReproduction();
                        flight.SetRandomDestination(roamingArea);
                        animator.SetBool("IsLanded", false);
                        reproduction.AllowForReproduction();
                        currentState = FlyingState.isFlying;
                        
                    }
                }
                

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
            isSleeping = true;
        } 
        else if(time == 7)
        {
            isSleeping = false;
        }
    }

    public void SetHome(Transform location)
    {
        /*Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, 5);
        Collider2D nearest = null;
        float distance = 0;

        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].CompareTag("OpenCrowSpot"))
            {
                float tempDistance = Vector3.Distance(transform.position, hit[i].transform.position);
                if (nearest == null || tempDistance < distance)
                {
                    nearest = hit[i];
                    distance = tempDistance;
                }
            }

        }
        if (nearest != null)
            home = nearest.transform;
*/
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<DrawZasYDisplacement>() != null && !justTookOff && collision.CompareTag("OpenFlower") && currentState == FlyingState.isFlying)
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
        if (collision.CompareTag("RainStorm") && !isRaining && !isSleeping)
        {
            isRaining = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("RainStorm") && isRaining)
        {
            isRaining = false;
        }
    }



}
