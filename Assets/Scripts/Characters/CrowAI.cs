using UnityEngine;

public class CrowAI : MonoBehaviour, IAnimal
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
    float glideTimer;
    bool glide;

    bool isSleeping;
    public Transform home;
    DrawZasYDisplacement displacmentZ;
    AnimalSounds sounds;
    bool isRaining;

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
        if (home == null)
            SetHome(transform);
        sounds = GetComponent<AnimalSounds>();
        DayNightCycle.instance.FullHourEventCallBack.AddListener(SetSleepOrWake);
        animator.SetBool("isLanded", true);
        flight = GetComponent<CharacterFlight>();
        walk = GetComponent<CharacterWalk>();
        glideTimer = SetRandomRange(3, 10);
        timeToStayAtDestination = SetRandomRange(new Vector2(5.0f, 15.0f));
        detectionTimeOutAmount = SetRandomRange(5, 60);

        displacmentZ = home.GetComponent<DrawZasYDisplacement>();
        
    }

    private void Update()
    {
        if (home == null)
            SetHome(transform);

        switch (currentState)
        {
            case FlyingState.isFlying:

                if (animator.GetBool("IsWalking"))
                    animator.SetBool("IsWalking", false);

                if (animator.GetBool("isLanded"))
                    animator.SetBool("isLanded", false);

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
                glideTimer -= Time.deltaTime;
                if (glideTimer <= 0)
                {
                    glide = !glide;
                    glideTimer = glide ? SetRandomRange(0.5f, 4) : SetRandomRange(0.5f, 10.0f);
                    animator.SetBool("isGliding", glide);
                }


                if (Vector2.Distance(transform.position, flight.currentDestination) <= 0.01f)
                {
                    if (!isSleeping)
                    {
                        flight.SetRandomDestination(roamingArea);
                        if (glide)
                        {
                            glide = false;
                            glideTimer = SetRandomRange(0.8f, 10);
                            animator.SetBool("isGliding", glide);
                        }
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
                    glideTimer -= Time.deltaTime;
                    if (glideTimer <= 0)
                    {
                        glideTimer = SetRandomRange(0.2f, 5.0f);
                        animator.SetTrigger("Idle");
                    }

                    timeToStayAtDestination -= Time.deltaTime;

                    if (flight.characterSprite.transform.localPosition.y == 0)
                    {
                        if (timeToStayAtDestination >= 4)
                        {
                            if (Random.Range(0.0f, 1.0f) <= 0.0075f)
                            {
                                walk.SetRandomDestination();
                                animator.SetBool("IsWalking", true);
                                currentState = FlyingState.isWalking;
                            }

                        }
                    }

                    if (timeToStayAtDestination <= 3)
                        flight.SetFacingDirection();

                    if (timeToStayAtDestination <= 0)
                    {
                        flight.SetRandomDestination(roamingArea);
                        currentState = FlyingState.isFlying;
                        animator.SetBool("isLanded", false);
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
                flight.Move();
                if (Vector2.Distance(transform.position, flight.currentDestination) <= 0.01f)
                {
                    currentState = FlyingState.isAtDestination;
                    animator.SetBool("isLanded", true);
                }

                break;

            case FlyingState.isWalking:
                walk.Move();

                if (Vector2.Distance(transform.position, walk.currentDestination) <= 0.01f)
                {
                    animator.SetBool("IsWalking", false);
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
        if(nearest != null)
            home = nearest.transform;
        
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
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<DrawZasYDisplacement>() != null && !justTookOff && collision.CompareTag("OpenCrowSpot") && currentState == FlyingState.isFlying)
        {
            var temp = collision.GetComponent<DrawZasYDisplacement>();
            flight.SetDestination(temp.transform.position, temp.displacedPosition);
            timeToStayAtDestination = SetRandomRange(5, 30);
            justTookOff = true;
            detectionTimeOutTimer = 0;
            detectionTimeOutAmount = SetRandomRange(5, 60);
            currentLandingSpot = collision;
            currentLandingSpot.tag = "ClosedCrowSpot";
            currentState = FlyingState.isLanding;
            
        }
        if (collision.CompareTag("Player"))
        {
            flight.SetRandomDestination(roamingArea);
            currentState = FlyingState.isFlying;
            justTookOff = true;
            detectionTimeOutTimer = 0;
            detectionTimeOutAmount = SetRandomRange(5,60);
            animator.SetBool("isLanded", false);
            
            if (currentLandingSpot != null)
            {
                currentLandingSpot.tag = "OpenCrowSpot";
            }
            
        }
        if (collision.CompareTag("RainStorm") && !isRaining && !isSleeping)
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
                detectionTimeOutAmount = SetRandomRange(5, 60);
                animator.SetBool("isLanded", false);

                if (currentLandingSpot != null)
                {
                    currentLandingSpot.tag = "OpenCrowSpot";
                }
            }
            isRaining = false;
        }
    }

    
}
