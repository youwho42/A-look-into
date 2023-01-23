using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klaxon.GravitySystem;

public class ButterflyAI : MonoBehaviour, IAnimal
{
    GravityItemNew gravityItem;
    public float roamingArea;
    float timeToStayAtDestination;
    bool justTookOff;
    public float detectionTimeOutAmount = 3;
    float detectionTimeOutTimer;
    DrawZasYDisplacement currentFlower;
    CanReachTileFlight flight;
    public Animator animator;
    bool isSleeping;
    bool isRaining;
    public bool isNocturnal;
    public InteractAreasManager interactAreas;

    static int landed_hash = Animator.StringToHash("IsLanded");

    bool activeState = true;
    SpriteRenderer animalSprite;
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
        gravityItem = GetComponent<GravityItemNew>();
        

        
        
        float randomIdleStart = Random.Range(0, animator.GetCurrentAnimatorStateInfo(0).length);
        animator.Play(0, 0, randomIdleStart);
        flight = GetComponent<CanReachTileFlight>();
        CheckForLandingArea();
        flight.SetRandomDestination();
        currentState = FlyingState.isFlying;
        GameEventManager.onTimeHourEvent.AddListener(SetSleepOrWake);
        animalSprite = gravityItem.itemObject.GetComponent<SpriteRenderer>();
        Invoke("InitializeTime", 2f);
    }
    void InitializeTime()
    {
        SetSleepOrWake(RealTimeDayNightCycle.instance.hours);
    }

    private void OnDestroy()
    {
        GameEventManager.onTimeHourEvent.RemoveListener(SetSleepOrWake);
    }

    bool CheckVisibility()
    {
        return animalSprite.isVisible;
    }

    private void Update()
    {
        
        if (!activeState)
            return;

        if (flight.centerOfActiveArea == null)
            CheckForLandingArea();


        switch (currentState)
        {
            case FlyingState.isFlying:

                flight.Fly();
                flight.isLanding = false;

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
                if (!justTookOff && CheckVisibility())
                    CheckForLandingArea();

                if (flight.isOverWater)
                    Deviate();
                break;


            case FlyingState.isLanding:

                flight.Fly();

                if ((Vector2)currentFlower.transform.position != flight.currentDestination || flight.isOverWater || !flight.canReachNextTile)
                {
                    Deviate();
                    break;
                }

                

                if (Vector2.Distance(transform.position, flight.currentDestination) <= 0.001f && Vector2.Distance(gravityItem.itemObject.localPosition, flight.currentDestinationZ) <= 0.001f)
                {
                    flight.isLanding = false;
                    timeToStayAtDestination = SetTimeToStayAtDestination();
                    animator.SetBool(landed_hash, true);
                    currentState = FlyingState.isAtDestination;
                }
                
                break;


            case FlyingState.isAtDestination:

                if (!isSleeping)
                {
                    timeToStayAtDestination -= Time.deltaTime;
                    if (timeToStayAtDestination <= 0)
                    {
                        SetToFlyingState();
                    }
                       
                }
                

                
                break;

            
        }
    }

    void Deviate()
    {
        isSleeping = false;
        SetToFlyingState();
        Invoke("ResetSleep", 2f);
    }
    private void ResetSleep()
    {
        SetSleepOrWake(RealTimeDayNightCycle.instance.hours);
    }

    void SetToFlyingState()
    {
        flight.SetRandomDestination();
        animator.SetBool(landed_hash, false);
        justTookOff = true;
        detectionTimeOutAmount = SetRandomRange(5, 30);
        currentState = FlyingState.isFlying;
    }

    float SetTimeToStayAtDestination()
    {
        return Random.Range(0.5f, 10.0f);
    }

    public void SetSleepOrWake(int time)
    {
        if (!isNocturnal)
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
        else
        {
            if (time >= 20 || time < 7)
            {
                isSleeping = false;
            }
            else if (time >= 7 && time < 20)
            {
                isSleeping = true;
            }
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
        float closestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (var item in interactAreas.allAreas)
        {
            if (item.isInUse || item.transform.position.z != transform.position.z)
                continue;
            var dist = Vector2.Distance(currentPosition, item.transform.position);
            if (dist < closestDistance)
            {
                if (dist < 0.3f)
                    continue;
                closestDistance = dist;
                bestTarget = item;
            }
        }

        if (bestTarget == null)
            return;

        flight.centerOfActiveArea = bestTarget;
        flight.SetDestination(bestTarget, true);
        justTookOff = false;
        detectionTimeOutAmount = SetRandomRange(5, 30);
        currentFlower = bestTarget;
        flight.isLanding = true;
        currentFlower.isInUse = true;
        currentState = FlyingState.isLanding;

    }

    public void SetActiveState(bool active)
    {
        activeState = active;
    }

    public void FleePlayer()
    {
        SetToFlyingState();
    }
}
