using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButterflyAI : MonoBehaviour, IAnimal
{
    GravityItem gravityItem;
    public float roamingArea;
    float timeToStayAtDestination;
    bool justTookOff;
    public float detectionTimeOutAmount = 3;
    float detectionTimeOutTimer;
    public CircleCollider2D captureCollider;
    DrawZasYDisplacement currentFlower;
    CanReachTileFlight flight;
    public Animator animator;
    bool isSleeping;
    bool isRaining;
    public Transform home;
    //EntityReproduction reproduction;

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
        gravityItem = GetComponent<GravityItem>();
        
        //reproduction = GetComponent<EntityReproduction>();
        float randomIdleStart = Random.Range(0, animator.GetCurrentAnimatorStateInfo(0).length);
        animator.Play(0, 0, randomIdleStart);
        flight = GetComponent<CanReachTileFlight>();
        SetHome(transform);
        flight.SetRandomDestination();
        currentState = FlyingState.isFlying;
        //GameEventManager.onTimeHourEvent.AddListener(SetSleepOrWake);
    }
    
    private void OnDestroy()
    {
        //GameEventManager.onTimeHourEvent.RemoveListener(SetSleepOrWake);
    }

    private void Update()
    {
        if (flight.centerOfActiveArea == null)
            SetHome(transform);

        captureCollider.offset = gravityItem.itemObject.localPosition;

        switch (currentState)
        {
            case FlyingState.isFlying:

                flight.Fly();
                CheckForLandingArea();

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

                if (!isSleeping || !isRaining)
                {
                    timeToStayAtDestination -= Time.deltaTime;
                    if (timeToStayAtDestination <= 0)
                    {
                        /*if (currentFlower != null)
                            currentFlower.GetComponentInParent<EntityReproduction>().AllowForReproduction();*/
                        flight.SetRandomDestination();
                        animator.SetBool(landed_hash, false);
                        //reproduction.AllowForReproduction();
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
        if (time == 20)
        {
            isSleeping = true;
        } 
        else if(time == 7)
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
        if (justTookOff || currentState != FlyingState.isFlying)
            return;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, .3f);
        if(hits.Length > 0)
        {
            
            foreach (var hit in hits)
            {
                if (hit.GetComponent<DrawZasYDisplacement>() == null)
                    continue;
                
                if (hit.CompareTag("OpenFlower"))
                {
                    var temp = hit.GetComponent<DrawZasYDisplacement>();
                    if (temp.isInUse)
                        continue;
                    flight.centerOfActiveArea = temp.gameObject.transform;
                    flight.SetDestination(temp);
                    justTookOff = false;
                    detectionTimeOutAmount = SetRandomRange(5, 30);
                    currentFlower = temp;
                    flight.isLanding = true;
                    currentFlower.isInUse = true;
                    currentState = FlyingState.isLanding;
                    return;
                }
                if (hit.CompareTag("Player"))
                {
                    flight.SetRandomDestination();
                    
                    justTookOff = true;
                    detectionTimeOutAmount = SetRandomRange(5, 30);
                    animator.SetBool(landed_hash, false);
                    if (currentFlower != null)
                    {
                        currentFlower.isInUse = false;
                    }
                    currentState = FlyingState.isFlying;
                    return;
                }
            }
        }
    }

   
    public void SetHome(Transform location)
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, 5);
        Collider2D nearest = null;
        float distance = 0;

        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].CompareTag("OpenFlower"))
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
        {
            home = nearest.transform;

        }
        flight.centerOfActiveArea = home;

    }

   



}
