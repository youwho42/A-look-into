using Klaxon.ConversationSystem;
using Klaxon.GravitySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GOAD
{
	public class GOAD_Scheduler_Ghost : GOAD_Scheduler
	{
        
        public readonly int velocityX_hash = Animator.StringToHash("VelocityX");
        public Animator animator;
        public Transform ghostHome;
        [HideInInspector]
        public bool isDeviating;

        [HideInInspector]
        public bool offScreen;

        [HideInInspector]
        public GravityItemGhost ghoster;

        [HideInInspector]
        public int currentPathIndex;

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

        bool inTalkRange;
        float inTalkRangeTimer;
        DialogueManagerUI dialogueManager;
        


        public override void Start()
        {
            base.Start();
            ghoster = GetComponent<GravityItemGhost>();
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
            if (PathRequestManager.instance.pathfinding.isometricGrid.nodeLookup.TryGetValue(ghoster.currentTilePosition.position, out IsometricNodeXYZ node))
                return node.walkable;
            return false;
        }

        public void GetRandomTilePosition(int distance, GOAD_Action action)
        {
            var currentPos = ghoster.centerOfActiveArea == null ? _transform.position : ghoster.centerOfActiveArea.transform.position;
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
            PathRequestManager.RequestPath(new PathRequest(ghoster.currentTilePosition.position, gridPos, true, OnPathFound));
        }

        public void OnPathFound(List<Vector3> newPath, bool success)
        {
            if (success)
            {
                List<Vector3> finalPath = new List<Vector3>();  
                foreach (var pathPos in newPath)
                {
                    var p = pathPos + new Vector3(Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f), 0.0f);
                    finalPath.Add(p);
                }

                aStarPath = finalPath;
            }
            else
                Debug.LogWarning("Path not found");

            gettingPath = false;
        }


        public void AStarDeviate(GOAD_Action action)
        {
            isDeviating = true;
            if (ghoster.isStuck)
                ghoster.hasDeviatePosition = false;

            if (!ghoster.hasDeviatePosition)
                ghoster.FindDeviateDestination(ghoster.tilemapObstacle ? 20 : 50);

            animator.SetFloat(velocityX_hash, 1);
            ghoster.SetDirection();

            if (ghoster.CheckDistanceToDestination() <= GlobalSettings.DistanceCheck)
            {
                isDeviating = false;
                currentPathIndex = 0;
                aStarPath.Clear();
                if (StartPositionValid())
                    SetAStarDestination(currentFinalDestination, action);
                else
                {
                    Debug.Log("start position not valid after deviate", gameObject);
                    aStarPath.Add(lastValidTileLocation);
                }

            }


            ghoster.SetLastPosition();
        }

        public void HandleOffScreenAStar(GOAD_Action action)
        {
            ghoster.currentDirection = Vector2.zero;



            if (offScreenPosMoved && currentPathIndex < aStarPath.Count)
            {
                timeTo = Mathf.RoundToInt(Vector2.Distance(_transform.position, aStarPath[currentPathIndex]) / ghoster.floatSpeed);
                timeTo = (timeTo + RealTimeDayNightCycle.instance.currentTimeRaw) % 1440;

                if (transform.position.x < aStarPath[currentPathIndex].x && !ghoster.facingRight)
                    ghoster.Flip();
                else if (transform.position.x > aStarPath[currentPathIndex].x && ghoster.facingRight)
                    ghoster.Flip();

                offScreenPosMoved = false;
            }


            if (RealTimeDayNightCycle.instance.currentTimeRaw >= timeTo && !offScreenPosMoved)
            {

                offScreenPosMoved = true;
                _transform.position = aStarPath[currentPathIndex];
                ghoster.currentTilePosition.position = ghoster.currentTilePosition.GetCurrentTilePosition(_transform.position);
                ghoster.currentLevel = ghoster.currentTilePosition.position.z;

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

            
            if (PlayerInformation.instance.player.position.x < _transform.position.x && ghoster.facingRight ||
            PlayerInformation.instance.player.position.x > _transform.position.x && !ghoster.facingRight)
                ghoster.Flip();
            

        }





        public void OnTriggerEnter2D(Collider2D collision)
        {



            if (collision.gameObject.CompareTag("Player") && collision.transform.position.z == _transform.position.z)
            {

                if (PlayerInformation.instance.playerAnimator.GetBool("IsSitting") || PlayerInformation.instance.playerAnimator.GetBool("IsSleeping"))
                    return;

                inTalkRange = true;
                animator.SetFloat(velocityX_hash, 0);
                ghoster.currentDirection = Vector2.zero;
                
                if (collision.transform.position.x < _transform.position.x && ghoster.facingRight ||
                collision.transform.position.x > _transform.position.x && !ghoster.facingRight)
                    ghoster.Flip();
                


            }
        }



        public void OnTriggerExit2D(Collider2D collision)
        {

            if (collision.gameObject.CompareTag("Player") && collision.transform.position.z == _transform.position.z)
                inTalkRange = false;

        }






    }
}
