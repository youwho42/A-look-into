using Klaxon.GravitySystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallPeopleMessengerAI : MonoBehaviour, IBallPerson
{
    
    public enum MessengerState
    {
        Appear,
        Follow,
        Idle,
        Sleep,
        Deviate,
        Disappear,
        Remove
    }

    public MessengerState currentState;

    //List<Material> materials = new List<Material>();
    List<SpriteRenderer> allSprites = new List<SpriteRenderer>();
    float timeIdle = 0;

    static int walking_hash = Animator.StringToHash("IsWalking");
    public Animator animator;
    CanReachTileWalk walk;
    CurrentTilePosition currentTilePosition;

    bool hasDeviatePosition;
    
    
    [HideInInspector]
    public Vector3 currentDestination;
    Vector3 lastPosition;

    bool talkComplete;
    bool disolved;
    Vector2 offset;


    InteractableBallPeopleMessenger interactor;
    
    static int isGrounded_hash = Animator.StringToHash("IsGrounded");
    static int velocityY_hash = Animator.StringToHash("VelocityY");
    static int sleeping_hash = Animator.StringToHash("IsSleeping");
    [HideInInspector]
    public bool hasInteracted;
    

    private void Start()
    {
        interactor = GetComponent<InteractableBallPeopleMessenger>();
        currentTilePosition = GetComponent<CurrentTilePosition>();
        walk = GetComponent<CanReachTileWalk>();
        allSprites = GetComponentsInChildren<SpriteRenderer>().ToList();
        foreach (var sprite in allSprites)
        {
            //materials.Add(sprite.material);
            sprite.material.SetFloat("_Fade", 0);
        }
    }

    private void OnEnable()
    {
        
        currentState = MessengerState.Appear;
    }

    private void Update()
    {

        switch (currentState)
        {
            case MessengerState.Appear:
                
                Disolve(true);
                break;

            case MessengerState.Follow:
                // get path to player, follow player, if too close go to idle
                if (lastPosition != transform.position)
                {
                    lastPosition = transform.position;
                }
                else
                {
                    if(!walk.jumpAhead)
                        currentState = MessengerState.Deviate;
                }
                    

                timeIdle = 0;
                hasDeviatePosition = false;
                if (CheckPlayerDistance() > 3)
                    currentState = MessengerState.Disappear;
                
                animator.SetBool(walking_hash, true);
                currentDestination = PlayerInformation.instance.player.position + (Vector3)offset;
                walk.SetWorldDestination(currentDestination);
                if (CheckDistanceToDestination() <= 0.02f)
                {
                    currentState = MessengerState.Idle;
                }

                
                walk.Walk();

                break;

            case MessengerState.Deviate:
                // deviate mf
                if (lastPosition != transform.position)
                    lastPosition = transform.position;
                else
                    hasDeviatePosition = false;

                if (!hasDeviatePosition)
                {
                    FindDeviateDestination();
                    
                }
                animator.SetBool(walking_hash, true);

                walk.SetWorldDestination(currentDestination);

                if (CheckDistanceToDestination() <= 0.02f)
                    currentState = MessengerState.Follow;

                walk.Walk();
                if (CheckPlayerDistance() > 1.5f)
                    currentState = MessengerState.Disappear;
                break;



            case MessengerState.Idle:

                var dir = PlayerInformation.instance.player.position - transform.position;
                walk.SetFacingDirection(dir);

                interactor.canInteract = !hasInteracted;
                //check distance from player, wait a sec, and start to follow if too far
                offset = new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
                animator.SetBool(walking_hash, false);
                
                timeIdle += Time.deltaTime;
                if (CheckPlayerDistance() > 3)
                {
                    timeIdle = 0;
                    currentState = MessengerState.Disappear;
                }
                
                if (timeIdle > 0.5f)
                {
                    if (CheckPlayerDistance() >= 0.5f)
                    {
                        interactor.canInteract = false;
                        currentState = MessengerState.Follow;
                    }
                        
                }
                if (timeIdle >= 10.0f)
                {
                    timeIdle = 0;
                    currentState = MessengerState.Sleep;
                }
                break;

            case MessengerState.Sleep:

                // get another bool to know if i want to keep interactor off after reading... i know what i mean.

                interactor.canInteract = false;
                //check distance from player, wait a sec, and start to follow if too far
                
                animator.SetBool(sleeping_hash, true);
                
                if (timeIdle < 0.5f)
                {
                    timeIdle += Time.deltaTime;
                }
                else
                {
                    if (CheckPlayerDistance() >= 0.31f)
                    {
                        animator.SetBool(sleeping_hash, false);
                        currentState = MessengerState.Follow;
                    }

                }
                break;

            case MessengerState.Disappear:
                if(!disolved)
                    Disolve(false);

                if (timeIdle < 1.5f)
                    timeIdle += Time.deltaTime;
                else
                    SetPositionNearPlayer();
                break;

            case MessengerState.Remove:
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
        currentState = MessengerState.Appear;
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
        if(disolveIn)
            currentState = MessengerState.Idle;
        disolved = !disolveIn;
    }

    float CheckPlayerDistance()
    {
        float dist = Vector2.Distance(transform.position, PlayerInformation.instance.player.position);
        
        return dist;
    }

    float CheckDistanceToDestination()
    {
        float dist = Vector2.Distance(transform.position, currentDestination);

        return dist;
    }

    public void SetToRemoveState()
    {
        currentState = MessengerState.Remove;
    }
}
