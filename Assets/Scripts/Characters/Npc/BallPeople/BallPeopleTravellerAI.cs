using Klaxon.GravitySystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallPeopleTravellerAI : MonoBehaviour, IBallPerson
{
    public enum TravellerState
    {
        Appear,
        Follow,
        Idle,
        Sleep,
        GoToDestination,
        AtFinalDestination,
        Deviate,
        Disappear,
        Remove
    }

    public TravellerState currentState;

    //List<Material> materials = new List<Material>();
    List<SpriteRenderer> allSprites = new List<SpriteRenderer>();
    float timeIdle = 0;

    
    public Animator animator;
    public GameObject arms;

    CanReachTileWalk walk;
    CurrentTilePosition currentTilePosition;

    bool hasDeviatePosition;


    [HideInInspector]
    public Vector3 currentDestination;
    Vector3 lastPosition;

    bool talkComplete;
    bool disolved;
    Vector2 offset;


    public Vector3 travellerDestination;

    static int walking_hash = Animator.StringToHash("IsWalking");
    static int isGrounded_hash = Animator.StringToHash("IsGrounded");
    static int velocityY_hash = Animator.StringToHash("VelocityY");
    static int sleeping_hash = Animator.StringToHash("IsSleeping");
    [HideInInspector]
    public bool hasInteracted;
    InteractableBallPeopleTraveller interactableTraveller;
    [HideInInspector]
    public bool hasFoundDestination;

    public float deviateThreshold;
    private void Start()
    {
        interactableTraveller = GetComponent<InteractableBallPeopleTraveller>();
        currentTilePosition = GetComponent<CurrentTilePosition>();
        walk = GetComponent<CanReachTileWalk>();
        allSprites = GetComponentsInChildren<SpriteRenderer>().ToList();
        foreach (var sprite in allSprites)
        {
            sprite.material.SetFloat("_Fade", 0);
        }
    }

    private void OnEnable()
    {

        currentState = TravellerState.Appear;
    }

    private void Update()
    {
        float dist = 1000;
        switch (currentState)
        {
            case TravellerState.Appear:

                Disolve(true);
                break;

            case TravellerState.Follow:
                // get path to player, follow player, if too close go to idle
                dist = Vector2.Distance(lastPosition, transform.position);

                if (lastPosition != transform.position)
                {
                    lastPosition = transform.position;
                }
                else
                {
                    if (!walk.jumpAhead)
                        currentState = TravellerState.Deviate;
                }
                    

                
                if(CheckDestinationDistance() <= 0.7f && hasInteracted && !hasFoundDestination)
                {
                    hasFoundDestination = true;
                    currentState = TravellerState.GoToDestination;
                }

                timeIdle = 0;
                hasDeviatePosition = false;
                if (CheckPlayerDistance() > 3)
                    currentState = TravellerState.Disappear;

                animator.SetBool(walking_hash, true);
                currentDestination = PlayerInformation.instance.player.position + (Vector3)offset;
                walk.SetWorldDestination(currentDestination);
                if (CheckDistanceToDestination() <= 0.02f)
                {
                    currentState = TravellerState.Idle;
                }


                walk.Walk();

                break;

            case TravellerState.Deviate:
                // deviate mf
                if (lastPosition != transform.position)
                    lastPosition = transform.position;
                else
                    hasDeviatePosition = false;

                if (!hasDeviatePosition)
                {
                    offset = new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
                    FindDeviateDestination();

                }
                animator.SetBool(walking_hash, true);

                walk.SetWorldDestination(currentDestination);

                if (CheckDistanceToDestination() <= 0.02f)
                {
                    offset = new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
                    if (!hasFoundDestination)
                        currentState = TravellerState.Follow;
                    else
                        currentState = TravellerState.GoToDestination;
                        
                }
                walk.Walk();
                if (CheckPlayerDistance() > 1.5f)
                    currentState = TravellerState.Disappear;

                break;



            case TravellerState.Idle:

                var dir = PlayerInformation.instance.player.position - transform.position;
                walk.SetFacingDirection(dir);
                arms.SetActive(!hasInteracted);
                interactableTraveller.canInteract = !hasInteracted;
                //check distance from player, wait a sec, and start to follow if too far
                hasFoundDestination = false;
                animator.SetBool(walking_hash, false);
                timeIdle += Time.deltaTime;
                if (timeIdle >= 10.0f)
                {
                    timeIdle = 0;
                    currentState = TravellerState.Sleep;
                }
                
                    
                if (CheckPlayerDistance() > 3)
                {
                    timeIdle = 0;
                    currentState = TravellerState.Disappear;
                }

                if (timeIdle > 0.5f)
                {
                    if (CheckPlayerDistance() >= 0.5f)
                    {
                        offset = new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
                        timeIdle = 0;
                        interactableTraveller.canInteract = false;
                        currentState = TravellerState.Follow;
                    }

                }
                
                break;

            case TravellerState.Sleep:

                // get another bool to know if i want to keep interactor off after reading... i know what i mean.

                //interactor.canInteract = false;
                //check distance from player, wait a sec, and start to follow if too far

                animator.SetBool(sleeping_hash, true);

                if (!hasFoundDestination)
                {
                    if (CheckPlayerDistance() >= 0.6f && !CheckUndertakingTasks())
                    {
                        animator.SetBool(sleeping_hash, false);
                        currentState = TravellerState.Follow;
                    }
                }
                else
                {
                    if (CheckPlayerDistance() <= 0.3f)
                    {
                        animator.SetBool(sleeping_hash, false);
                        currentState = TravellerState.AtFinalDestination;
                    }
                }
                    
                break;

            case TravellerState.GoToDestination:

                if (lastPosition != transform.position)
                {
                    lastPosition = transform.position;
                }
                else
                {
                    if (!walk.jumpAhead)
                        currentState = TravellerState.Deviate;
                }

                
                if (timeIdle >= 10.0f)
                {
                    timeIdle = 0;
                    currentState = TravellerState.Sleep;
                }
                currentDestination = travellerDestination + (Vector3)offset;
                walk.SetWorldDestination(currentDestination);
                if (CheckDistanceToDestination() <= 0.02f)
                {
                    if(CheckUndertakingTasks())
                        hasInteracted = false;
                    
                    currentState = TravellerState.AtFinalDestination;
                }
                walk.Walk();

                break;

            case TravellerState.AtFinalDestination:

                var lookDir = PlayerInformation.instance.player.position - transform.position;
                walk.SetFacingDirection(lookDir);
                bool taskComplete = CheckUndertakingTasks();


                if (interactableTraveller.undertaking.undertaking.CurrentState != Klaxon.UndertakingSystem.UndertakingState.Complete && taskComplete)
                {
                    arms.SetActive(true);
                    interactableTraveller.canInteract = true;
                    
                }
                else
                {
                    arms.SetActive(false);
                    interactableTraveller.canInteract = false;
                }

                if (CheckPlayerDistance() >= 0.6f && !taskComplete)
                {
                    
                    currentState = TravellerState.Follow;
                }

                animator.SetBool(walking_hash, false);
                timeIdle += Time.deltaTime;
                if (timeIdle >= 10.0f)
                {
                    timeIdle = 0;
                    currentState = TravellerState.Sleep;
                }
                
                break;

            case TravellerState.Disappear:
                if (!disolved)
                    Disolve(false);

                if (timeIdle < 1.5f)
                    timeIdle += Time.deltaTime;
                else
                    SetPositionNearPlayer();
                break;

            case TravellerState.Remove:
                if (!disolved)
                {
                    timeIdle = 0;
                    Disolve(false);
                }


                if (timeIdle < 2f)
                    timeIdle += Time.deltaTime;
                else
                    Destroy(gameObject);
                break;

        }

    }

    bool CheckUndertakingTasks()
    {
        bool tasksComplete = true;
        for (int i = 0; i < interactableTraveller.undertaking.undertaking.Tasks.Count; i++)
        {
            if (interactableTraveller.undertaking.undertaking.Tasks[i] == interactableTraveller.undertaking.task)
                continue;
            if (!interactableTraveller.undertaking.undertaking.Tasks[i].IsComplete)
                tasksComplete = false;
        }
        return tasksComplete;
    }

    void LateUpdate()
    {
        if (walk.gravityItem == null)
            return;
        animator.SetBool(isGrounded_hash, walk.gravityItem.isGrounded);
        animator.SetFloat(velocityY_hash, walk.gravityItem.isGrounded ? 0 : walk.gravityItem.displacedPosition.y);

    }

    void SetPositionNearPlayer()
    {
        Vector2 offset = new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
        transform.position = PlayerInformation.instance.player.position + (Vector3)offset;
        currentTilePosition.position = currentTilePosition.GetCurrentTilePosition(transform.position);
        currentState = TravellerState.Appear;
    }
    void FindDeviateDestination()
    {
        Vector2 offset = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        offset = offset.normalized;
        Vector3 checkPosition = (transform.position + (Vector3)offset * walk.gravityItem.checkTileDistance) - Vector3.forward;
        Vector3 doubleCheckPosition = transform.position - Vector3.forward;
        if (walk.gravityItem.CheckForObstacles(checkPosition, doubleCheckPosition, offset))
        {
            FindDeviateDestination();
        }
        else
        {
            currentDestination = transform.position + (Vector3)offset * .2f;
            hasDeviatePosition = true;
        }
    }

    void Disolve(bool disolveIn)
    {
        for (int i = 0; i < allSprites.Count; i++)
        {
            if (allSprites[i].gameObject.activeSelf)
                DissolveEffect.instance.StartDissolve(allSprites[i].material, 1f, disolveIn);
        }
        if (disolveIn)
            currentState = TravellerState.Idle;
        disolved = !disolveIn;
    }

    float CheckPlayerDistance()
    {
        float dist = Vector2.Distance(transform.position, PlayerInformation.instance.player.position);

        return dist;
    }

    float CheckDestinationDistance()
    {
        float dist = Vector2.Distance(transform.position, travellerDestination);

        return dist;
    }

    float CheckDistanceToDestination()
    {
        float dist = Vector2.Distance(transform.position, currentDestination);

        return dist;
    }

    public void SetToRemoveState()
    {
        currentState = TravellerState.Remove;
    }
}
