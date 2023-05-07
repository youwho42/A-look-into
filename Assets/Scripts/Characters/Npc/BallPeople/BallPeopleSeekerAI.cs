using Klaxon.GravitySystem;
using Klaxon.UndertakingSystem;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class BallPeopleSeekerAI : MonoBehaviour, IBallPerson
{

    public enum SeekerState
    {
        Appear,
        Follow,
        Idle,
        Sleep,
        GoToObject,
        AtObject,
        Deviate,
        Disappear,
        Remove
    }

    public SeekerState currentState;

    List<SpriteRenderer> allSprites = new List<SpriteRenderer>();
    float timeIdle = 0;

    
    public Animator animator;
    GravityItemWalker walker;

    bool talkComplete;
    bool disolved;
    Vector2 offset;


    InteractableBallPeopleSeeker interactor;
    public GameObject arms;

    static int walking_hash = Animator.StringToHash("IsWalking");
    static int isGrounded_hash = Animator.StringToHash("IsGrounded");
    static int velocityY_hash = Animator.StringToHash("VelocityY");
    static int sleeping_hash = Animator.StringToHash("IsSleeping");
    static int lick_hash = Animator.StringToHash("Lick");

    bool hasLicked;

    [HideInInspector]
    public bool hasInteracted;

    [HideInInspector]
    public Vector3 currentObjectPosition;
    //[HideInInspector]
    public QI_ItemData seekItem;
    //[HideInInspector]
    public int seekAmount;
    //[HideInInspector]
    public int foundAmount;
    [HideInInspector]
    public List<Vector3> seekItemsFound = new List<Vector3>();
    public float seekRadius;
    public LayerMask interactableLayer;
    GameObject currentSeekItem;
    public LayerMask obstacleLayer;

    public CompleteTaskObject seekTask; 

    private void Start()
    {
        interactor = GetComponent<InteractableBallPeopleSeeker>();
        walker = GetComponent<GravityItemWalker>();
        allSprites = GetComponentsInChildren<SpriteRenderer>().ToList();
        foreach (var sprite in allSprites)
        {
            sprite.material.SetFloat("_Fade", 0);
        }
    }

    private void OnEnable()
    {

        currentState = SeekerState.Appear;
    }

    private void Update()
    {

        switch (currentState)
        {
            case SeekerState.Appear:

                walker.currentDir = Vector2.zero;
                Disolve(true);
                walker.ResetLastPosition();
                break;

            case SeekerState.Follow:
                if (walker.isStuck)
                {
                    if (!walker.jumpAhead)
                        currentState = SeekerState.Deviate;
                }


                timeIdle = 0;
                walker.hasDeviatePosition = false;
                if (CheckPlayerDistance() > 3)
                    currentState = SeekerState.Disappear;

                animator.SetBool(walking_hash, true);
                walker.currentDestination = PlayerInformation.instance.player.position + (Vector3)offset;
                walker.SetWorldDestination(walker.currentDestination);
                walker.SetDirection();
                if (walker.CheckDistanceToDestination() <= 0.02f)
                {
                    currentState = SeekerState.Idle;
                }
                
                if (walker.CheckDistanceToDestination() <= 0.02f)
                {
                    currentState = SeekerState.Idle;
                }
                if (hasInteracted && !seekTask.task.IsComplete)
                {
                    currentSeekItem = CheckForSeekItem();
                    if (currentSeekItem != null)
                    {
                        walker.currentDestination = GetSeekItemPosition();
                        currentState = SeekerState.GoToObject;
                    }
                }
                walker.SetLastPosition();

                break;

            case SeekerState.Deviate:
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

                walker.SetWorldDestination(walker.currentDestination);
                walker.SetDirection();
                if (walker.CheckDistanceToDestination() <= 0.02f)
                    currentState = SeekerState.Follow;

                
                if (CheckPlayerDistance() > 1.5f)
                    currentState = SeekerState.Disappear;
                walker.SetLastPosition();
                break;



            case SeekerState.Idle:
                walker.currentDir = Vector2.zero;
                hasLicked = false;
                var dir = PlayerInformation.instance.player.position - transform.position;
                walker.SetFacingDirection(dir);
                arms.SetActive(!hasInteracted);
                interactor.canInteract = !hasInteracted;
                //check distance from player, wait a sec, and start to follow if too far
                offset = new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
                animator.SetBool(walking_hash, false);

                timeIdle += Time.deltaTime;
                if (CheckPlayerDistance() > 3)
                {
                    timeIdle = 0;
                    currentState = SeekerState.Disappear;
                }

                if (timeIdle > 0.5f)
                {
                    if (CheckPlayerDistance() >= 0.5f)
                    {
                        interactor.canInteract = false;
                        currentState = SeekerState.Follow;
                    }

                }
                if (timeIdle >= 10.0f)
                {
                    timeIdle = 0;
                    currentState = SeekerState.Sleep;
                }
                walker.ResetLastPosition();
                break;

            case SeekerState.Sleep:

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
                        currentState = SeekerState.Follow;
                    }

                }
                walker.ResetLastPosition();
                break;

            case SeekerState.GoToObject:

                if (walker.isStuck)
                {
                    if (!walker.jumpAhead)
                        currentState = SeekerState.Deviate;
                }

                walker.currentDestination = GetSeekItemPosition();
                walker.SetWorldDestination(walker.currentDestination);
                walker.SetDirection();
                if (walker.CheckDistanceToDestination() <= 0.01f)
                {
                    seekItemsFound.Add(currentSeekItem.transform.position);
                    currentSeekItem = null;
                    currentState = SeekerState.AtObject;
                }
                walker.SetLastPosition();
                


                break;

            case SeekerState.AtObject:
                walker.currentDir = Vector2.zero;
                animator.SetBool(walking_hash, false);
                if (!hasLicked)
                {
                    timeIdle = 0;
                    animator.SetTrigger(lick_hash);
                    hasLicked = true;
                    foundAmount++;
                    
                    if (foundAmount == seekAmount)
                    {
                        seekTask.undertaking.TryCompleteTask(seekTask.task);
                        hasInteracted = false;
                    }
                    
                }
                if (timeIdle < 2f)
                    timeIdle += Time.deltaTime;
                else
                    currentState = SeekerState.Idle;
                walker.ResetLastPosition();
                break;

            case SeekerState.Disappear:
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

            case SeekerState.Remove:
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

    Vector3 GetSeekItemPosition()
    {
        Vector3 pos = currentSeekItem.transform.position;
        Vector2 dir = transform.position - pos;
        dir = dir.normalized;
        dir *= 0.09f;
        var colliders = currentSeekItem.GetComponentsInChildren<Collider2D>();
        foreach (var coll in colliders)
        {
            if(gameObject.layer == obstacleLayer)
            {
                pos = coll.ClosestPoint(transform.position);
                break;
            }
        }

        return pos + (Vector3)dir;
    }

    GameObject CheckForSeekItem()
    {

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, seekRadius, interactableLayer);

        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].transform.position.z != transform.position.z)
                    continue;
                if (colliders[i].TryGetComponent(out QI_Item item))
                {
                    if (item.Data == seekItem && !seekItemsFound.Contains(colliders[i].transform.position))
                    {
                        hasLicked = false;
                        
                        return colliders[i].gameObject;
                    }

                }
            }

        }
        return null;
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
                        currentState = SeekerState.Appear;
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
        if (disolveIn)
            currentState = SeekerState.Idle;
        disolved = !disolveIn;
    }

    float CheckPlayerDistance()
    {
        float dist = Vector2.Distance(transform.position, PlayerInformation.instance.player.position);

        return dist;
    }

    

    public void SetToRemoveState()
    {
        currentState = SeekerState.Remove;
    }
}
