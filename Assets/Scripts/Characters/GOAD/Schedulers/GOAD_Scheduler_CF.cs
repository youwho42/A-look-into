using Klaxon.GravitySystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Klaxon.GOAD
{
	public class GOAD_Scheduler_CF : GOAD_Scheduler
    {
        public readonly int isGrounded_hash = Animator.StringToHash("IsGrounded");
        public readonly int isRunning_hash = Animator.StringToHash("IsRunning");
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

        CurrentTilePosition currentTilePosition;
        List<FixableObject> allFixables = new List<FixableObject>();
        public override void Start()
        {
            base.Start();
            walker = GetComponent<GravityItemWalk>();
            lastValidTileLocation = transform.position;
            currentTilePosition = GetComponent<CurrentTilePosition>();
            lastValidTile = currentTilePosition.position;
            allFixables = FindObjectsByType<FixableObject>(FindObjectsSortMode.None).ToList();
            
        }



        private void Update()
        {
            if(lastValidTile != currentTilePosition.position)
            {
                lastValidTile = currentTilePosition.position;
                // check fixables for closest and in range
                var closest = GetClosestFixable();
            }
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

        //public void GetRandomTilePosition(int distance, GOAD_Action action)
        //{
        //    var currentPos = _transform.position;
        //    Vector3 destination = currentPos;
        //    destination = GridManager.instance.GetRandomTileWorldPosition(currentPos, distance * .5f);
        //    currentFinalDestination = destination;
        //    SetAStarDestination(destination, action);
        //}

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
            gettingPath = true;
            PathRequestManager.RequestPath(new PathRequest(walker.currentTilePosition.position, gridPos, OnPathFound));
        }

        public void OnPathFound(List<Vector3> newPath, bool success)
        {
            if (success)
                aStarPath = newPath;
            else
                Debug.LogWarning("Path not found", gameObject);

            gettingPath = false;
        }


        FixableObject GetClosestFixable()
        {
            var dist = float.MaxValue;
            FixableObject closest = null;

            foreach (var fixable in allFixables)
            {
                var d = (fixable.transform.position - _transform.position).sqrMagnitude;
                if (d > 3)
                    continue;

                if (d < dist)
                {
                    dist = d;
                    closest = fixable;
                }

            }
            return closest;
        }

    }
}
