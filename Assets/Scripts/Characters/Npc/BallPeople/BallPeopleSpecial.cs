using Klaxon.GravitySystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallPeopleSpecial : MonoBehaviour, IBallPerson
{
    public enum SpecialState
    {
        Appear,
        Follow,
        Idle,
        Sleep,
        Deviate,
        Disappear,
        Remove
    }
    public SpecialState currentState;

    List<SpriteRenderer> allSprites = new List<SpriteRenderer>();
    public SpriteRenderer accessory;
    RandomAccessories randomAccessory;
    float timeIdle = 0;
    
    public Animator animator;
    GravityItemWalk walker;



    
    bool talkComplete;
    bool disolved;
    Vector2 offset;

    static int walking_hash = Animator.StringToHash("IsWalking");
    static int isGrounded_hash = Animator.StringToHash("IsGrounded");
    static int velocityY_hash = Animator.StringToHash("VelocityY");
    static int sleeping_hash = Animator.StringToHash("IsSleeping");
    
    private void Start()
    {
        walker = GetComponent<GravityItemWalk>();
        randomAccessory = GetComponent<RandomAccessories>();
        allSprites = GetComponentsInChildren<SpriteRenderer>().ToList();
        foreach (var sprite in allSprites)
        {
            sprite.material.SetFloat("_Fade", 0);
        }
        randomAccessory.PopulateList();
        randomAccessory.SetAccessories(randomAccessory.GetAccessory(accessory));
    }
    private void OnEnable()
    {

        currentState = SpecialState.Appear;
    }
    private void Update()
    {

        switch (currentState)
        {
            case SpecialState.Appear:
                walker.currentDirection = Vector2.zero;
                Disolve(true);
                walker.ResetLastPosition();
                break;

            case SpecialState.Follow:
                // get path to player, follow player, if too close go to idle

                if (walker.isStuck)
                {
                    if (!walker.jumpAhead)
                        currentState = SpecialState.Deviate;
                }
                

                timeIdle = 0;
                walker.hasDeviatePosition = false;
                if (CheckPlayerDistance() > 3)
                    currentState = SpecialState.Disappear;

                animator.SetBool(walking_hash, true);
                walker.currentDestination = PlayerInformation.instance.player.position + (Vector3)offset;
                walker.SetWorldDestination(walker.currentDestination);
                walker.SetDirection();
                if (walker.CheckDistanceToDestination() <= 0.02f)
                {
                    currentState = SpecialState.Idle;
                }

                walker.SetLastPosition();
                break;

            case SpecialState.Deviate:
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
                    currentState = SpecialState.Follow;

                
                if (CheckPlayerDistance() > 1.5f)
                    currentState = SpecialState.Disappear;
                walker.SetLastPosition();
                break;



            case SpecialState.Idle:
                walker.currentDirection = Vector2.zero;
                var dir = PlayerInformation.instance.player.position - transform.position;
                walker.SetFacingDirection(dir);

                //check distance from player, wait a sec, and start to follow if too far
                offset = new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
                animator.SetBool(walking_hash, false);

                timeIdle += Time.deltaTime;
                if (CheckPlayerDistance() > 3)
                {
                    timeIdle = 0;
                    currentState = SpecialState.Disappear;
                }

                if (timeIdle > 0.5f)
                {
                    if (CheckPlayerDistance() >= 0.5f)
                    {
                        currentState = SpecialState.Follow;
                    }

                }
                if (timeIdle >= 10.0f)
                {
                    timeIdle = 0;
                    currentState = SpecialState.Sleep;
                }

                walker.ResetLastPosition();
                break;

            case SpecialState.Sleep:

                // get another bool to know if i want to keep interactor off after reading... i know what i mean.

                //check distance from player, wait a sec, and start to follow if too far
                walker.currentDirection = Vector2.zero;
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
                        currentState = SpecialState.Follow;
                    }

                }

                walker.ResetLastPosition();
                break;

            case SpecialState.Disappear:
                walker.currentDirection = Vector2.zero;
                animator.SetBool(walking_hash, false);
                if (!disolved)
                    Disolve(false);

                if (timeIdle < 1.5f)
                    timeIdle += Time.deltaTime;
                else
                    SetPositionNearPlayer();

                walker.ResetLastPosition();

                break;

            case SpecialState.Remove:
                walker.currentDirection = Vector2.zero;
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
                        currentState = SpecialState.Appear;
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
            currentState = SpecialState.Idle;
        disolved = !disolveIn;
    }

    float CheckPlayerDistance()
    {
        float dist = Vector2.Distance(transform.position, PlayerInformation.instance.player.position);

        return dist;
    }

    public void SetToRemoveState()
    {
        currentState = SpecialState.Remove;
    }
    public void InteractionFinished()
    {
        
    }

}
