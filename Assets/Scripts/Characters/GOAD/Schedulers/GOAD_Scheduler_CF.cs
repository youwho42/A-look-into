using Klaxon.GravitySystem;
using Klaxon.Interactable;
using QuantumTek.QuantumInventory;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Klaxon.GOAD
{
	public class GOAD_Scheduler_CF : GOAD_Scheduler
    {
        public readonly int isGrounded_hash = Animator.StringToHash("IsGrounded");
        public readonly int isRunning_hash = Animator.StringToHash("IsRunning");
        public readonly int breakObject_hash = Animator.StringToHash("BreakObject");
        public readonly int velocityX_hash = Animator.StringToHash("VelocityX");
        public readonly int velocityY_hash = Animator.StringToHash("VelocityY");

        public Animator animator;


        [HideInInspector]
        public bool isDeviating;

        [HideInInspector]
        public bool offScreen;
        [HideInInspector]
        public bool nearPlayer;

        [HideInInspector]
        public GravityItemWalk walker;
        [HideInInspector]
        public Vector3 currentFinalDestination;
        [HideInInspector]
        public Vector3 lastValidTileLocation;
        [HideInInspector]
        public Vector3Int lastValidTile;

        float timeTo;
        [HideInInspector]
        public bool offScreenPosMoved = true;

        public Transform speechBubbleTransform;

        public FlumpOozeManager flumpOozeManager;


        [HideInInspector]
        public int currentPathIndex;
        // A* Pathfinding
        [HideInInspector]
        public List<Vector3> aStarPath = new List<Vector3>();

        [HideInInspector]
        public bool gettingPath;
        [HideInInspector]
        public bool validPath;

        [HideInInspector]
        public CurrentTilePosition currentTilePosition;

        [HideInInspector]
        public List<FixableObject> allFixables = new List<FixableObject>();
        [HideInInspector]
        public FixableObject currentFixable;
        [HideInInspector]
        public InteractableCraftingStation currentCraftingStation;

        public GOAD_ScriptableCondition fleeCondition;
        [HideInInspector]
        public Transform fleeTransform;

        [HideInInspector]
        public CokernutManager manager;
        [HideInInspector]
        public bool isFleeing;

        [HideInInspector]
        public UIScreenManager sleep;


        public override void Start()
        {
            base.Start();
            SetUpCokernutFlump();
        }
        public void SetUpCokernutFlump()
        {
            manager = GetComponentInParent<CokernutManager>();
            walker = GetComponent<GravityItemWalk>();
            sleep = UIScreenManager.instance;
            lastValidTileLocation = transform.position;
            currentTilePosition = GetComponent<CurrentTilePosition>();
            lastValidTile = currentTilePosition.position;
            allFixables = FindObjectsByType<FixableObject>(FindObjectsSortMode.None).ToList();
            for (int i = allFixables.Count - 1; i > 0; i--)
            {
                if (!allFixables[i].cokernutFlumpInteractable)
                    allFixables.RemoveAt(i);
            }
        }



        private void Update()
        {
            if (sleep.isSleeping)
                return;
            if (currentGoalIndex < 0 && availableActions.Count > 0)
            {
                GetCurrentGoal();
                SetNextAction();
            }

            if (currentAction != null)
            {
                if (currentAction.IsRunning)
                    currentAction.PerformAction(this);
                if (currentActionComplete)
                {
                    currentActionComplete = false;
                    currentAction.EndAction(this);
                    if (currentAction.success)
                        SetNextAction();
                    else
                        ResetGoal();

                    return;
                }

            }
            animator.SetBool(isGrounded_hash, walker.isGrounded);
            animator.SetFloat(velocityY_hash, walker.isGrounded ? 0 : walker.displacedPosition.y);
        }

        void LateUpdate()
        {
            if (walker == null)
                return;
            animator.SetBool(isGrounded_hash, walker.isGrounded);
            animator.SetFloat(velocityY_hash, walker.isGrounded ? 0 : walker.displacedPosition.y);

        }

        void SetNextAction()
        {
            if (actionQueue.Count > 0)
            {
                currentAction = actionQueue.Dequeue();
                currentAction.StartAction(this);
            }
            else
                ResetGoal();
            currentActionName = currentAction != null ? currentAction.actionName : "No Current Action";
        }


        public void Deviate()
        {
            isDeviating = true;
            if (walker.isStuck)
                walker.hasDeviatePosition = false;

            if (!walker.hasDeviatePosition)
                walker.FindDeviateDestination(walker.tilemapObstacle ? 20 : 50);


            walker.SetDirection();

            if (walker.CheckDistanceToDestination() <= 0.02f)
                isDeviating = false;

            walker.SetLastPosition();

        }

        
        public bool StartPositionValid()
        {
            if (PathRequestManager.instance.pathfinding.isometricGrid.nodeLookup.TryGetValue(walker.currentTilePosition.position, out IsometricNodeXYZ node))
                return node.walkable;
            return false;
        }

        public void SetAStarDestination(Vector3 destination, GOAD_Action action)
        {

            var start = GridManager.instance.GetTilePosition(_transform.position);
            var end = GridManager.instance.GetTilePosition(destination);
            if (start == end)
            {
                action.AStarDestinationIsCurrentPosition(this);
                return;
            }
            Vector3 destPos = destination;
            destPos.z -= 1;
            Vector3Int gridPos = GridManager.instance.groundMap.WorldToCell(destPos);
            if(!PathRequestManager.instance.pathfinding.isometricGrid.nodeLookup[gridPos].walkable)
            {
                float distance = float.MaxValue;
                Vector3Int best = Vector3Int.zero;
                for (int x = -1; x < 2; x++)
                {
                    for (int y = -1; y < 2; y++)
                    {
                        Vector2Int offset = new Vector2Int(x, y);
                        var d = ((gridPos + (Vector3Int)offset) - currentTilePosition.position).sqrMagnitude;
                        if (d < distance && PathRequestManager.instance.pathfinding.isometricGrid.nodeLookup[gridPos + (Vector3Int)offset].walkable)
                        {
                            distance = d;
                            best = gridPos + (Vector3Int)offset;
                        }
                    }
                }
                gridPos = best;
            }
            gettingPath = true;
            PathRequestManager.RequestPath(new PathRequest(walker.currentTilePosition.position, gridPos, OnPathFound));
        }



        public void OnPathFound(List<Vector3> newPath, bool success)
        {
            if (success)
                aStarPath = newPath;
            else
                Debug.LogWarning("Path not found", gameObject);
            validPath = success;
            gettingPath = false;
        }



        public void HandleOffScreenAStar(GOAD_Action action)
        {
            walker.currentDirection = Vector2.zero;



            if (offScreenPosMoved && currentPathIndex < aStarPath.Count)
            {
                timeTo = Mathf.RoundToInt(Vector2.Distance(_transform.position, aStarPath[currentPathIndex]) / walker.walkSpeed);
                timeTo = (timeTo + RealTimeDayNightCycle.instance.currentTimeRaw) % 1440;

                if (_transform.position.x < aStarPath[currentPathIndex].x && !walker.facingRight)
                    walker.Flip();
                else if (_transform.position.x > aStarPath[currentPathIndex].x && walker.facingRight)
                    walker.Flip();

                offScreenPosMoved = false;
            }


            if (RealTimeDayNightCycle.instance.currentTimeRaw >= timeTo && !offScreenPosMoved)
            {

                offScreenPosMoved = true;
                _transform.position = aStarPath[currentPathIndex];
                walker.currentTilePosition.position = walker.currentTilePosition.GetCurrentTilePosition(_transform.position);
                walker.currentLevel = walker.currentTilePosition.position.z;

                if (currentPathIndex <= aStarPath.Count - 1)
                    currentPathIndex++;

                if (currentPathIndex >= aStarPath.Count)
                {
                    action.success = true;
                    SetActionComplete(true);
                    return;
                }
            }
        }

        public void HandleOffScreenWander(GOAD_Action action, Vector3 wanderDestination)
        {
            walker.currentDirection = Vector2.zero;

            

            if (offScreenPosMoved)
            {
                timeTo = Mathf.RoundToInt(Vector2.Distance(_transform.position, wanderDestination / walker.walkSpeed));
                timeTo = (timeTo + RealTimeDayNightCycle.instance.currentTimeRaw) % 1440;

                if (_transform.position.x < wanderDestination.x && !walker.facingRight)
                    walker.Flip();
                else if (_transform.position.x > wanderDestination.x && walker.facingRight)
                    walker.Flip();

                offScreenPosMoved = false;
            }


            if (RealTimeDayNightCycle.instance.currentTimeRaw >= timeTo && !offScreenPosMoved)
            {
                offScreenPosMoved = true;
                _transform.position = wanderDestination;
                walker.currentTilePosition.position = walker.currentTilePosition.GetCurrentTilePosition(_transform.position);
                walker.currentLevel = walker.currentTilePosition.position.z;
                action.success = Random.value < 0.5f;
                SetActionComplete(true);
            }
        }

        public FixableObject GetClosestFixable()
        {
            var dist = float.MaxValue;
            FixableObject closest = null;

            foreach (var fixable in allFixables)
            {
                if (!fixable.fixedObject.activeInHierarchy || Mathf.Abs(fixable.transform.position.z-transform.position.z) > 1)
                    continue;
                var d = (fixable.transform.position - _transform.position).sqrMagnitude;
                if (d > 10)
                    continue;

                if (d < dist)
                {
                    dist = d;
                    closest = fixable;
                }

            }
            return closest;
        }

        public InteractableCraftingStation GetClosestCraftingStation()
        {
            var dist = float.MaxValue;
            InteractableCraftingStation closest = null;


            var allCraftingStation = FindObjectsByType<InteractableCraftingStation>(FindObjectsSortMode.None).ToList();
            for (int i = allCraftingStation.Count - 1; i > 0; i--)
            {
                if (allCraftingStation[i].GetComponent<QI_Item>() == null)
                    allCraftingStation.RemoveAt(i);
            }


            foreach (var craftingStation in allCraftingStation)
            {
                if (Mathf.Abs(craftingStation.transform.position.z - transform.position.z) > 1)
                    continue;
                var d = (craftingStation.transform.position - _transform.position).sqrMagnitude;
                if (d > 10)
                    continue;

                if (d < dist)
                {
                    dist = d;
                    closest = craftingStation;
                }

            }
            return closest;
        }

        public void FleePlayer(Transform playerTransform)
        {
            if (HasBelief(fleeCondition.Condition, true) && isFleeing)
                return;
            fleeTransform = playerTransform;
            SetBeliefState(fleeCondition.Condition, true);
            if (currentAction != null)
                InteruptCurrentGoal();
        }

        void InteruptCurrentGoal()
        {
            currentAction.success = false;
            SetActionComplete(true);
            currentAction.EndAction(this);
            ResetGoal();
        }

    }
}
