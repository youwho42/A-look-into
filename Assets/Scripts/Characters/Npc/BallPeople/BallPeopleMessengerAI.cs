using Klaxon.GravitySystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BallPeopleSpecial;

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

    
    public Animator animator;
    GravityItemWalker walker;


    bool talkComplete;
    bool disolved;
    Vector2 offset;


    InteractableBallPeopleMessenger interactor;
    static int walking_hash = Animator.StringToHash("IsWalking");
    static int isGrounded_hash = Animator.StringToHash("IsGrounded");
    static int velocityY_hash = Animator.StringToHash("VelocityY");
    static int sleeping_hash = Animator.StringToHash("IsSleeping");
    [HideInInspector]
    public bool hasInteracted;
    

    private void Start()
    {
        interactor = GetComponent<InteractableBallPeopleMessenger>();
        walker = GetComponent<GravityItemWalker>();
        allSprites = GetComponentsInChildren<SpriteRenderer>().ToList();
        foreach (var sprite in allSprites)
        {
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
                walker.currentDir = Vector2.zero;
                Disolve(true);
                walker.ResetLastPosition();
                break;

            case MessengerState.Follow:
                // get path to player, follow player, if too close go to idle
                if (walker.isStuck)
                {
                    if (!walker.jumpAhead)
                        currentState = MessengerState.Deviate;
                }


                timeIdle = 0;
                walker.hasDeviatePosition = false;
                if (CheckPlayerDistance() > 3)
                    currentState = MessengerState.Disappear;
                
                animator.SetBool(walking_hash, true);
                walker.currentDestination = PlayerInformation.instance.player.position + (Vector3)offset;
                walker.SetWorldDestination(walker.currentDestination);
                walker.SetDirection();
                if (walker.CheckDistanceToDestination() <= 0.02f)
                {
                    currentState = MessengerState.Idle;
                }
                walker.SetLastPosition();
                break;

            case MessengerState.Deviate:
                if (walker.isStuck && walker.hasDeviatePosition)
                {
                    offset = new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
                    walker.currentDestination = PlayerInformation.instance.player.position + (Vector3)offset;
                    walker.SetDirection();
                    walker.hasDeviatePosition = false;
                }

                if (!walker.hasDeviatePosition)
                {
                    walker.FindDeviateDestination(walker.tilemapObstacle ? 20 : 50);
                }
                animator.SetBool(walking_hash, true);
                walker.SetDirection();


                if (walker.CheckDistanceToDestination() <= 0.02f)
                    currentState = MessengerState.Follow;

                if (CheckPlayerDistance() > 1.5f)
                    currentState = MessengerState.Disappear;

                walker.SetLastPosition();


                break;



            case MessengerState.Idle:
                walker.currentDir = Vector2.zero;
                var dir = PlayerInformation.instance.player.position - transform.position;
                walker.SetFacingDirection(dir);

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
                walker.ResetLastPosition();
                break;

            case MessengerState.Sleep:
                walker.currentDir = Vector2.zero;
                
                interactor.canInteract = false;
                
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
                walker.ResetLastPosition();
                break;

            case MessengerState.Disappear:
                walker.currentDir = Vector2.zero;
                animator.SetBool(walking_hash, false);
                if (!disolved)
                    Disolve(false);

                if (timeIdle < 1.5f)
                    timeIdle += Time.deltaTime;
                else
                    SetPositionNearPlayer();
                walker.ResetLastPosition();
                break;

            case MessengerState.Remove:
                walker.currentDir = Vector2.zero;
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
        if (walker == null)
            return;
        animator.SetBool(isGrounded_hash, walker.isGrounded);
        animator.SetFloat(velocityY_hash, walker.isGrounded ? 0 : walker.displacedPosition.y);

    }

    void SetPositionNearPlayer()
    {
        Vector2 offset = new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
        transform.position = PlayerInformation.instance.player.position + (Vector3)offset;
        if (walker.tileBlockInfo != null)
        {
            foreach (var tile in walker.tileBlockInfo)
            {
                
                if (tile.direction == Vector3Int.zero)
                {
                    if (tile.isValid)
                    {
                        walker.currentTilePosition.position = walker.currentTilePosition.GetCurrentTilePosition(transform.position);
                        currentState = MessengerState.Appear;
                    }
                    else
                    {
                        SetPositionNearPlayer();
                    }
                }
            }
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

    

    public void SetToRemoveState()
    {
        currentState = MessengerState.Remove;
    }
}
