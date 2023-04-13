using Klaxon.GravitySystem;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallPeopleFarmHarvesterAI : MonoBehaviour, IBallPerson
{

    public enum HarvesterState
    {
        Appear,
        Idle,
        GoToPlantLocation,
        AtPlantLocation,
        GoToBox,
        AtBox,
        Deviate,
        Remove
    }

    public HarvesterState currentState;
    public HarvesterState lastState;

    List<SpriteRenderer> allSprites = new List<SpriteRenderer>();
    float timeIdle = 0;

    static int walking_hash = Animator.StringToHash("IsWalking");
    static int lick_hash = Animator.StringToHash("Lick");
    static int isGrounded_hash = Animator.StringToHash("IsGrounded");
    static int velocityY_hash = Animator.StringToHash("VelocityY");
    static int sleeping_hash = Animator.StringToHash("IsSleeping");

    public Animator animator;
    CanReachTileWalk walk;
    CurrentTilePosition currentTilePosition;

    bool hasDeviatePosition;


    [HideInInspector]
    public Vector3 currentDestination;
    [HideInInspector]
    public Vector3 deviateDestination;
    Vector3 lastPosition;


    bool disolved;
    Vector2 offset;


    bool hasLicked;
    [HideInInspector]
    public QI_Inventory seedBoxInventory;

    [HideInInspector]
    public PlantingArea plantingArea;

    public LayerMask obstacleLayer;
    public GameObject arms;

    public PlantLife currentHarvestable;
    private void Start()
    {
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
        currentState = HarvesterState.Appear;
        arms.SetActive(false);
    }

    private void Update()
    {

        switch (currentState)
        {
            case HarvesterState.Appear:

                Disolve(true);
                break;

            case HarvesterState.Idle:
                hasLicked = false;
                var dir = PlayerInformation.instance.player.position - transform.position;
                walk.SetFacingDirection(dir);

                //check distance from player, wait a sec, and start to follow if too far
                offset = new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
                animator.SetBool(walking_hash, false);

                timeIdle += Time.deltaTime;

                if (timeIdle > .5f)
                {
                    if (plantingArea.canHarvest)
                    {

                        currentDestination = GetPlantPosition();
                        currentState = HarvesterState.GoToPlantLocation;
                        break;
                    }

                }
                if (timeIdle >= 10.0f)
                {
                    timeIdle = 0;
                    currentState = HarvesterState.Remove;
                }
                break;


            

            case HarvesterState.GoToPlantLocation:
                hasLicked = false;
                lastState = HarvesterState.GoToPlantLocation;
                if (lastPosition != transform.position)
                {
                    lastPosition = transform.position;
                }
                else
                {
                    if (!walk.jumpAhead)
                    {
                        currentState = HarvesterState.Deviate;
                        break;
                    }
                }
                animator.SetBool(walking_hash, true);
                walk.SetWorldDestination(currentDestination);
                walk.Walk();
                if (CheckDistanceToDestination(currentDestination) <= 0.06f)
                {
                    currentState = HarvesterState.AtPlantLocation;
                }



                break;

            case HarvesterState.AtPlantLocation:

                animator.SetBool(walking_hash, false);
                if (!hasLicked)
                {
                    timeIdle = 0;
                    animator.SetTrigger(lick_hash);
                    hasLicked = true;
                    if(!plantingArea.plantUsedLocations.Contains(currentDestination))
                    {
                        currentState = HarvesterState.Idle;
                        break;
                    }
                    plantingArea.plantFreeLocations.Add(currentDestination);
                    plantingArea.plantUsedLocations.Remove(currentDestination);
                    Destroy(currentHarvestable.gameObject);
                    
                }
                if (timeIdle < 0.7f)
                    timeIdle += Time.deltaTime;
                else
                {
                    currentDestination = GetSeedBoxPosition();
                    currentState = HarvesterState.GoToBox;
                }
                    

                break;

            case HarvesterState.GoToBox:
                hasLicked = false;
                lastState = HarvesterState.GoToBox;
                if (lastPosition != transform.position)
                {
                    lastPosition = transform.position;
                }
                else
                {
                    if (!walk.jumpAhead)
                    {
                        currentState = HarvesterState.Deviate;
                        break;
                    }

                }
                animator.SetBool(walking_hash, true);
                //currentDestination = travellerDestination + (Vector3)offset;
                walk.SetWorldDestination(currentDestination);
                walk.Walk();
                if (CheckDistanceToDestination(currentDestination) <= 0.01f)
                {

                    currentState = HarvesterState.AtBox;
                }


                break;

            case HarvesterState.AtBox:

                animator.SetBool(walking_hash, false);
                if (!hasLicked)
                {
                    timeIdle = 0;
                    animator.SetTrigger(lick_hash);
                    hasLicked = true;

                    //ADD ITEMS

                    foreach (var item in plantingArea.seedItem.plantedObject.harvestedItems)
                    {
                        seedBoxInventory.AddItem(item.harvestedItem, item.harvestedAmount, false);
                    }
                    plantingArea.CheckForHarvestable();
                    

                }
                if (timeIdle < 0.7f)
                    timeIdle += Time.deltaTime;
                else
                    currentState = HarvesterState.Idle;

                break;

            case HarvesterState.Deviate:
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

                walk.SetWorldDestination(deviateDestination);

                if (CheckDistanceToDestination(deviateDestination) <= 0.02f)
                    currentState = lastState;

                walk.Walk();

                break;

            case HarvesterState.Remove:
                if (!disolved)
                {
                    timeIdle = 0;
                    Disolve(false);
                }


                if (timeIdle < 2f)
                {
                    timeIdle += Time.deltaTime;
                }
                else
                {
                    plantingArea.ballPersonHarvesterActive = false;
                    Destroy(gameObject);
                }

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

    Vector3 GetPlantPosition()
    {
        
        var pos = plantingArea.harvestablePlants[0].transform.position;
        currentHarvestable = plantingArea.harvestablePlants[0];
        plantingArea.harvestablePlants.RemoveAt(0);
        return pos;
    }

    Vector3 GetSeedBoxPosition()
    {
        Vector3 pos = seedBoxInventory.transform.position;
        Vector2 dir = transform.position - pos;
        dir = dir.normalized;
        dir *= 0.09f;
        var colliders = seedBoxInventory.GetComponentsInChildren<Collider2D>();
        foreach (var coll in colliders)
        {
            if (gameObject.layer == obstacleLayer)
            {
                pos = coll.ClosestPoint(transform.position);
                break;
            }
        }

        return pos + (Vector3)dir;
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
            deviateDestination = transform.position + (Vector3)offset * .2f;
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
            currentState = HarvesterState.Idle;
        disolved = !disolveIn;
    }
    float CheckDistanceToDestination(Vector3 destination)
    {
        float dist = Vector2.Distance(transform.position, destination);

        return dist;
    }

    public void SetToRemoveState()
    {
        currentState = HarvesterState.Remove;
    }
}