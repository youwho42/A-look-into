using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using UnityEngine;

public class NPC_BasicAI : MonoBehaviour
{
    MoveToNode moveToNode;
    public CurrentTilePosition homeBaseTarget;
    public CurrentTilePosition bedBaseTarget;
    public CurrentTilePosition activityBaseTarget;

    bool atHome, atActivity;
    public int goToActivityTime; 
    public int goToHomeTime; 
    public int wanderTime;
    public int wanderDistance;
    bool gettingPath;

    float idleTimer;
    NPCStates lastState;

    public NPCStates currentState;
    public enum NPCStates
    {
        Idle,
        Wandering,
        GoingToActivity,
        Activity,
        GoingHome,
        GoingToBed,
        Sleeping, 
        Talking
    }



    private IEnumerator Start()
    {
        
        moveToNode = GetComponent<MoveToNode>();
        currentState = NPCStates.Idle;
        

        yield return new WaitForSeconds(1);
        GameEventManager.onTimeTickEvent.AddListener(SetStateOnTime);
        atHome = CheckDistanceToDestination(homeBaseTarget.position);
    }

    public void SetStateOnTime(int time)
    {
        if (currentState == NPCStates.Talking)
            return;
        if (time >= wanderTime && time < goToActivityTime)
            currentState = NPCStates.Wandering;
        else if (time >= goToActivityTime && time < goToHomeTime && !atActivity)
            currentState = NPCStates.GoingToActivity;
        else if (!atHome && (time >= goToHomeTime || time < wanderTime))
            currentState = NPCStates.GoingHome;
    }
            



    private void Update()
    {
        switch (currentState)
        {
            case NPCStates.Idle:
                idleTimer -= Time.deltaTime;
                if(idleTimer < 0)
                {
                    idleTimer = Random.Range(1.0f, 10.0f);
                    moveToNode.Flip();
                }
                break;

            case NPCStates.Wandering:
                atActivity = false;
                atHome = false;
                if(!gettingPath && !moveToNode.hasPath)
                    SetDestination(PathRequestManager.GetRandomDistancedTile(moveToNode.currentTilePosition.position, wanderDistance));

                if (moveToNode.hasPath)
                    moveToNode.Move();

                break;

            case NPCStates.GoingToActivity:

                if (CheckDistanceToDestination(activityBaseTarget.position))
                {
                    atActivity = true;
                    moveToNode.ClearPath();
                    currentState = NPCStates.Idle;
                }
                  
                if (!gettingPath && !moveToNode.hasPath)
                    SetDestination(activityBaseTarget.position);

                if (moveToNode.hasPath)
                    moveToNode.Move();
                
                break;

            case NPCStates.Activity:
                
                break;

            case NPCStates.GoingHome:

                if (CheckDistanceToDestination(homeBaseTarget.position))
                {
                    atHome = true;
                    moveToNode.ClearPath();
                    currentState = NPCStates.Idle;
                }

                if (!gettingPath && !moveToNode.hasPath)
                    SetDestination(homeBaseTarget.position);

                if (moveToNode.hasPath)
                    moveToNode.Move();

               
                break;

            case NPCStates.Sleeping:

                break;

            case NPCStates.Talking:
                FacePlayer();
                moveToNode.moveSpeed = 0;
                break;
        }

    }

    bool CheckDistanceToDestination(Vector3Int position)
    {
        var dist = Vector3.Distance(transform.position, moveToNode.GetTileWorldPosition(position) + Vector3.forward);
        return dist <= 0.1f;
    }

    void FacePlayer()
    {
        float npcX = transform.position.x;
        float playerX = PlayerInformation.instance.player.position.x;
        if (npcX > playerX && moveToNode.facingRight)
            moveToNode.Flip();
        else if (npcX < playerX && !moveToNode.facingRight)
            moveToNode.Flip();
    }
    void SetDestination(Vector3Int tilePosition)
    {
        //Vector3Int dest = PathRequestManager.GetRandomWalkableNode();
        gettingPath = true;
        PathRequestManager.RequestPath(new PathRequest(moveToNode.currentTilePosition.position, tilePosition, OnPathFound));
    }

    public void OnPathFound(List<Vector3> newPath, bool success)
    {
        if (success)
        {
            moveToNode.PathFound(newPath);
        }
        else
        {
            Debug.Log("Path not found");
        }
        gettingPath = false;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            
            lastState = currentState;
            currentState = NPCStates.Talking;
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
