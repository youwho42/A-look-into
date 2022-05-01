using UnityEngine;

public class BirdAI : MonoBehaviour, IAnimal
{
    
    float timeToStayAtDestination;
    bool justTookOff;
    public float detectionTimeOutAmount = 3;
    float detectionTimeOutTimer;
    

    DrawZasYDisplacement currentLandingSpot;
    GravityItem gravityItem;
    CanReachTileFlight flight;
    CanReachTileWalk walk;
    public Animator animator;
    float glideTimer;
    bool glide;

    bool isSleeping;
    public Transform home;
    DrawZasYDisplacement displacmentZ;
    AnimalSounds sounds;
    bool isRaining;

    public string landingSpotTagName;
   

    static int landed_hash = Animator.StringToHash("IsLanded");
    static int walking_hash = Animator.StringToHash("IsWalking");
    static int gliding_hash = Animator.StringToHash("IsGliding");

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

        //GameEventManager.onTimeHourEvent.AddListener(SetSleepOrWake);

        animator.SetBool(landed_hash, true);
        gravityItem = GetComponent<GravityItem>();
        flight = GetComponent<CanReachTileFlight>();
        walk = GetComponent<CanReachTileWalk>();
        SetHome(transform);
        glideTimer = SetRandomRange(3, 10);
        timeToStayAtDestination = SetRandomRange(new Vector2(5.0f, 15.0f));
        detectionTimeOutAmount = SetRandomRange(5, 60);
        flight.SetRandomDestination();
        displacmentZ = home.GetComponent<DrawZasYDisplacement>();
        
    }

    private void OnDestroy()
    {
        //GameEventManager.onTimeHourEvent.RemoveListener(SetSleepOrWake);

    }

    private void Update()
    {
        if (home == null)
            SetHome(transform);

        switch (currentState)
        {
            case FlyingState.isFlying:

                
                animator.SetBool(walking_hash, false);

                
                animator.SetBool(landed_hash, false);

                if (sounds.mute)
                    sounds.mute = false;




                flight.Fly();
                
                CheckForLandingArea();



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

                break;


            

            case FlyingState.isAtDestination:
                if (!isSleeping || !isRaining)
                {
                    glideTimer -= Time.deltaTime;
                    if (glideTimer <= 0)
                    {
                        glideTimer = SetRandomRange(0.2f, 5.0f);
                        animator.SetTrigger("Idle");
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
                        currentState = FlyingState.isFlying;
                        animator.SetBool(landed_hash, false);
                        justTookOff = true;
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

                flight.Fly();

                if (Vector2.Distance(transform.position, flight.currentDestination) <= 0.001f && Vector2.Distance(gravityItem.itemObject.localPosition, flight.currentDestinationZ) <= 0.001f)
                {
                    timeToStayAtDestination = SetRandomRange(new Vector2(5.0f, 15.0f));
                    flight.isLanding = false;
                    currentState = FlyingState.isAtDestination;
                    animator.SetBool(landed_hash, true);
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
    public void SetHome(Transform location)
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, 5);
        Collider2D nearest = null;
        float distance = 0;

        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].CompareTag("Open" + landingSpotTagName)) 
            {
                float tempDistance = Vector3.Distance(transform.position, hit[i].transform.position);
                if (nearest == null || tempDistance < distance)
                {
                    nearest = hit[i];
                    distance = tempDistance;
                }
            }
                
        }
        if(nearest != null)
        {
            home = nearest.transform;
            
        }
        flight.centerOfActiveArea = home;

    }

    float SetRandomRange(Vector2 minMaxRange)
    {
        return Random.Range(minMaxRange.x, minMaxRange.y);
    }

    float SetRandomRange(float min, float max)
    {
        return Random.Range(min, max);
    }

    /*public void SetSleepOrWake(int time)
    {
        if (time == 21)
        {

            flight.SetDestination(home.position, displacmentZ.displacedPosition);
            currentState = FlyingState.isLanding;

            isSleeping = true;

        }
        else if (time == 6)
        {
            isSleeping = false;
        }
    }*/

    void CheckForLandingArea()
    {
        if (justTookOff || currentState != FlyingState.isFlying)
            return;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, .5f);
        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                if (hit.GetComponent<DrawZasYDisplacement>() == null)
                    continue;
                if (hit.CompareTag("Open" + landingSpotTagName))
                {
                    var temp = hit.GetComponent<DrawZasYDisplacement>();
                    if (temp.isInUse)
                        continue;
                    flight.centerOfActiveArea = temp.gameObject.transform;
                    flight.SetDestination(temp);
                    
                    justTookOff = false;
                    detectionTimeOutAmount = SetRandomRange(5, 30);
                    currentLandingSpot = temp;
                    flight.isLanding = true;
                    currentLandingSpot.isInUse = true;
                    currentState = FlyingState.isLanding;
                    return;
                }
                if (hit.CompareTag("Player"))
                {
                    flight.SetRandomDestination();
                    
                    justTookOff = true;
                    detectionTimeOutAmount = SetRandomRange(5, 30);
                    animator.SetBool(landed_hash, false);
                    if (currentLandingSpot != null)
                    {
                        currentLandingSpot.tag = "Open" + landingSpotTagName;
                    }
                    currentState = FlyingState.isFlying;
                    return;
                }
            }
        }
    }

    

    
}
