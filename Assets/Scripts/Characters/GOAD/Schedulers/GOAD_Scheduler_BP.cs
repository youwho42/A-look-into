using Klaxon.GravitySystem;
using Klaxon.Interactable;
using Klaxon.UndertakingSystem;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Klaxon.GOAD
{
	public class GOAD_Scheduler_BP : GOAD_Scheduler, IBallPerson
    {

        public readonly int walking_hash = Animator.StringToHash("IsWalking");
        public readonly int isGrounded_hash = Animator.StringToHash("IsGrounded");
        public readonly int velocityY_hash = Animator.StringToHash("VelocityY");
        public readonly int sleeping_hash = Animator.StringToHash("IsSleeping");
        public readonly int lick_hash = Animator.StringToHash("Lick");
        public Animator animator;
        List<SpriteRenderer> allSprites = new List<SpriteRenderer>();

        [HideInInspector]
        public bool isDeviating;

        [HideInInspector]
        public bool offScreen;

        [HideInInspector]
        public GravityItemWalk walker;

        float timeTo;
        [HideInInspector]
        public bool offScreenPosMoved = true;
        [HideInInspector]
        public UIScreenManager sleep;


        [HideInInspector]
        public Klaxon.Interactable.Interactable interactor;
        [HideInInspector]
        public bool hasInteracted;
        public GameObject arms;
        public GOAD_ScriptableCondition questComplteCondition;
        public GOAD_ScriptableCondition fireInteractCondition;
        [HideInInspector]
        public Campfire currentFire;

        bool hasMovedOffScreen;
        float offScreenMoveTime;

        public enum BP_Type
        {
            Messenger,
            Seeker,
            Traveller,
            TravellerHome,
            Farmer,
            Indicator,
            Villager,
            Locationer
        }
        public BP_Type type;

        [HideInInspector]
        public Vector3 BPHomeDestination;
        public Transform speechBubbleTransform;
        PlayerInformation player;
        /// <summary>
        /// Traveller BP
        /// </summary>
        #region Traveller BP
        [HideInInspector]
        public bool hasFoundDestination;
        [HideInInspector]
        public bool justIndicatedTravellerDestination;
        public GOAD_ScriptableCondition travellerAreaFoundCondition;

        #endregion


        /// <summary>
        /// Seeker BP
        /// </summary> 
        #region Seeker BP
        [HideInInspector]
        public bool isSeeking;
        [HideInInspector]
        public QI_ItemData seekItem;
        [HideInInspector]
        public int seekAmount;
        [HideInInspector]
        public GameObject currentSeekItem;
        [HideInInspector]
        public Vector3 currentSeekItemLocation;
        [HideInInspector]
        public int foundAmount;
        [HideInInspector]
        public List<Vector3> seekItemsFound = new List<Vector3>();
        public float seekRadius;
        [HideInInspector]
        public CompleteTaskObject task;
        public GOAD_ScriptableCondition seekCondition;

        #endregion

        /// <summary>
        /// Indicator BP
        /// </summary>
        #region IndicatorAndLocationerBP

        [Header("A* Pathfinding")]
        // A* Pathfinding
        [HideInInspector]
        public List<Vector3> aStarPath = new List<Vector3>();
        [HideInInspector]
        public Vector3 destination;
        [HideInInspector]
        public bool gettingPath;
        [HideInInspector]
        public Vector3 currentFinalDestination;
        [HideInInspector]
        public Vector3 lastValidTileLocation;
        [HideInInspector]
        public int currentFailedPathfindingAttempts;
        [HideInInspector]
        public int currentPathIndex;

        [HideInInspector]
        public int indicatorIndex;
        [HideInInspector]
        public bool justIndicated;
        public Transform locationerLocation;
        [HideInInspector]
        public CompleteTaskObject locationFoundTask;
        [HideInInspector]
        public Vector3 validStartPosition;
        
        #endregion

        /// <summary>
        /// Planter BP
        /// </summary>
        #region Planter BP
        [HideInInspector]
        public QI_Inventory seedBoxInventory;
        [HideInInspector]
        public PlantingArea plantingArea;
        [HideInInspector]
        public Vector3 currentPlantDestination;
        [HideInInspector]
        public PlantLife currentHarvestable; 
        #endregion


        public override void Start()
        {
            base.Start();
            player = PlayerInformation.instance;
            sleep = UIScreenManager.instance;
            walker = GetComponent<GravityItemWalk>();
            walker.currentDirection = Vector2.zero;
            allSprites = GetComponentsInChildren<SpriteRenderer>().ToList();
            foreach (var sprite in allSprites)
            {
                sprite.material.SetFloat("_Fade", 0);
            }
            Disolve(true);
            walker.ResetLastPosition();
            interactor = GetComponent<Interactable.Interactable>();
            if (type == BP_Type.Indicator)
                GameEventManager.onMapClearMarkersEvent.AddListener(ClearIndicator);
        }

        private void OnDisable()
        {
            if (type == BP_Type.Indicator)
                GameEventManager.onMapClearMarkersEvent.RemoveListener(ClearIndicator);
        }

        void ClearIndicator()
        {
            SetBeliefState(questComplteCondition.Condition, questComplteCondition.State);
            currentAction.success = true;
            SetActionComplete(true);
        }


        private void Update()
        {

            
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
            if (type == BP_Type.Seeker && seekItem != null && !isSeeking)
            {
                SeekItem();
            }

            if (type == BP_Type.Traveller && !justIndicatedTravellerDestination && !hasFoundDestination/* && !GetBeliefState("QuestOver")*/)
            {
                SeekTravellerDestination();
            }
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


        public void Disolve(bool disolveIn)
        {
            for (int i = 0; i < allSprites.Count; i++)
            {
                if (allSprites[i].gameObject.activeSelf)
                    DissolveEffect.instance.StartDissolve(allSprites[i].material, 1.5f, disolveIn);
            }
            
        }


        public bool CheckNearPlayer(float maxDistance)
        {
            float dist = (_transform.position - player.player.position).sqrMagnitude;

            return dist <= maxDistance * maxDistance;
        }

        public bool StartPositionValid()
        {
            if (PathRequestManager.instance.pathfinding.isometricGrid.nodeLookup.TryGetValue(walker.currentTilePosition.position, out IsometricNodeXYZ node))
                return node.walkable; 
            return false;

        }

        public void SetValidStartPosition(Vector3 destination, GOAD_Action action)
        {
            float dist = float.MaxValue;
            for (int x = -2; x < 3; x++)
            {
                for (int y = -2; y < 3; y++)
                {
                    Vector3Int offset = new Vector3Int(x, y, 0);
                    var potentialTile = walker.currentTilePosition.position + offset;
                    if(PathRequestManager.instance.pathfinding.isometricGrid.nodeLookup.ContainsKey(potentialTile))
                    {
                        if (PathRequestManager.instance.pathfinding.isometricGrid.nodeLookup[potentialTile].walkable)
                        {
                            var pos = GridManager.instance.GetTileWorldPosition(potentialTile);
                            float d = (pos - _transform.position).sqrMagnitude;
                            if (d < dist)
                            {
                                dist = d;
                                validStartPosition = pos;
                            }
                        }
                    }
                    
                }
            }
            SetAStarDestination(destination, action, validStartPosition);

        }

        public void SetAStarDestination(Vector3 destination, GOAD_Action action, Vector3 startPos)
        {
            
            var start = GridManager.instance.GetTilePosition(startPos);
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
            PathRequestManager.RequestPath(new PathRequest(start, gridPos, OnPathFound));
        }

        public void OnPathFound(List<Vector3> newPath, bool success)
        {
            if (success)
            {
                currentFailedPathfindingAttempts = 0;
                aStarPath = newPath;
            }
            else
            {
                currentFailedPathfindingAttempts++;
                Debug.LogWarning("Path not found", gameObject);
            }


            gettingPath = false;
        }

        //public void AStarDeviate(GOAD_Action action)
        //{
        //    isDeviating = true;
        //    //if (walker.isStuck)
        //    //{
        //    //    if (UnstuckCharacter())
        //    //        return;
        //    //}



        //    if (walker.isStuck)
        //        walker.hasDeviatePosition = false;

        //    if (!walker.hasDeviatePosition)
        //        walker.FindDeviateDestination(walker.tilemapObstacle ? 20 : 50);

        //    //animator.SetFloat(velocityX_hash, 1);
        //    walker.SetDirection();

        //    if (walker.CheckDistanceToDestination() <= 0.02f)
        //    {
        //        isDeviating = false;
        //        currentPathIndex = 0;
        //        aStarPath.Clear();
        //        if (StartPositionValid())
        //            SetAStarDestination(currentFinalDestination, action);
        //        else
        //        {
        //            Debug.LogWarning($"start position not valid after deviate \ncurrent pos{transform.position} last pos{lastValidTileLocation}", gameObject);
        //            transform.position = lastValidTileLocation;
        //            //aStarPath.Add(lastValidTileLocation);
        //        }

        //    }


        //    walker.SetLastPosition();
        //}


        public void Deviate()
        {
            isDeviating = true;
            if (walker.isStuck)
            {
                var hit = Physics2D.OverlapPoint(transform.position, LayerMask.GetMask("Obstacle"), transform.position.z, transform.position.z);
                if (hit != null)
                {
                    if (hit.TryGetComponent(out DrawZasYDisplacement disp))
                    {
                        if (disp.positionZ > 0)
                        {
                            for (int d = 1; d < 6; d++)
                            {
                                for (float i = 0; i < Mathf.PI * 2; i += Mathf.PI * 0.25f)
                                {
                                    Vector2 dir = new Vector2(Mathf.Cos(i), Mathf.Sin(i));
                                    dir = dir.normalized;
                                    dir *= d * 0.05f;
                                    var posI = transform.position + (Vector3)dir;
                                    var h = Physics2D.OverlapPoint(posI, LayerMask.GetMask("Obstacle"), posI.z, posI.z);
                                    if (!h && GridManager.instance.GetTileValid(posI))
                                    {
                                        transform.position = posI;
                                        walker.ResetLastPosition();
                                        isDeviating = false;
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            

            
            if (walker.isStuck)
                walker.hasDeviatePosition = false;

            if (!walker.hasDeviatePosition)
                walker.FindDeviateDestination(walker.tilemapObstacle ? 20 : 50);


            walker.SetDirection();

            if (walker.CheckDistanceToDestination() <= 0.02f)
                isDeviating = false;

            walker.SetLastPosition();
        }



        public void HandleOffScreen(GOAD_Action action, Vector3 currentDestination)
        {
            walker.currentDirection = Vector2.zero;

            if (hasMovedOffScreen)
            {
                offScreenMoveTime = Mathf.RoundToInt(Vector2.Distance(_transform.position, currentDestination) / walker.walkSpeed);
                offScreenMoveTime = (offScreenMoveTime + RealTimeDayNightCycle.instance.currentTimeRaw) % 1440;

                if (_transform.position.x < currentDestination.x && !walker.facingRight)
                    walker.Flip();
                else if (_transform.position.x > currentDestination.x && walker.facingRight)
                    walker.Flip();

                hasMovedOffScreen = false;
            }

            if (RealTimeDayNightCycle.instance.currentTimeRaw >= offScreenMoveTime && !hasMovedOffScreen)
            {

                hasMovedOffScreen = true;
                _transform.position = currentDestination;
                walker.currentTilePosition.position = walker.currentTilePosition.GetCurrentTilePosition(_transform.position);
                walker.currentLevel = walker.currentTilePosition.position.z;

                action.ReachFinalDestination(this);

                walker.SetLastPosition();
            }
        }

        public void SetToRemoveState()
        {
            SetBeliefState(questComplteCondition.Condition, questComplteCondition.State);
            currentAction.success = true;
            SetActionComplete(true);
        }
        public void InteractionFinished()
        {
            hasInteracted = true;
        }

        public void InvokeResetJustIndicatedTravellerDestination()
        {
            Invoke("ResetJustIndicatedTravellerDestination", 5.0f);
        }
        void ResetJustIndicatedTravellerDestination()
        {
            justIndicatedTravellerDestination = false;
        }
        void SeekTravellerDestination()
        {
            
            if (hasInteracted && !hasFoundDestination)
            {
                var dist = NumberFunctions.GetDistanceV3(_transform.position, BPHomeDestination);
                if (dist < 3.24f)
                {
                    hasFoundDestination = true;
                    currentAction.success = true;
                    SetActionComplete(true);
                    SetBeliefState(travellerAreaFoundCondition.Condition, travellerAreaFoundCondition.State);
                }
            }
        }

        public void SetFire(Campfire fire)
        {
            currentFire = fire;
            SetBeliefState(fireInteractCondition.Condition, fireInteractCondition.State);
            currentAction.success = true;
            SetActionComplete(true);

        }

        void SeekItem()
        {
            if (hasInteracted && !task.task.IsComplete)
            {
                currentSeekItem = CheckForSeekItem();
                if (currentSeekItem != null)
                {
                    currentSeekItemLocation = GetSeekItemPosition();
                    isSeeking = true;
                    SetBeliefState(seekCondition.Condition, true);
                    currentAction.success = true;
                    SetActionComplete(true);
                }
            }
        }

        Vector3 GetSeekItemPosition()
        {
            Vector3 pos = currentSeekItem.transform.position;
            Vector2 dir = _transform.position - pos;
            dir = dir.normalized;
            dir *= 0.055f;
            var colliders = currentSeekItem.GetComponentsInChildren<Collider2D>();
            foreach (var coll in colliders)
            {
                if (gameObject.layer == LayerMask.NameToLayer("Obstacle"))
                {
                    pos = coll.ClosestPoint(_transform.position);
                    break;
                }
            }

            return pos + (Vector3)dir;
        }

        GameObject CheckForSeekItem()
        {
            
            Collider2D[] colliders = Physics2D.OverlapCircleAll(_transform.position, seekRadius, LayerMask.GetMask("Interactable"), _transform.position.z, _transform.position.z);

            if (colliders.Length > 0)
            {

                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].gameObject.TryGetComponent(out Interactable.Interactable interactable))
                    {
                        if (interactable.isBeingDragged)
                            continue;
                    }
                        

                    if (colliders[i].gameObject.TryGetComponent(out QI_Item item))
                    {
                        
                        
                        if (item.Data == seekItem && !seekItemsFound.Contains(colliders[i].transform.position))
                            return colliders[i].gameObject;
                        
                    }
                }
            }
            return null;
        }


    }
}
