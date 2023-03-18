using Klaxon.GravitySystem;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.PeerToPeer.Collaboration;
using UnityEngine;
using UnityEngine.Rendering;

public class MessengerAI : MonoBehaviour
{
    
    public enum MessengerState
    {
        Appear,
        Follow,
        Idle,
        Deviate,
        Disappear,
        Remove
    }

    public MessengerState currentState;

    List<Material> materials = new List<Material>();
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

    
    static int isGrounded_hash = Animator.StringToHash("IsGrounded");
    static int velocityY_hash = Animator.StringToHash("VelocityY");

    
    

    private void Start()
    {
        
        currentTilePosition = GetComponent<CurrentTilePosition>();
        walk = GetComponent<CanReachTileWalk>();
        var sprites = GetComponentsInChildren<SpriteRenderer>();
        foreach (var sprite in sprites)
        {
            materials.Add(sprite.material);
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
                if (CheckDistanceToDestinaion() <= 0.02f)
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

                if (CheckDistanceToDestinaion() <= 0.02f)
                    currentState = MessengerState.Follow;

                walk.Walk();

                break;



            case MessengerState.Idle:
                //check distance from player, wait a sec, and start to follow if too far
                offset = new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
                animator.SetBool(walking_hash, false);
                if (CheckPlayerDistance() > 3)
                {
                    timeIdle = 0;
                    currentState = MessengerState.Disappear;
                }
                
                if (timeIdle < 0.5f)
                {
                    timeIdle += Time.deltaTime;
                }
                else
                {
                    if (CheckPlayerDistance() >= 0.5f)
                    {
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
        for (int i = 0; i < materials.Count; i++)
        {
            DissolveEffect.instance.StartDissolve(materials[i], 1f, disolveIn);
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

    float CheckDistanceToDestinaion()
    {
        float dist = Vector2.Distance(transform.position, currentDestination);

        return dist;
    }

}
