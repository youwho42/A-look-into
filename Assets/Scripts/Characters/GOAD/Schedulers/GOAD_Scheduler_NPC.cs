using Klaxon.ConversationSystem;
using Klaxon.GravitySystem;
using Klaxon.Interactable;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Klaxon.GOAD
{
    public class GOAD_Scheduler_NPC : GOAD_Scheduler
    {


        public readonly int isGrounded_hash = Animator.StringToHash("IsGrounded");
        public readonly int isRunning_hash = Animator.StringToHash("IsRunning");
        public readonly int isSitting_hash = Animator.StringToHash("IsSitting");
        public readonly int isIdleSitting_hash = Animator.StringToHash("IdleSit");
        public readonly int isSleeping_hash = Animator.StringToHash("IsSleeping");
        public readonly int isCrafting_hash = Animator.StringToHash("IsCrafting");
        public readonly int velocityX_hash = Animator.StringToHash("VelocityX");
        public readonly int velocityY_hash = Animator.StringToHash("VelocityY");
        public Animator animator;


        //[HideInInspector]
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
        


        [Header("Node Pathfinding")]
        // Node Pathfinding
        [HideInInspector]
        public List<NavigationNode> nodePath = new List<NavigationNode>();
        [HideInInspector]
        public NavigationNode currentNode;
        [HideInInspector]
        public NavigationNode lastValidNode;
        

        [HideInInspector]
        public bool isInside;

        float timeTo;
        [HideInInspector]
        public bool offScreenPosMoved = true;
        [HideInInspector]
        public UIScreenManager sleep;

        [HideInInspector]
        public InteractableChair currentRestSeat;

        [HideInInspector]
        public bool isBusy;
        bool inTalkRange;
        float inTalkRangeTimer;
        DialogueManagerUI dialogueManager;
        public Transform speechBubbleTransform;
        public GOAD_ScriptableCondition canDailySpeakWithPlayer;
        public GOAD_ScriptableCondition canSpeakWithPlayerPeriod;
        [HideInInspector]
        public InteractableDialogue interactable;
        [HideInInspector]
        public int currentFailedPathfindingAttempts;
        public NavigationNode mainHomeNode;
        public override void Start()
        {
            base.Start();
            interactable = GetComponent<InteractableDialogue>();
            walker = GetComponent<GravityItemWalk>();
            agentInventory = GetComponent<QI_Inventory>();
            sleep = UIScreenManager.instance;
            dialogueManager = DialogueManagerUI.instance;
            lastValidTileLocation = transform.position;
        }

        

        private void Update()
        {
            
            if (inTalkRange)
            {
                TalkRangeTimer();
                return;
            }
            if (nearPlayer && IsConditionMet(canDailySpeakWithPlayer) && IsConditionMet(canSpeakWithPlayerPeriod))
            {
                if(Random.Range(0.0f, 1.0f) <= 0.25f)
                    ContextSpeechBubbleManager.instance.SetContextBubble(3, speechBubbleTransform, LocalizationSettings.StringDatabase.GetLocalizedString($"Variable-Texts", "Hello Player"), true);
                
                SetBeliefState(canDailySpeakWithPlayer.Condition, !canDailySpeakWithPlayer.State);
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
            if(start == end)
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

        public void NodeDeviate(GOAD_Action action)
        {
            if (UnstuckCharacter())
                return;

            isDeviating = true;
            if (walker.isStuck)
                walker.hasDeviatePosition = false;

            if (!walker.hasDeviatePosition)
                walker.FindDeviateDestination(walker.tilemapObstacle ? 20 : 50);

            animator.SetFloat(velocityX_hash, 1);
            walker.SetDirection();

            if (walker.CheckDistanceToDestination() <= 0.02f)
                isDeviating = false;

            walker.SetLastPosition();
        }

        public void AStarDeviate(GOAD_Action action)
        {

            if (UnstuckCharacter())
                return;


            isDeviating = true;
            if (walker.isStuck)
                walker.hasDeviatePosition = false;

            if (!walker.hasDeviatePosition)
                walker.FindDeviateDestination(walker.tilemapObstacle ? 20 : 50);

            animator.SetFloat(velocityX_hash, 1);
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
                    Debug.LogWarning($"start position not valid after deviate \ncurrent pos{transform.position} last pos{lastValidTileLocation}", gameObject);
                    transform.position = lastValidTileLocation;
                    //aStarPath.Add(lastValidTileLocation);
                }

            }


            walker.SetLastPosition();
        }

        bool UnstuckCharacter()
        {
            var hits = Physics2D.OverlapPointAll(transform.position, LayerMask.GetMask("Obstacle"), transform.position.z, transform.position.z);
            for (int i = 0; i < hits.Length; i++)
            {

                if (hits[i].TryGetComponent(out DrawZasYDisplacement disp))
                {
                    if (disp.positionZ <= 0)
                        continue;

                    for (int d = 1; d < 6; d++)
                    {
                        for (float a = 0; a < Mathf.PI * 2; a += Mathf.PI * 0.25f)
                        {
                            Vector2 dir = new Vector2(Mathf.Cos(a), Mathf.Sin(a));
                            dir = dir.normalized;
                            dir *= d * 0.05f;
                            var posI = transform.position + (Vector3)dir;
                            var h = Physics2D.OverlapPoint(posI, LayerMask.GetMask("Obstacle"), posI.z, posI.z);
                            if (!h && GridManager.instance.GetTileValid(posI))
                            {
                                transform.position = posI;
                                return true;
                            }
                        }
                    }

                }


            }
            return false;
        }


        public void Deviate()
        {
            if (UnstuckCharacter())
                return;

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


        public void HandleOffScreenNodes(GOAD_Action action)
        {
            walker.currentDirection = Vector2.zero;
            if (offScreenPosMoved && currentPathIndex < nodePath.Count)
            {
                timeTo = Mathf.RoundToInt(Vector2.Distance(_transform.position, nodePath[currentPathIndex].transform.position) / walker.walkSpeed);
                timeTo = (timeTo + RealTimeDayNightCycle.instance.currentTimeRaw) % 1440;
                if (_transform.position.x < nodePath[currentPathIndex].transform.position.x && !walker.facingRight)
                    walker.Flip();
                else if (_transform.position.x > nodePath[currentPathIndex].transform.position.x && walker.facingRight)
                    walker.Flip();
                offScreenPosMoved = false;
            }


            if (RealTimeDayNightCycle.instance.currentTimeRaw >= timeTo && !offScreenPosMoved)
            {

                offScreenPosMoved = true;
                _transform.position = nodePath[currentPathIndex].transform.position;
                walker.currentTilePosition.position = walker.currentTilePosition.GetCurrentTilePosition(_transform.position);
                walker.currentLevel = walker.currentTilePosition.position.z;
                if (currentPathIndex < nodePath.Count)
                {
                    lastValidNode = currentNode;
                    currentPathIndex++;
                }
                if (currentPathIndex >= nodePath.Count)
                {
                    lastValidNode = currentNode;
                    action.OffscreenNodeHandleComplete(this);
                }
                else
                {
                    currentNode = nodePath[currentPathIndex];
                }

            }
        }

        void TalkRangeTimer()
        {
            if (!dialogueManager.isSpeaking)
            {
                inTalkRangeTimer += Time.deltaTime;
                if (inTalkRangeTimer >= 5f)
                {
                    inTalkRange = false;
                    inTalkRangeTimer = 0;
                }
            }
            else
            {
                if (dialogueManager.currentInteractable.gameObject != gameObject)
                    inTalkRange = false;
                inTalkRangeTimer = 0;
            }

            if (!animator.GetBool(isSitting_hash) && !animator.GetBool(isSleeping_hash))
            {
                if (PlayerInformation.instance.player.position.x < transform.position.x && walker.facingRight ||
                PlayerInformation.instance.player.position.x > transform.position.x && !walker.facingRight)
                    walker.Flip();
            }

        }





        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (interactable == null)
                return;
            if (!interactable.canInteract)
                return;
           

            if (collision.gameObject.CompareTag("Player") && collision.transform.position.z == _transform.position.z)
            {

                if (isBusy || animator.GetBool(isSleeping_hash) || PlayerInformation.instance.playerAnimator.GetBool(isIdleSitting_hash))
                    return;

                StopNPC(collision.transform.position);

            }
        }

        public void StopNPC(Vector3 position)
        {
            inTalkRange = true;
            animator.SetFloat(velocityX_hash, 0);
            walker.currentDirection = Vector2.zero;

            if (!animator.GetBool(isSitting_hash))
            {
                if (position.x < _transform.position.x && walker.facingRight ||
                    position.x > _transform.position.x && !walker.facingRight)
                    walker.Flip();
            }
        }

        public void OnTriggerExit2D(Collider2D collision)
        {

            if (collision.gameObject.CompareTag("Player") && collision.transform.position.z == _transform.position.z)
                inTalkRange = false;

        }




    }


}