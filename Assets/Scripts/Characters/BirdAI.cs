using UnityEngine;
using Klaxon.GravitySystem;

public class BirdAI : MonoBehaviour, IAnimal
{
    
    float timeToStayAtDestination;
    bool justTookOff;
    public float detectionTimeOutAmount = 3;
    float detectionTimeOutTimer;

    public InteractAreasManager interactAreas;
    DrawZasYDisplacement currentLandingSpot;
    GravityItemNew gravityItem;
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
    RealTimeDayNightCycle realTimeDayNightCycle;

    static int landed_hash = Animator.StringToHash("IsLanded");
    static int walking_hash = Animator.StringToHash("IsWalking");
    static int gliding_hash = Animator.StringToHash("IsGliding");
    static int sleeping_hash = Animator.StringToHash("IsSleeping");
    static int idle_hash = Animator.StringToHash("Idle");

    bool activeState = true;

    SpriteRenderer animalSprite;

    MusicGeneratorItem musicItem;

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
        realTimeDayNightCycle = RealTimeDayNightCycle.instance;
        sounds = GetComponent<AnimalSounds>();
        musicItem = GetComponentInChildren<MusicGeneratorItem>();
        GameEventManager.onTimeTickEvent.AddListener(SetSleepOrWake);

        animator.SetBool(landed_hash, true);
        gravityItem = GetComponent<GravityItemNew>();
        flight = GetComponent<CanReachTileFlight>();
        walk = GetComponent<CanReachTileWalk>();
        CheckForLandingArea();
        glideTimer = SetRandomRange(3, 10);
        timeToStayAtDestination = SetRandomRange(new Vector2(2.0f, 4.0f));
        detectionTimeOutAmount = SetRandomRange(5, 60);
        flight.SetRandomDestination();
        animalSprite = gravityItem.itemObject.GetComponent<SpriteRenderer>();
        Invoke("SetSleepState", 1f);
    }

    void OnEnable()
    {
        SetToFlyingState();
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
        
        if (!activeState)
            return;

        switch (currentState)
        {
            case FlyingState.isFlying:
                SetBoidsState(true);

                gravityItem.isWeightless = true;

                animator.SetBool(walking_hash, false);
                

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
                    break;
                }

                flight.isLanding = false;
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
                if (!justTookOff && CheckVisibility())
                    CheckForLandingArea();

                break;


            

            case FlyingState.isAtDestination:
                SetBoidsState(false);
                animator.SetBool(landed_hash, true);
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
                                break;
                            }

                        }
                    }

                    

                    if (timeToStayAtDestination <= 0)
                    {
                        SetToFlyingState();
                    }
                    if (sounds.mute)
                        sounds.mute = false;
                    
                }
                else
                {
                    if(Vector2.Distance(transform.position, flight.centerOfActiveArea.transform.position) >= 0.01f)
                    {
                        SetToFlyingState();
                        break;
                    }
                    animator.SetBool(sleeping_hash, true);
                    if (!sounds.mute)
                        sounds.mute = true;
                }

                

                break;




            case FlyingState.isLanding:

                SetBoidsState(false);
                flight.Fly();

                if ((Vector2)currentLandingSpot.transform.position != flight.currentDestination || flight.isOverWater || !flight.canReachNextTile)
                {
                    Deviate();
                    break;
                }
                

                if (Vector2.Distance(transform.position, flight.currentDestination) <= 0.01f && Vector2.Distance(gravityItem.itemObject.localPosition, flight.currentDestinationZ) <= 0.01f)
                {
                    timeToStayAtDestination = SetRandomRange(new Vector2(5.0f, 15.0f));
                    flight.isLanding = false;
                    animator.SetBool(landed_hash, true);
                    currentState = FlyingState.isAtDestination;
                }

                break;

            case FlyingState.isWalking:
                animator.SetBool(walking_hash, true);
                SetBoidsState(false);
                gravityItem.isWeightless = false;
                walk.Walk();

                timeToStayAtDestination -= Time.deltaTime;
                if (timeToStayAtDestination <= 0)
                {
                    SetToFlyingState();
                }
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

    void SetBoidsState(bool isInBoids)
    {
        if (flight.boid != null)
        {
            flight.useBoids = isInBoids;
            flight.boid.inBoidPool = isInBoids;
        }
        
    }

    void SetToFlyingState()
    {
        if (flight == null)
            return;
        flight.SetRandomDestination();
        animator.SetBool(landed_hash, false);
        justTookOff = true;
        detectionTimeOutAmount = SetRandomRange(5, 30);
        currentState = FlyingState.isFlying;
    }
    
    float SetRandomRange(Vector2 minMaxRange)
    {
        return Random.Range(minMaxRange.x, minMaxRange.y);
    }

    float SetRandomRange(float min, float max)
    {
        return Random.Range(min, max);
    }

    public void SetSleepOrWake(int tick)
    {
        var time = realTimeDayNightCycle.currentTimeRaw;
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
        musicItem.isActive = !isSleeping;
        
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
            if (item == null || item.isInUse || item.transform.position.z != transform.position.z)
                continue;
            var dist = Vector2.Distance(currentPosition, item.transform.position);
           
            if (dist < closestDistance)
            {
                if (dist < 1f)
                    continue;
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

    public void SetActiveState(bool active)
    {
        activeState = active;
    }

    public void FleePlayer(Transform playerTransform)
    {
        SetToFlyingState();
    }
}
