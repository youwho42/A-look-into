using Klaxon.GravitySystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BallPeopleMessengerAI;

public class BallPeopleTravelerAI : MonoBehaviour, IBallPerson
{
    public enum TravelerState
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

    public TravelerState currentState;

    List<SpriteRenderer> allSprites = new List<SpriteRenderer>();
    float timeIdle = 0;

    
    public Animator animator;
    public GameObject arms;

    GravityItemWalker walker;

    
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
    InteractableBallPeopleTraveller interactableTraveler;
    [HideInInspector]
    public bool hasFoundDestination;

    private void Start()
    {
        interactableTraveler = GetComponent<InteractableBallPeopleTraveller>();
        walker = GetComponent<GravityItemWalker>();
        allSprites = GetComponentsInChildren<SpriteRenderer>().ToList();
        foreach (var sprite in allSprites)
        {
            sprite.material.SetFloat("_Fade", 0);
        }
    }

    private void OnEnable()
    {

        currentState = TravelerState.Appear;
    }

    private void Update()
    {
        //float dist = 1000;
        switch (currentState)
        {
            case TravelerState.Appear:

                walker.currentDir = Vector2.zero;
                Disolve(true);
                walker.ResetLastPosition();
                break;

            case TravelerState.Follow:
                if (walker.isStuck)
                {
                    if (!walker.jumpAhead)
                        currentState = TravelerState.Deviate;
                }


                timeIdle = 0;
                walker.hasDeviatePosition = false;
                if (CheckPlayerDistance() > 3)
                    currentState = TravelerState.Disappear;

                animator.SetBool(walking_hash, true);
                walker.currentDestination = PlayerInformation.instance.player.position + (Vector3)offset;
                walker.SetWorldDestination(walker.currentDestination);
                walker.SetDirection();
                if (walker.CheckDistanceToDestination() <= 0.02f)
                {
                    currentState = TravelerState.Idle;
                }
                walker.SetLastPosition();

                break;

            case TravelerState.Deviate:
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

                if (CheckPlayerDistance() > 1.5f)
                    currentState = TravelerState.Disappear;

                walker.SetLastPosition();

                if (walker.CheckDistanceToDestination() <= 0.02f)
                {
                    if (!hasFoundDestination)
                    {
                        offset = new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
                        currentState = TravelerState.Follow;
                    }
                    else
                    {
                        offset = new Vector2(Random.Range(-0.15f, 0.15f), Random.Range(-0.15f, 0.15f));
                        currentState = TravelerState.GoToDestination;
                    }
                }
                
                break;



            case TravelerState.Idle:
                walker.currentDir = Vector2.zero;
                var dir = PlayerInformation.instance.player.position - transform.position;
                walker.SetFacingDirection(dir);
                arms.SetActive(!hasInteracted);
                interactableTraveler.canInteract = !hasInteracted;
                //check distance from player, wait a sec, and start to follow if too far
                hasFoundDestination = false;
                animator.SetBool(walking_hash, false);
                timeIdle += Time.deltaTime;
                if (timeIdle >= 10.0f)
                {
                    timeIdle = 0;
                    currentState = TravelerState.Sleep;
                }
                
                    
                if (CheckPlayerDistance() > 3)
                {
                    timeIdle = 0;
                    currentState = TravelerState.Disappear;
                }

                if (timeIdle > 0.5f)
                {
                    if (CheckPlayerDistance() >= 0.5f)
                    {
                        offset = new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
                        timeIdle = 0;
                        interactableTraveler.canInteract = false;
                        currentState = TravelerState.Follow;
                    }

                }
                walker.ResetLastPosition();
                break;


            case TravelerState.Sleep:
                walker.currentDir = Vector2.zero;
                interactableTraveler.canInteract = false;
                animator.SetBool(sleeping_hash, true);

                if (!hasFoundDestination)
                {
                    if (CheckPlayerDistance() >= 0.6f && !CheckUndertakingTasks())
                    {
                        animator.SetBool(sleeping_hash, false);
                        currentState = TravelerState.Follow;
                    }
                }
                else
                {
                    if (CheckPlayerDistance() <= 0.3f)
                    {
                        animator.SetBool(sleeping_hash, false);
                        currentState = TravelerState.AtFinalDestination;
                    }
                }
                walker.ResetLastPosition();
                break;

            case TravelerState.GoToDestination:

                if (walker.isStuck)
                {
                    if (!walker.jumpAhead)
                        currentState = TravelerState.Deviate;
                }

                timeIdle = 0;
                walker.hasDeviatePosition = false;

                walker.currentDestination = travellerDestination + (Vector3)offset;
                walker.SetWorldDestination(walker.currentDestination);
                walker.SetDirection();
                if (walker.CheckDistanceToDestination() <= 0.02f)
                {
                    if(CheckUndertakingTasks())
                        hasInteracted = false;
                    
                    currentState = TravelerState.AtFinalDestination;
                }
                walker.SetLastPosition();
                break;

            case TravelerState.AtFinalDestination:

                var lookDir = PlayerInformation.instance.player.position - transform.position;
                walker.SetFacingDirection(lookDir);
                bool taskComplete = CheckUndertakingTasks();

                if (interactableTraveler.undertaking.undertaking.CurrentState != Klaxon.UndertakingSystem.UndertakingState.Complete && taskComplete)
                {
                    arms.SetActive(true);
                    interactableTraveler.canInteract = true;
                }
                else
                {
                    arms.SetActive(false);
                    interactableTraveler.canInteract = false;
                }
                if (CheckPlayerDistance() >= 0.6f && !taskComplete)
                {
                    currentState = TravelerState.Follow;
                }
                animator.SetBool(walking_hash, false);
                timeIdle += Time.deltaTime;
                if (timeIdle >= 10.0f)
                {
                    timeIdle = 0;
                    currentState = TravelerState.Sleep;
                }
                walker.ResetLastPosition();
                break;

            case TravelerState.Disappear:
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

            case TravelerState.Remove:
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

    bool CheckUndertakingTasks()
    {
        bool tasksComplete = true;
        for (int i = 0; i < interactableTraveler.undertaking.undertaking.Tasks.Count; i++)
        {
            if (interactableTraveler.undertaking.undertaking.Tasks[i] == interactableTraveler.undertaking.task)
                continue;
            if (!interactableTraveler.undertaking.undertaking.Tasks[i].IsComplete)
                tasksComplete = false;
        }
        return tasksComplete;
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
                        currentState = TravelerState.Appear;
                    }
                    else
                    {
                        SetPositionNearPlayer();
                    }
                }
            }
        }
        //currentTilePosition.position = currentTilePosition.GetCurrentTilePosition(transform.position);
        //currentState = MessengerState.Appear;
    }
    

    void Disolve(bool disolveIn)
    {
        for (int i = 0; i < allSprites.Count; i++)
        {
            if (allSprites[i].gameObject.activeSelf)
                DissolveEffect.instance.StartDissolve(allSprites[i].material, 1f, disolveIn);
        }
        if (disolveIn)
            currentState = TravelerState.Idle;
        disolved = !disolveIn;
    }

    float CheckPlayerDistance()
    {
        float dist = Vector2.Distance(transform.position, PlayerInformation.instance.player.position);

        return dist;
    }

    
   

    public void SetToRemoveState()
    {
        currentState = TravelerState.Remove;
    }
}
