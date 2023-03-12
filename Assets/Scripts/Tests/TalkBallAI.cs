using Klaxon.GravitySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TalkBallAI : MonoBehaviour
{
    
    public enum TalkBallState
    {
        Appear,
        Follow,
        Idle,
        Disappear
    }

    public TalkBallState currentState;

    List<Material> materials = new List<Material>();
    float timeIdle = 0;

    static int walking_hash = Animator.StringToHash("IsWalking");
    public Animator animator;
    MoveToNode moveToNode;
    bool gettingPath;

    private void Start()
    {

        moveToNode = GetComponent<MoveToNode>();
        var sprites = GetComponentsInChildren<SpriteRenderer>();
        foreach (var sprite in sprites)
        {
            materials.Add(sprite.material);
            sprite.material.SetFloat("_Fade", 0);
        }
    }

    private void OnEnable()
    {
        
        currentState = TalkBallState.Appear;
    }

    private void Update()
    {

        switch (currentState)
        {
            case TalkBallState.Appear:
                
                Disolve(true);
                break;

            case TalkBallState.Follow:
                // follow player, if too close go to idle
                animator.SetBool(walking_hash, true);
                //if (!gettingPath && !moveToNode.hasPath)
                //    SetDestination(PathRequestManager.GetRandomDistancedTile(moveToNode.currentTilePosition.position, wanderDistance));

                //if (moveToNode.hasPath)
                //    moveToNode.Move();
                timeIdle = 0;
                if (CheckPlayerDistance() <= 0.2f)
                    currentState = TalkBallState.Idle;

                break;

            case TalkBallState.Idle:
                //check distance from player, wait a sec, and start to follow if too far
                animator.SetBool(walking_hash, false);
                if (timeIdle < 2)
                {
                    timeIdle += Time.deltaTime;
                }
                else
                {
                    if (CheckPlayerDistance() >= 0.6f)
                        currentState = TalkBallState.Follow;
                }
                break;

            case TalkBallState.Disappear:
                Disolve(false);
                break;
            
        }

    }
    

    void Disolve(bool disolveIn)
    {
        for (int i = 0; i < materials.Count; i++)
        {
            DissolveEffect.instance.StartDissolve(materials[i], 1f, disolveIn);
        }
        if(disolveIn)
            currentState = TalkBallState.Idle;
        
    }

    float CheckPlayerDistance()
    {
        float dist = Vector2.Distance(transform.position, PlayerInformation.instance.player.position);
        
        return dist;
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
            Debug.Log("Cannot reach player");
        }
        gettingPath = false;
    }

}
