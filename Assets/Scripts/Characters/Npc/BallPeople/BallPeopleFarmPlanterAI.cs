using Klaxon.GravitySystem;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallPeopleFarmPlanterAI : MonoBehaviour, IBallPerson
{

    public enum PlanterState
    {
        Appear,
        Idle,
        GoToBox,
        AtBox,
        GoToPlantLocation,
        AtPlantLocation,
        Deviate,
        Remove
    }

    public PlanterState currentState;
    public PlanterState lastState;

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
        currentState = PlanterState.Appear;
        arms.SetActive(false);
    }

    private void Update()
    {

        switch (currentState)
        {
            case PlanterState.Appear:

                Disolve(true);
                break;

            case PlanterState.Idle:
                hasLicked = false;
                var dir = PlayerInformation.instance.player.position - transform.position;
                walk.SetFacingDirection(dir);
                
                //check distance from player, wait a sec, and start to follow if too far
                offset = new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
                animator.SetBool(walking_hash, false);

                timeIdle += Time.deltaTime;

                if (timeIdle > .5f)
                {
                    if (plantingArea.canPlant)
                    {
                        currentDestination = GetSeedBoxPosition();
                        currentState = PlanterState.GoToBox;
                    }
                        
                }
                if (timeIdle >= 10.0f)
                {
                    timeIdle = 0;
                    currentState = PlanterState.Remove;
                }
                break;

            
            case PlanterState.GoToBox:
                lastState = PlanterState.GoToBox;
                if (lastPosition != transform.position)
                {
                    lastPosition = transform.position;
                }
                else
                {
                    if (!walk.jumpAhead)
                    {
                        currentState = PlanterState.Deviate;
                        break;
                    }
                        
                }
                animator.SetBool(walking_hash, true);
                //currentDestination = travellerDestination + (Vector3)offset;
                walk.SetWorldDestination(currentDestination);
                walk.Walk();
                if (CheckDistanceToDestination(currentDestination) <= 0.01f)
                {
                    
                    currentState = PlanterState.AtBox;
                }


                break;

            case PlanterState.AtBox:

                animator.SetBool(walking_hash, false);
                if (!hasLicked)
                {
                    timeIdle = 0;
                    animator.SetTrigger(lick_hash);
                    hasLicked = true;

                    seedBoxInventory.RemoveItem(plantingArea.seedItem, 1);
                    if (plantingArea.plantFreeLocations.Count > 0)
                    {
                        currentDestination = plantingArea.plantFreeLocations[0];
                        plantingArea.plantFreeLocations.RemoveAt(0);
                        plantingArea.CheckForPlantable();
                    }
                        
                    
                }
                if (timeIdle < 0.7f)
                    timeIdle += Time.deltaTime;
                else
                    currentState = PlanterState.GoToPlantLocation;

                break;

            case PlanterState.GoToPlantLocation:
                hasLicked = false;
                lastState = PlanterState.GoToPlantLocation;
                if (lastPosition != transform.position)
                {
                    lastPosition = transform.position;
                }
                else
                {
                    if (!walk.jumpAhead)
                    {
                        currentState = PlanterState.Deviate;
                        break;
                    }
                }
                animator.SetBool(walking_hash, true);
                walk.SetWorldDestination(currentDestination);
                walk.Walk();
                if (CheckDistanceToDestination(currentDestination) <= 0.06f)
                {

                    currentState = PlanterState.AtPlantLocation;
                }

                

                break;

            case PlanterState.AtPlantLocation:

                animator.SetBool(walking_hash, false);
                if (!hasLicked)
                {
                    timeIdle = 0;
                    animator.SetTrigger(lick_hash);
                    hasLicked = true;

                    PlantSeed();
                    // Add plant at location
                }
                if (timeIdle < 0.7f)
                    timeIdle += Time.deltaTime;
                else
                    currentState = PlanterState.Idle;

                break;


            case PlanterState.Deviate:
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

            case PlanterState.Remove:
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
                    plantingArea.ballPersonPlanterActive = false;
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

    void PlantSeed()
    {
        plantingArea.plantUsedLocations.Add(currentDestination);
        var go = Instantiate(plantingArea.seedItem.plantedObject.ItemPrefab, currentDestination, Quaternion.identity);
        go.GetComponent<InteractableFarmPlant>().plantingArea = plantingArea;
        go.GetComponent<SaveableItemEntity>().GenerateId();
        PlantLife plant = go.GetComponent<PlantLife>();
        plant.SetSprites();
        plant.SetNextCycleTime();
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
            currentState = PlanterState.Idle;
        disolved = !disolveIn;
    }
    float CheckDistanceToDestination(Vector3 destination)
    {
        float dist = Vector2.Distance(transform.position, destination);

        return dist;
    }

    public void SetToRemoveState()
    {
        currentState = PlanterState.Remove;
    }
}
