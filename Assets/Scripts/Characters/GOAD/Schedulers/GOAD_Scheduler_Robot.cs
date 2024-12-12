using Klaxon.GravitySystem;
using Klaxon.Interactable;
using QuantumTek.QuantumInventory;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
	public class GOAD_Scheduler_Robot : GOAD_Scheduler
	{
        public readonly int Direction_hash = Animator.StringToHash("DirectionX");
        public readonly int Gather_hash = Animator.StringToHash("Gather");
        public readonly int Wander_hash = Animator.StringToHash("Wander");
        public readonly int Open_hash = Animator.StringToHash("Open");
        public readonly int Close_hash = Animator.StringToHash("Close");
        public readonly int Deactivate_hash = Animator.StringToHash("Deactivate");
        public Animator animator;
        bool changingDirection;

        [HideInInspector]
        public bool isDeviating;

        [HideInInspector]
        public bool offScreen;
        [HideInInspector]
        public bool nearPlayer;

        [HideInInspector]
        public GravityItemWalk walker;

        [HideInInspector]
        public int currentPathIndex;
        [HideInInspector]
        public QI_Inventory agentInventory;
        public RobotLightManager robotLights;

        [Header("A* Pathfinding")]
        // A* Pathfinding
        [HideInInspector]
        public List<Vector3> aStarPath = new List<Vector3>();

        [HideInInspector]
        public bool gettingPath;
        [HideInInspector]
        public Vector3 currentFinalDestination;
        [HideInInspector]
        public Vector3 lastValidTileLocation;

        float timeTo;
        bool hasGatherTime;
        [HideInInspector]
        public bool offScreenPosMoved = true;
        [HideInInspector]
        public UIScreenManager sleep;
        public InteractableRobot interactable;
        bool inInteractRange;
        float inInteractRangeTimer;

        public GOAD_ScriptableCondition robotActiveCondition;
        public NavigationNode homeBase;
        public override void Start()
        {
            base.Start();
            
            walker = GetComponent<GravityItemWalk>();
            agentInventory = GetComponent<QI_Inventory>();
            sleep = UIScreenManager.instance;
            lastValidTileLocation = transform.position;
        }



        private void Update()
        {
            if (interactable.isOpen)
            {
                walker.currentDirection = Vector2.zero;
                return;
            }
                
            if (inInteractRange)
            {
                TalkRangeTimer();
                return;
            }
            //if (nearPlayer && IsConditionMet(canDailySpeakWithPlayer) && IsConditionMet(canSpeakWithPlayerPeriod))
            //{
            //    if (Random.Range(0.0f, 1.0f) <= 0.25f)
            //        ContextSpeechBubbleManager.instance.SetContextBubble(3, speechBubbleTransform, LocalizationSettings.StringDatabase.GetLocalizedString($"Variable-Texts", "Hello Player"), true);

            //    SetBeliefState(canDailySpeakWithPlayer.Condition, !canDailySpeakWithPlayer.State);
            //}

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


        public bool StartPositionValid()
        {
            if (PathRequestManager.instance.pathfinding.isometricGrid.nodeLookup.TryGetValue(walker.currentTilePosition.position, out IsometricNodeXYZ node))
                return node.walkable;
            return false;
        }

        public void GetRandomTilePosition(int distance, GOAD_Action action)
        {
            var currentPos = _transform.position;
            Vector3 destination = currentPos;
            destination = GridManager.instance.GetRandomTileWorldPosition(currentPos, distance * .5f);
            currentFinalDestination = destination;
            SetAStarDestination(destination, action);
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

        public void AStarDeviate(GOAD_Action action)
        {
            isDeviating = true;
            if (walker.isStuck)
                walker.hasDeviatePosition = false;

            if (!walker.hasDeviatePosition)
                walker.FindDeviateDestination(walker.tilemapObstacle ? 20 : 50);

            //animator.SetFloat(velocityX_hash, 1);
            walker.SetDirection();

            if (walker.CheckDistanceToDestination() <= 0.02f)
            {
                isDeviating = false;
                currentPathIndex = 0;
                aStarPath.Clear();
                if (StartPositionValid())
                    SetAStarDestination(currentFinalDestination, action);
                else
                {
                    Debug.LogWarning("start position not valid after deviate", gameObject);
                    aStarPath.Add(lastValidTileLocation);
                }

            }


            walker.SetLastPosition();
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


        public void HandleSleepGather(GOAD_Action action)
        {
            if (!hasGatherTime)
            {
                timeTo = 6;
                timeTo = (timeTo + RealTimeDayNightCycle.instance.currentTimeRaw) % 1440;
                hasGatherTime = true;
            }
            
            if (RealTimeDayNightCycle.instance.currentTimeRaw >= timeTo)
            {
                agentInventory.AddItem(interactable.robotPriorities[interactable.currentPriority].PriorityDatabase.GetRandomWeightedItem(), 1, false);
                robotLights.SetInventoryLights();
                action.success = true;
                SetActionComplete(true);
                hasGatherTime = false;
                return;
            }

        }

        public void SetRobotActive(bool state)
        {
            SetBeliefState(robotActiveCondition.Condition, state);
        }

        void TalkRangeTimer()
        {
            if (!interactable.isOpen)
            {
                inInteractRangeTimer += Time.deltaTime;
                if (inInteractRangeTimer >= 5f)
                {
                    inInteractRange = false;
                    inInteractRangeTimer = 0;
                }
            }
            //else
            //{
            //    if (dialogueManager.currentInteractable.gameObject != gameObject)
            //        inInteractRange = false;
            //    inTalkRangeTimer = 0;
            //}

            
            if (PlayerInformation.instance.player.position.x < transform.position.x && walker.facingRight ||
            PlayerInformation.instance.player.position.x > transform.position.x && !walker.facingRight)
                walker.Flip();
            

        }


        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (interactable == null)
                return;
            if (!interactable.canInteract)
                return;


            if (collision.gameObject.CompareTag("Player") && collision.transform.position.z == _transform.position.z)
            {

                //if (PlayerInformation.instance.playerAnimator.GetBool(isIdleSitting_hash))
                //    return;

                inInteractRange = true;
                //animator.SetFloat(velocityX_hash, 0);
                walker.currentDirection = Vector2.zero;

                
                if (collision.transform.position.x < _transform.position.x && walker.facingRight ||
                collision.transform.position.x > _transform.position.x && !walker.facingRight)
                    walker.Flip();



            }
        }



        public void OnTriggerExit2D(Collider2D collision)
        {

            if (collision.gameObject.CompareTag("Player"))
                inInteractRange = false;

        }

    }
}
