using Klaxon.ConversationSystem;
using Klaxon.GravitySystem;
using Klaxon.Interactable;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
    public class GOAD_Scheduler_NPC : GOAD_Scheduler
    {


        public readonly int isGrounded_hash = Animator.StringToHash("IsGrounded");
        public readonly int isSitting_hash = Animator.StringToHash("IsSitting");
        public readonly int isSleeping_hash = Animator.StringToHash("IsSleeping");
        public readonly int isCrafting_hash = Animator.StringToHash("IsCrafting");
        public readonly int velocityX_hash = Animator.StringToHash("VelocityX");
        public readonly int velocityY_hash = Animator.StringToHash("VelocityY");
        public Animator animator;


        [HideInInspector]
        public bool isDeviating;

        [HideInInspector]
        public bool offScreen;

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

        bool inTalkRange;
        float inTalkRangeTimer;
        DialogueManagerUI dialogueManager;

        public override void Start()
        {
            base.Start();
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
                    {
                        SetNextAction();
                    }
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

        public void GetRandomTilePosition(float distance)
        {
            var currentPos = transform.position;
            Vector3 destination = currentPos;
            while (destination == currentPos)
            {
                destination = GridManager.instance.GetRandomTileWorldPosition(currentPos, distance);
            }
            currentFinalDestination = destination;
            SetAStarDestination(destination);
        }

        public void SetAStarDestination(Vector3 destination)
        {
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
                aStarPath = newPath;
            }
            else
            {
                Debug.LogWarning("Path not found");
            }
            gettingPath = false;
        }

        public void NodeDeviate(GOAD_Action action)
        {
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

            }

            walker.SetLastPosition();
        }

        public void AStarDeviate(GOAD_Action action)
        {
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
                    SetAStarDestination(currentFinalDestination);
                else
                {
                    Debug.Log("start position not valid after deviate");
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
                timeTo = Mathf.RoundToInt(Vector2.Distance(transform.position, aStarPath[currentPathIndex]) / walker.walkSpeed);
                timeTo = (timeTo + RealTimeDayNightCycle.instance.currentTimeRaw) % 1440;

                if (transform.position.x < aStarPath[currentPathIndex].x && !walker.facingRight)
                    walker.Flip();
                else if (transform.position.x > aStarPath[currentPathIndex].x && walker.facingRight)
                    walker.Flip();

                offScreenPosMoved = false;
            }


            if (RealTimeDayNightCycle.instance.currentTimeRaw >= timeTo && !offScreenPosMoved)
            {

                offScreenPosMoved = true;
                walker.transform.position = aStarPath[currentPathIndex];
                walker.currentTilePosition.position = walker.currentTilePosition.GetCurrentTilePosition(walker.transform.position);
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
                timeTo = Mathf.RoundToInt(Vector2.Distance(transform.position, nodePath[currentPathIndex].transform.position) / walker.walkSpeed);
                timeTo = (timeTo + RealTimeDayNightCycle.instance.currentTimeRaw) % 1440;
                if (transform.position.x < nodePath[currentPathIndex].transform.position.x && !walker.facingRight)
                    walker.Flip();
                else if (transform.position.x > nodePath[currentPathIndex].transform.position.x && walker.facingRight)
                    walker.Flip();
                offScreenPosMoved = false;
            }


            if (RealTimeDayNightCycle.instance.currentTimeRaw >= timeTo && !offScreenPosMoved)
            {

                offScreenPosMoved = true;
                walker.transform.position = nodePath[currentPathIndex].transform.position;
                walker.currentTilePosition.position = walker.currentTilePosition.GetCurrentTilePosition(walker.transform.position);
                walker.currentLevel = walker.currentTilePosition.position.z;
                if (currentPathIndex < nodePath.Count)
                {
                    lastValidNode = currentNode;
                    currentPathIndex++;

                }
                if (currentPathIndex >= nodePath.Count)
                {

                    lastValidNode = currentNode;
                    action.success = true;
                    SetActionComplete(true);
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



            if (collision.gameObject.CompareTag("Player") && collision.transform.position.z == transform.position.z)
            {
                inTalkRange = true;
                animator.SetFloat(velocityX_hash, 0);
                walker.currentDirection = Vector2.zero;
                if (!animator.GetBool(isSitting_hash) && !animator.GetBool(isSleeping_hash))
                {
                    if (collision.transform.position.x < transform.position.x && walker.facingRight ||
                    collision.transform.position.x > transform.position.x && !walker.facingRight)
                        walker.Flip();
                }


            }
        }



        public void OnTriggerExit2D(Collider2D collision)
        {

            if (collision.gameObject.CompareTag("Player") && collision.transform.position.z == transform.position.z)
            {

                inTalkRange = false;
            }
        }




    }


}