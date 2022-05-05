using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeAI : MonoBehaviour, IAnimal
{
    GravityItem gravityItem;
    public float roamingArea;
    float timeToStayAtDestination;
    bool justTookOff;
    public float detectionTimeOutAmount = 3;
    float detectionTimeOutTimer;
    DrawZasYDisplacement currentFlower;
    CanReachTileFlight flight;
    public Animator animator;
    bool isSleeping;
    AudioSource audio;

    public InteractAreasManager interactAreas;

    static int landed_hash = Animator.StringToHash("IsLanded");


    [SerializeField]
    public FlyingState currentState;


    public enum FlyingState
    {
        isFlying,
        isLanding,
        isAtDestination,

    }

    private void Start()
    {
        gravityItem = GetComponent<GravityItem>();

        audio = GetComponent<AudioSource>();
        float randomIdleStart = Random.Range(0, animator.GetCurrentAnimatorStateInfo(0).length);
        animator.Play(0, 0, randomIdleStart);
        flight = GetComponent<CanReachTileFlight>();
        CheckForLandingArea();
        flight.SetRandomDestination();
        currentState = FlyingState.isFlying;
        GameEventManager.onTimeHourEvent.AddListener(SetSleepOrWake);
        Invoke("SetSleepState", 1f);
    }

    void SetSleepState()
    {
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

                if (audio.mute)
                    audio.mute = false;
                if (isSleeping)
                {
                    flight.SetDestination(flight.centerOfActiveArea, true);
                    justTookOff = false;
                    detectionTimeOutAmount = SetRandomRange(5, 30);
                    currentFlower = flight.centerOfActiveArea;
                    flight.isLanding = true;
                    currentState = FlyingState.isLanding;
                }


                flight.Fly();


                if (justTookOff)
                {

                    if (currentFlower != null)
                    {
                        currentFlower.isInUse = false;
                        currentFlower = null;
                    }

                    detectionTimeOutTimer += Time.deltaTime;
                    if (detectionTimeOutTimer >= detectionTimeOutAmount)
                    {
                        detectionTimeOutTimer = 0;
                        justTookOff = false;
                    }
                }

                if (!justTookOff)
                    CheckForLandingArea();

                break;


            case FlyingState.isLanding:

                flight.Fly();
                if (Vector2.Distance(transform.position, flight.currentDestination) <= 0.001f && Vector2.Distance(gravityItem.itemObject.localPosition, flight.currentDestinationZ) <= 0.001f)
                {
                    flight.isLanding = false;
                    timeToStayAtDestination = SetTimeToStayAtDestination();
                    animator.SetBool(landed_hash, true);
                    currentState = FlyingState.isAtDestination;
                }

                break;


            case FlyingState.isAtDestination:

                if (!audio.mute)
                    audio.mute = true;

                if (!isSleeping)
                {
                    timeToStayAtDestination -= Time.deltaTime;
                    if (timeToStayAtDestination <= 0)
                    {
                        flight.SetRandomDestination();
                        animator.SetBool(landed_hash, false);
                        justTookOff = true;
                        currentState = FlyingState.isFlying;
                    }

                }
                else
                {
                    if (Vector2.Distance(transform.position, flight.centerOfActiveArea.transform.position) >= 0.01f)
                    {
                        flight.SetRandomDestination();
                        animator.SetBool(landed_hash, false);
                        justTookOff = true;
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
        if (time >= 20 || time < 7)
        {
            isSleeping = true;
        }
        else if (time >= 7 && time < 20)
        {
            isSleeping = false;
        }
    }

    float SetRandomRange(float min, float max)
    {
        return Random.Range(min, max);
    }


    void CheckForLandingArea()
    {
        if (interactAreas == null || justTookOff || currentState != FlyingState.isFlying)
            return;

        DrawZasYDisplacement bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (var item in interactAreas.allAreas)
        {
            if (item.isInUse)
                continue;
            Vector3 directionToTarget = item.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = item;
            }
        }


        
        flight.SetDestination(bestTarget, true);
        justTookOff = false;
        detectionTimeOutAmount = SetRandomRange(5, 30);
        currentFlower = bestTarget;
        flight.isLanding = true;
        currentFlower.isInUse = true;
        currentState = FlyingState.isLanding;

    }











}
