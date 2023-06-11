using Cinemachine;
using Klaxon.GravitySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_AI : MonoBehaviour
{
    public enum NPCState
    {
        Idle,
        Wandering,
        FollowNodePath,
        Activity,
        Sleeping,
        Talking, 
        Deviate
    }
    public NPCState currentState;
    public NPCState lastState;
    public NPCState nextState;

    GravityItemWalker walker;
    
    float timeIdle = 0;
    float maxIdleTime;

    public Animator animator;
    //CanReachTileWalk walk;
    CurrentTilePosition currentTilePosition;

    //bool hasDeviatePosition;


    

    bool disolved;
    Vector2 offset;

    public Vector2Int goToActivityTime;
    public Vector2Int goToHomeTime;
    public Vector2Int goToWanderTimeA;
    public Vector2Int goToWanderTimeB;
    bool atHome, atActivity, atWandering;
    NavigationNodeType currentNavigationNodeType;
    public float wanderDistance;

    List<NavigationNode> path = new List<NavigationNode>();
    int currentPathIndex;
    NavigationNode currentNode;
    NavigationNode finalNavigationNode;
    public NavigationNode homeInsideNode;
    public NavigationNode homeOutsideNode;
    public NavigationNode activityNode;

    static int isGrounded_hash = Animator.StringToHash("IsGrounded");
    static int velocityX_hash = Animator.StringToHash("VelocityX");
    static int velocityY_hash = Animator.StringToHash("VelocityY");

    

    
    //public Vector2 currentDirection;
    //public Vector2 currentDestination;

    private void Start()
    {
        currentTilePosition = GetComponent<CurrentTilePosition>();
        walker = GetComponent<GravityItemWalker>();
        maxIdleTime = Random.Range(1f, 5f);
        currentState = NPCState.Sleeping;
        GameEventManager.onTimeTickEvent.AddListener(SetStateOnTime);

        
    }
    private void OnDisable()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(SetStateOnTime);
    }

    private void SetStateOnTime(int tick)
    {
        if (currentState == NPCState.Talking)
            return;

        //bool hasActivity = goToActivityTime != 0;
        //int dayThingsTime = hasActivity ? goToActivityTime : goToHomeTime;

        if (tick >= goToWanderTimeA.x && tick < goToWanderTimeA.y && !atWandering)
        {
            finalNavigationNode = homeOutsideNode;
            currentState = NPCState.FollowNodePath;
            nextState = NPCState.Idle;
            
        }
        else if (tick >= goToWanderTimeB.x && tick < goToWanderTimeB.y && !atWandering)
        {
            currentState = NPCState.Idle;
        }
        else if (tick >= goToActivityTime.x && tick < goToActivityTime.y && !atActivity)
        {
            finalNavigationNode = activityNode;
            currentState = NPCState.FollowNodePath;
            nextState = NPCState.Activity;
            
        }
        else if ((tick >= goToHomeTime.x || tick < goToHomeTime.y) && !atHome)
        {
            finalNavigationNode = homeInsideNode;
            currentState = NPCState.FollowNodePath;
            nextState = NPCState.Sleeping;
            
        }
            
    }

    
    private void Update()
    {
        if (RealTimeDayNightCycle.instance.isPaused)
            return;


        switch (currentState)
        {

            case NPCState.Idle:
                timeIdle += Time.deltaTime;
                atWandering = true;
                atHome = false;
                atActivity = false;
                walker.currentDir = Vector2.zero;
                animator.SetFloat(velocityX_hash, 0);
                if (timeIdle > maxIdleTime)
                {
                    walker.SetRandomDestination(wanderDistance);
                    timeIdle = 0;
                    currentState = NPCState.Wandering;
                }
                walker.ResetLastPosition();
                break;

            case NPCState.Wandering:
                atWandering = true;
                atHome = false;
                atActivity = false;
                lastState = currentState;
                currentNavigationNodeType = NavigationNodeType.Outside;
                animator.SetFloat(velocityX_hash, 1);
                walker.hasDeviatePosition = false;
                if (walker.isStuck)
                {
                    if (!walker.jumpAhead) 
                        currentState = NPCState.Deviate;
                }
                walker.SetDirection();


                if (walker.CheckDistanceToDestination() <= 0.01f)
                {
                    maxIdleTime = Random.Range(1f, 5f);
                    currentState = NPCState.Idle;
                }
                walker.SetLastPosition();
                break;

            case NPCState.Deviate:
                // deviate mf
                if (walker.isStuck)
                {
                    walker.hasDeviatePosition = false;
                }

                if (!walker.hasDeviatePosition)
                {
                    walker.FindDeviateDestination(walker.tilemapObstacle ? 20 : 50);
                }
                animator.SetFloat(velocityX_hash, 1);
                walker.SetDirection();
                
                if (walker.CheckDistanceToDestination() <= 0.02f)
                {
                    walker.SetRandomDestination(wanderDistance);
                    currentState = lastState;
                }
                    

                walker.SetLastPosition();
                break;


            case NPCState.FollowNodePath:
                walker.hasDeviatePosition = false;
                lastState = currentState;
                if (walker.isStuck)
                {
                    if (!walker.jumpAhead)
                        currentState = NPCState.Deviate;
                }

                if (currentNode == null && path.Count <= 0)
                {
                    path.Clear();
                    currentPathIndex = 0;
                    currentNode = NavigationNodesManager.instance.GetClosestNavigationNode(transform.position, currentNavigationNodeType, NavigationPathType.BluValley);
                    path = currentNode.FindPath(finalNavigationNode);
                }
                animator.SetFloat(velocityX_hash, 1);
                if (path.Count > 0)
                {
                    walker.currentDestination = path[currentPathIndex].transform.position;
                }
                if(!walker.onSlope)
                    walker.SetDirection();

                if (walker.CheckDistanceToDestination() <= 0.02f)
                {
                    if (currentPathIndex < path.Count - 1)
                    {
                        currentPathIndex++;
                        walker.currentDestination = path[currentPathIndex].transform.position;
                    }
                    else if(currentPathIndex == path.Count - 1)
                    {
                        path.Clear();
                        currentPathIndex = 0;
                        currentNode = null;
                        currentState = nextState;
                    }
                }

                walker.SetLastPosition();


                break;

            case NPCState.Activity:
                walker.currentDir = Vector2.zero;
                atWandering = false;
                atHome = false;
                atActivity = true;
                animator.SetFloat(velocityX_hash, 0);
                walker.ResetLastPosition();
                break;

            case NPCState.Sleeping:
                walker.currentDir = Vector2.zero;
                currentNavigationNodeType = NavigationNodeType.Inside;
                atWandering = false;
                atHome = true;
                atActivity = false;
                animator.SetFloat(velocityX_hash, 0);
                walker.ResetLastPosition();
                break;

            case NPCState.Talking:
                walker.currentDir = Vector2.zero;
                animator.SetFloat(velocityX_hash, 0);
                FacePlayer();
                
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
    void FacePlayer()
    {
        float npcX = transform.position.x;
        float playerX = PlayerInformation.instance.player.position.x;
        if (npcX > playerX && walker.facingRight)
            walker.Flip();
        else if (npcX < playerX && !walker.facingRight)
            walker.Flip();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            lastState = currentState;
            currentState = NPCState.Talking;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            currentState = lastState;
        }
    }


}
