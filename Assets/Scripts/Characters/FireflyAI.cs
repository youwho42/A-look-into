using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UIElements;
using Klaxon.GravitySystem;
public class FireflyAI : MonoBehaviour, IAnimal
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

    MusicGeneratorItem musicItem;

    public Vector2Int wakeSleepTimes;

    RealTimeDayNightCycle realTimeDayNightCycle;

    bool activeState = true;

    [SerializeField]
    public FlyingState currentState;
    SpriteRenderer animalSprite;

    public enum FlyingState
    {
        isFlying,
        isLanding,
        isAtDestination,

    }

    private void Start()
    {
        gravityItem = GetComponent<GravityItemNew>();
        realTimeDayNightCycle = RealTimeDayNightCycle.instance;
        musicItem = GetComponentInChildren<MusicGeneratorItem>();
        float randomIdleStart = Random.Range(0, animator.GetCurrentAnimatorStateInfo(0).length);
        animator.Play(0, 0, randomIdleStart);
        flight = GetComponent<CanReachTileFlight>();
        
        flight.SetRandomDestination();
        currentState = FlyingState.isFlying;
        GameEventManager.onTimeTickEvent.AddListener(SetSleepOrWake);
        animalSprite = gravityItem.itemObject.GetComponent<SpriteRenderer>();
        Invoke("SetSleepState", 1f);
    }

    void SetSleepState()
    {
        SetSleepOrWake(RealTimeDayNightCycle.instance.hours);
    }
    private void OnDestroy()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(SetSleepOrWake);
    }

    bool CheckVisibility()
    {
        return animalSprite.isVisible;
    }

    private void Update()
    {
        if (!CheckVisibility())
        {
            if (Time.frameCount % 20 != 0)
                return;
        }
        if (!activeState)
            return;

        switch (currentState)
        {
            case FlyingState.isFlying:

                
                if (isSleeping)
                {
                    flight.SetDestination(flight.centerOfActiveArea, true);
                    justTookOff = false;
                    detectionTimeOutAmount = SetRandomRange(5, 30);
                    currentFlower = flight.centerOfActiveArea;
                    flight.isLanding = true;
                    currentState = FlyingState.isLanding;
                    break;
                }

                flight.isLanding = false;
                flight.Fly();


                break;


            case FlyingState.isLanding:

                flight.Fly();
                flight.isLanding = true;

                if (flight.isOverWater || !flight.canReachNextTile)
                {
                    Deviate();
                    break;
                }
                if (Vector2.Distance(transform.position, flight.currentDestination) <= 0.001f && Vector2.Distance(gravityItem.itemObject.localPosition, flight.currentDestinationZ) <= 0.001f)
                {
                    flight.isLanding = false;
                    timeToStayAtDestination = SetTimeToStayAtDestination();
                    
                    currentState = FlyingState.isAtDestination;
                }

                break;


            case FlyingState.isAtDestination:

                

                if (!isSleeping)
                {
                    timeToStayAtDestination -= Time.deltaTime;
                    if (timeToStayAtDestination <= 0)
                    {
                        flight.SetRandomDestination();
                        
                        justTookOff = true;
                        currentState = FlyingState.isFlying;
                        break;
                    }

                }
                else
                {
                    if (Vector2.Distance(transform.position, flight.centerOfActiveArea.transform.position) >= 0.01f)
                    {
                        flight.SetRandomDestination();
                        
                        justTookOff = true;
                        currentState = FlyingState.isFlying;
                        break;
                    }
                }

                break;


        }
    }

    void Deviate()
    {
        isSleeping = false;
        
        Invoke("ResetSleep", 2f);
    }
    private void ResetSleep()
    {
        SetSleepOrWake(RealTimeDayNightCycle.instance.hours);
    }

    float SetTimeToStayAtDestination()
    {
        return Random.Range(0.5f, 10.0f);
    }

    public void SetSleepOrWake(int time)
    {
        if (realTimeDayNightCycle.currentTimeRaw >= wakeSleepTimes.x && realTimeDayNightCycle.currentTimeRaw < wakeSleepTimes.y)
            isSleeping = false;
        else if (realTimeDayNightCycle.currentTimeRaw >= wakeSleepTimes.y || realTimeDayNightCycle.currentTimeRaw < wakeSleepTimes.x)
            isSleeping = true;
        musicItem.isActive = !isSleeping;
        gravityItem.itemObject.gameObject.SetActive(!isSleeping);
    }

    float SetRandomRange(float min, float max)
    {
        return Random.Range(min, max);
    }

    public void SetActiveState(bool active)
    {
        activeState = active;
    }

    public void FleePlayer(Transform playerTransform)
    {
        
    }
}
