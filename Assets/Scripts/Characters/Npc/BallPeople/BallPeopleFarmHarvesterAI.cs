using Klaxon.GravitySystem;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BallPeopleFarmPlanterAI;

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

    public PlantLife currentHarvestable;

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
        currentState = HarvesterState.Appear;
        arms.SetActive(false);
    }

    private void Update()
    {

        switch (currentState)
        {
            case HarvesterState.Appear:

                walker.currentDir = Vector2.zero;
                Disolve(true);
                walker.ResetLastPosition();
                break;

            case HarvesterState.Idle:
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
                    if (plantingArea.canHarvest)
                    {
                        currentPlantDestination = GetPlantPosition();
                        walker.currentDestination = currentPlantDestination;
                        currentState = HarvesterState.GoToPlantLocation;
                        break;
                    }

                }
                if (timeIdle >= 10.0f)
                {
                    timeIdle = 0;
                    currentState = HarvesterState.Remove;
                }
                walker.ResetLastPosition();
                break;


            

            case HarvesterState.GoToPlantLocation:
                hasLicked = false;
                lastState = HarvesterState.GoToPlantLocation;
                if (walker.isStuck)
                {
                    if (!walker.jumpAhead)
                        currentState = HarvesterState.Deviate;
                }
                animator.SetBool(walking_hash, true);
                walker.SetWorldDestination(currentPlantDestination);
                walker.SetDirection();
                if (walker.CheckDistanceToDestination() <= 0.06f)
                {
                    currentState = HarvesterState.AtPlantLocation;
                }

                walker.SetLastPosition();

                break;

            case HarvesterState.AtPlantLocation:
                walker.currentDir = Vector2.zero;
                animator.SetBool(walking_hash, false);
                if (!hasLicked)
                {
                    timeIdle = 0;
                    animator.SetTrigger(lick_hash);
                    hasLicked = true;
                    if(!plantingArea.plantUsedLocations.Contains(currentPlantDestination))
                    {
                        currentState = HarvesterState.Idle;
                        break;
                    }
                    plantingArea.plantFreeLocations.Add(currentPlantDestination);
                    plantingArea.plantUsedLocations.Remove(currentPlantDestination);
                    plantingArea.harvestablePlants.Remove(currentHarvestable);
                    Destroy(currentHarvestable.gameObject);
                    
                }
                if (timeIdle < 0.7f)
                    timeIdle += Time.deltaTime;
                else
                {
                    walker.currentDestination = GetSeedBoxPosition();
                    currentState = HarvesterState.GoToBox;
                }
                walker.ResetLastPosition();

                break;

            case HarvesterState.GoToBox:
                lastState = HarvesterState.GoToBox;
                if (walker.isStuck)
                {
                    if (!walker.jumpAhead)
                        currentState = HarvesterState.Deviate;
                }

                animator.SetBool(walking_hash, true);
                walker.currentDestination = GetSeedBoxPosition();
                walker.SetWorldDestination(walker.currentDestination);
                walker.SetDirection();
                if (walker.CheckDistanceToDestination() <= 0.05f)
                {
                    currentState = HarvesterState.AtBox;
                }
                walker.SetLastPosition();


                break;

            case HarvesterState.AtBox:
                walker.currentDir = Vector2.zero;
                animator.SetBool(walking_hash, false);
                if (!hasLicked)
                {
                    timeIdle = 0;
                    animator.SetTrigger(lick_hash);
                    hasLicked = true;

                    //ADD ITEMS

                    foreach (var item in plantingArea.seedItem.plantedObject.harvestedItems)
                    {
                        var amount = item.minMaxAmount.x == 0 ? 1 : item.minMaxAmount.x;
                        seedBoxInventory.AddItem(item.harvestedItem, amount, false);
                    }
                    plantingArea.CheckForHarvestable();
                    

                }
                if (timeIdle < 0.7f)
                    timeIdle += Time.deltaTime;
                else
                    currentState = HarvesterState.Idle;
                walker.ResetLastPosition();
                break;

            case HarvesterState.Deviate:
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

            case HarvesterState.Remove:
                walker.currentDir = Vector2.zero;
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
        if (walker == null)
            return;
        animator.SetBool(isGrounded_hash, walker.isGrounded);
        animator.SetFloat(velocityY_hash, walker.isGrounded ? 0 : walker.displacedPosition.y);

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
    

    public void SetToRemoveState()
    {
        currentState = HarvesterState.Remove;
    }
}