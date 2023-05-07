using Klaxon.GravitySystem;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BallPeopleSeekerAI;

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
    GravityItemWalker walker;
    [HideInInspector]
    public Vector3 currentPlantDestination;

    
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
        walker = GetComponent<GravityItemWalker>();
        allSprites = GetComponentsInChildren<SpriteRenderer>().ToList();
        foreach (var sprite in allSprites)
        {
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

                walker.currentDir = Vector2.zero;
                Disolve(true);
                walker.ResetLastPosition();
                break;

            case PlanterState.Idle:
                walker.currentDir = Vector2.zero;
                hasLicked = false;
                var dir = PlayerInformation.instance.player.position - transform.position;
                walker.SetFacingDirection(dir);
                
                //check distance from player, wait a sec, and start to follow if too far
                offset = new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
                animator.SetBool(walking_hash, false);

                timeIdle += Time.deltaTime;

                if (timeIdle > .5f)
                {
                    if (plantingArea.canPlant)
                    {
                        walker.currentDestination = GetSeedBoxPosition();
                        currentState = PlanterState.GoToBox;
                    }
                        
                }
                if (timeIdle >= 10.0f)
                {
                    timeIdle = 0;
                    currentState = PlanterState.Remove;
                }
                walker.ResetLastPosition();
                break;

            
            case PlanterState.GoToBox:
                lastState = PlanterState.GoToBox;
                if (walker.isStuck)
                {
                    if (!walker.jumpAhead)
                        currentState = PlanterState.Deviate;
                }

                animator.SetBool(walking_hash, true);
                walker.currentDestination = GetSeedBoxPosition();
                walker.SetWorldDestination(walker.currentDestination);
                walker.SetDirection();
                if (walker.CheckDistanceToDestination() <= 0.05f)
                {
                    currentState = PlanterState.AtBox;
                }
                walker.SetLastPosition();

                break;

            case PlanterState.AtBox:
                walker.currentDir = Vector2.zero;
                animator.SetBool(walking_hash, false);
                if (!hasLicked)
                {
                    timeIdle = 0;
                    animator.SetTrigger(lick_hash);
                    hasLicked = true;

                    seedBoxInventory.RemoveItem(plantingArea.seedItem, 1);
                    if (plantingArea.plantFreeLocations.Count > 0)
                    {
                        currentPlantDestination = plantingArea.plantFreeLocations[0];
                        walker.currentDestination = currentPlantDestination;
                        plantingArea.plantFreeLocations.RemoveAt(0);
                        plantingArea.CheckForPlantable();
                    }
                        
                    
                }
                if (timeIdle < 0.7f)
                    timeIdle += Time.deltaTime;
                else
                    currentState = PlanterState.GoToPlantLocation;
                walker.ResetLastPosition();
                break;

            case PlanterState.GoToPlantLocation:
                hasLicked = false;
                lastState = PlanterState.GoToPlantLocation;
                if (walker.isStuck)
                {
                    if (!walker.jumpAhead)
                        currentState = PlanterState.Deviate;
                }
                animator.SetBool(walking_hash, true);
                walker.SetWorldDestination(currentPlantDestination);
                walker.SetDirection();
                if (walker.CheckDistanceToDestination() <= 0.06f)
                {
                    currentState = PlanterState.AtPlantLocation;
                }

                walker.SetLastPosition();

                break;

            case PlanterState.AtPlantLocation:
                walker.currentDir = Vector2.zero;
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
                walker.ResetLastPosition();
                break;


            case PlanterState.Deviate:
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
                    currentState = lastState;

                walker.SetLastPosition();

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
        if (walker == null)
            return;
        animator.SetBool(isGrounded_hash, walker.isGrounded);
        animator.SetFloat(velocityY_hash, walker.isGrounded ? 0 : walker.displacedPosition.y);

    }


    Vector3 GetSeedBoxPosition()
    {
        Vector3 pos = seedBoxInventory.transform.position;
        Vector2 dir = transform.position - pos;
        dir = dir.normalized;
        dir *= 0.1f;
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
        plantingArea.plantUsedLocations.Add(currentPlantDestination);
        var go = Instantiate(plantingArea.seedItem.plantedObject.ItemPrefab, currentPlantDestination, Quaternion.identity);
        go.GetComponent<InteractableFarmPlant>().plantingArea = plantingArea;
        go.GetComponent<SaveableItemEntity>().GenerateId();
        PlantLife plant = go.GetComponent<PlantLife>();
        plant.SetSprites();
        plant.SetNextCycleTime();
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
    

    public void SetToRemoveState()
    {
        currentState = PlanterState.Remove;
    }
}
