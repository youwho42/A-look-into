using UnityEngine;

public class BirdAI : MonoBehaviour, IAnimal
{
    
    float timeToStayAtDestination;
    bool justTookOff;
    public float detectionTimeOutAmount = 3;
    float detectionTimeOutTimer;

    public InteractAreasManager interactAreas;
    DrawZasYDisplacement currentLandingSpot;
    GravityItem gravityItem;
    CanReachTileFlight flight;
    CanReachTileWalk walk;
    public Animator animator;
    float glideTimer;
    bool glide;
    public DrawZasYDisplacement currentSpot;
    bool isSleeping;
    
    AnimalSounds sounds;
    public Vector2Int wakeSleepTimes;
    public bool isNocturnal;


    static int landed_hash = Animator.StringToHash("IsLanded");
    static int walking_hash = Animator.StringToHash("IsWalking");
    static int gliding_hash = Animator.StringToHash("IsGliding");
    static int sleeping_hash = Animator.StringToHash("IsSleeping");
    static int idle_hash = Animator.StringToHash("Idle");

    [SerializeField]
    public FlyingState currentState;
    public enum FlyingState
    {
        isAtDestination,
        isFlying,
        isLanding,
        isWalking
    }

    private void Start()
    {
        
        sounds = GetComponent<AnimalSounds>();

        GameEventManager.onTimeHourEvent.AddListener(SetSleepOrWake);

        animator.SetBool(landed_hash, true);
        gravityItem = GetComponent<GravityItem>();
        flight = GetComponent<CanReachTileFlight>();
        walk = GetComponent<CanReachTileWalk>();
        CheckForLandingArea();
        glideTimer = SetRandomRange(3, 10);
        timeToStayAtDestination = SetRandomRange(new Vector2(5.0f, 15.0f));
        detectionTimeOutAmount = SetRandomRange(5, 60);
        flight.SetRandomDestination();

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

                
                animator.SetBool(walking_hash, false);
                flight.isLanding = false;

                animator.SetBool(landed_hash, false);

                if (sounds.mute)
                    sounds.mute = false;
                if (isSleeping)
                {
                    flight.SetDestination(flight.centerOfActiveArea, true);
                    justTookOff = false;
                    detectionTimeOutAmount = SetRandomRange(5, 30);
                    currentLandingSpot = flight.centerOfActiveArea;
                    flight.isLanding = true;
                    currentLandingSpot.isInUse = true;
                    currentState = FlyingState.isLanding;
                }
                    
                flight.Fly();
                
                if (justTookOff)
                {
                    if (currentLandingSpot != null)
                    {
                        currentLandingSpot.isInUse = false;
                        currentLandingSpot = null;
                    }
                    detectionTimeOutTimer += Time.deltaTime;
                    if (detectionTimeOutTimer >= detectionTimeOutAmount)
                    {
                        justTookOff = false;
                        detectionTimeOutTimer = 0;
                    }
                }
                glideTimer -= Time.deltaTime;
                if (glideTimer <= 0)
                {
                    glide = !glide;
                    glideTimer = glide ? SetRandomRange(0.5f, 4) : SetRandomRange(0.5f, 10.0f);
                    animator.SetBool(gliding_hash, glide);
                }


                if (glide)
                {
                    glide = false;
                    glideTimer = SetRandomRange(0.8f, 10);
                    animator.SetBool(gliding_hash, glide);
                }
                if (!justTookOff)
                    CheckForLandingArea();

                break;


            

            case FlyingState.isAtDestination:
               
                if (!isSleeping)
                {
                    animator.SetBool(sleeping_hash, false);
                    glideTimer -= Time.deltaTime;
                    if (glideTimer <= 0)
                    {
                        glideTimer = SetRandomRange(1.5f, 15.0f);
                        animator.SetTrigger(idle_hash);
                    }

                    timeToStayAtDestination -= Time.deltaTime;

                    if (gravityItem.itemObject.localPosition.y == 0)
                    {
                        if (timeToStayAtDestination >= 4)
                        {
                            if (Random.Range(0.0f, 1.0f) <= 0.0075f)
                            {
                                walk.SetRandomDestination();
                                animator.SetBool(walking_hash, true);
                                currentState = FlyingState.isWalking;
                            }

                        }
                    }

                    

                    if (timeToStayAtDestination <= 0)
                    {
                        flight.SetRandomDestination();
                        animator.SetBool(landed_hash, false);
                        justTookOff = true;
                        currentState = FlyingState.isFlying;
                    }
                    if (sounds.mute)
                        sounds.mute = false;
                    
                }
                else
                {
                    if(Vector2.Distance(transform.position, flight.centerOfActiveArea.transform.position) >= 0.01f)
                    {
                        flight.SetRandomDestination();
                        animator.SetBool(landed_hash, false);
                        justTookOff = true;
                        currentState = FlyingState.isFlying;
                        break;
                    }
                    animator.SetBool(sleeping_hash, true);
                    if (!sounds.mute)
                        sounds.mute = true;
                }

                

                break;




            case FlyingState.isLanding:

                flight.Fly();

                if (Vector2.Distance(transform.position, flight.currentDestination) <= 0.001f && Vector2.Distance(gravityItem.itemObject.localPosition, flight.currentDestinationZ) <= 0.001f)
                {

                    
                    timeToStayAtDestination = SetRandomRange(new Vector2(5.0f, 15.0f));
                    flight.isLanding = false;
                    animator.SetBool(landed_hash, true);
                    
                    currentState = FlyingState.isAtDestination;
                    
                }

                break;

            case FlyingState.isWalking:
                walk.Walk();
                if (currentLandingSpot != null)
                {
                    currentLandingSpot.isInUse = false;
                    currentLandingSpot = null;
                }
                if (Vector2.Distance(transform.position, walk.currentDestination) <= 0.01f)
                {
                    animator.SetBool(walking_hash, false);
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
        if (!isNocturnal) 
        {
            if (time >= wakeSleepTimes.y || time < wakeSleepTimes.x)
                isSleeping = true;
            else if (time >= wakeSleepTimes.x && time < wakeSleepTimes.y)
                isSleeping = false;
        } 
        else
        {
            if (time >= wakeSleepTimes.y && time < wakeSleepTimes.x)
                isSleeping = true;
            else if (time >= wakeSleepTimes.x || time < wakeSleepTimes.y)
                isSleeping = false;
        }
        
    }



    void CheckForLandingArea()
    {
        if (interactAreas == null || justTookOff || currentState != FlyingState.isFlying)
            return;
        
        DrawZasYDisplacement bestTarget = null;
        float closestDistance = Mathf.Infinity;
        Vector2 currentPosition = transform.position;
        foreach (var item in interactAreas.allAreas)
        {
            if (item.isInUse || item.transform.position.z != transform.position.z)
                continue;
            var dist = Vector2.Distance(currentPosition, item.transform.position);
           
            if (dist < closestDistance)
            {
                closestDistance = dist;
                bestTarget = item;
            }
        }

        if (bestTarget == null)
            return;

        flight.SetDestination(bestTarget, true);

        justTookOff = false;
        detectionTimeOutAmount = SetRandomRange(5, 30);
        currentLandingSpot = bestTarget;
        flight.isLanding = true;
        currentLandingSpot.isInUse = true;
        currentState = FlyingState.isLanding;


    }

    
    

    
}
