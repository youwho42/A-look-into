using Klaxon.ConversationSystem;
using Klaxon.GravitySystem;
using QuantumTek.QuantumInventory;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.SAP
{
    public class SAP_Scheduler_NPC : MonoBehaviour
    {

        public readonly int isGrounded_hash = Animator.StringToHash("IsGrounded");
        public readonly int isSitting_hash = Animator.StringToHash("IsSitting");
        public readonly int isSleeping_hash = Animator.StringToHash("IsSleeping");
        public readonly int isCrafting_hash = Animator.StringToHash("IsCrafting");
        public readonly int velocityX_hash = Animator.StringToHash("VelocityX");
        public readonly int velocityY_hash = Animator.StringToHash("VelocityY");
        public Animator animator;
        
        public List<SAP_Goal> goals = new List<SAP_Goal>();

        public Dictionary<string, bool> beliefs = new Dictionary<string, bool>();

        int currentGoal = -1;

        [HideInInspector]
        public NavigationNodeType currentNavigationNodeType;
        
        public NavigationPathType pathType;
        [HideInInspector]
        public bool currentGoalComplete;
        public string currentGoalName;
        [HideInInspector]
        public bool offScreen;
        int currentGoalTimer = -1;
        [HideInInspector]
        public NavigationNode lastValidNode;
        bool inTalkRange;
        float inTalkRangeTimer;
        DialogueManagerUI dialogueManager;

        [HideInInspector]
        public GravityItemWalk walker;

        [HideInInspector]
        public QI_Inventory agentInventory;
        public QI_Inventory stashInventory;
        [HideInInspector]
        public UIScreenManager sleep;
        [HideInInspector]
        public bool offScreenPosMoved = true;
        [HideInInspector]
        public int timeTo;
        [HideInInspector]
        public bool isDeviating;

        [Header("A* Pathfinding")]
        // A* Pathfinding
        [HideInInspector]
        public List<Vector3> aStarPath = new List<Vector3>();
        public bool useAStar;
        [HideInInspector]
        public bool gettingPath;
        [HideInInspector]
        public Vector3 currentFinalDestination;
        [HideInInspector]
        public Vector3 lastValidTileLocation;

        public void Start()
        {
            walker = GetComponent<GravityItemWalk>();
            agentInventory = GetComponent<QI_Inventory>();
            sleep = UIScreenManager.instance;
            dialogueManager = DialogueManagerUI.instance;
            lastValidTileLocation = transform.position;
        }

        public void Update()
        {
            if (inTalkRange)
            {
                TalkRangeTimer();
                return;
            }
                

            
            if (currentGoal == -1)
            {
                SetNewGoal();
            }
            else
            { 
                goals[currentGoal].Action.PerformAction(this);

                if (goals[currentGoal].TimeLimit > 0)
                {
                    if (currentGoalTimer == -1)
                        currentGoalTimer = (RealTimeDayNightCycle.instance.currentTimeRaw + goals[currentGoal].TimeLimit) % 1440;
                    if (RealTimeDayNightCycle.instance.currentTimeRaw == currentGoalTimer)
                    {
                        SetBeliefState(goals[currentGoal].TimeLimitCondition.Condition, goals[currentGoal].TimeLimitCondition.State);
                        currentGoalTimer = -1;
                    }
                }
            }
            

            if (currentGoalComplete && currentGoal >= 0)
            {
                goals[currentGoal].Action.EndPerformAction(this);
                currentGoal = -1;

                currentGoalComplete = false;
            }
        }

        void SetNewGoal()
        {
            
            int bestOption = -1;
            int bestIndex = -1;
            for (int i = 0; i < goals.Count; i++)
            {
                goals[i].Action.InitialCheckPerformAction(this);
                goals[i].IsRunning = false;
                if (CanCompleteGoal(goals[i]))
                {
                    if (goals[i].Priority > bestOption)
                    {
                        bestOption = goals[i].Priority;
                        bestIndex = i;
                    }

                }
            }
            
            if(bestIndex > -1)
            {

                if (currentGoal != bestIndex)
                {
                    if (currentGoal > -1)
                        goals[currentGoal].Action.EndPerformAction(this);

                    goals[bestIndex].Action.StartPerformAction(this);
                }

                currentGoal = bestIndex;
                goals[currentGoal].IsRunning = true;

                if (currentGoalName != goals[currentGoal].GoalName)
                    currentGoalTimer = -1;
                
                currentGoalName = goals[currentGoal].GoalName;
            }
            else
            {
                currentGoalName = "";
                currentGoalTimer = -1;
            }
            
        }

        bool CanCompleteGoal(SAP_Goal goal)
        {
            Dictionary<string, bool> temp = new Dictionary<string, bool>(beliefs);

            // Combine the two dictionaries without modifying the original dictionaries
            foreach (var kvp in SAP_WorldBeliefStates.instance.worldStates)
            {
                temp[kvp.Key] = kvp.Value;
            }
            int conditionsMet = 0;
            foreach (var c in goal.Conditions)
            {
                bool state;
                if (temp.TryGetValue(c.Condition, out state))
                {
                    if (c.State == state)
                    {
                        conditionsMet++;
                    }
                }
            }
            return conditionsMet == goal.Conditions.Count;
        }

        
        void TalkRangeTimer()
        {
            if (!dialogueManager.isSpeaking) { 
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



        

        public void SetBeliefState(string condition, bool state)
        {
            if (!beliefs.ContainsKey(condition))
                beliefs.Add(condition, state);
            else
                beliefs[condition] = state;
        }

        public bool HasBelief(string condition, bool state)
        {
            if (beliefs.ContainsKey(condition))
            {
                if (beliefs[condition] == state)
                    return true;
            }
            return false;
        }


        public void HandleOffScreenNodes(SAP_Action action)
        {
            walker.currentDirection = Vector2.zero;
            if (offScreenPosMoved && action.currentPathIndex < action.path.Count)
            {
                timeTo = Mathf.RoundToInt(Vector2.Distance(transform.position, action.path[action.currentPathIndex].transform.position) / walker.walkSpeed);
                timeTo = (timeTo + RealTimeDayNightCycle.instance.currentTimeRaw) % 1440;
                if (transform.position.x < action.path[action.currentPathIndex].transform.position.x && !walker.facingRight)
                    walker.Flip();
                else if (transform.position.x > action.path[action.currentPathIndex].transform.position.x && walker.facingRight)
                    walker.Flip();
                offScreenPosMoved = false;
            }

            
            if (RealTimeDayNightCycle.instance.currentTimeRaw >= timeTo && !offScreenPosMoved)
            {

                offScreenPosMoved = true;
                walker.transform.position = action.path[action.currentPathIndex].transform.position;
                walker.currentTilePosition.position = walker.currentTilePosition.GetCurrentTilePosition(walker.transform.position);
                walker.currentLevel = walker.currentTilePosition.position.z;
                if (action.currentPathIndex < action.path.Count)
                {
                    lastValidNode = action.currentNode;
                    action.currentPathIndex++;
                    
                }
                if (action.currentPathIndex >= action.path.Count)
                {
                    
                    lastValidNode = action.currentNode;
                    action.ReachFinalDestination(this);
                }
                else
                {
                    action.currentNode = action.path[action.currentPathIndex];
                }

            }
        }

        public void HandleOffScreenAStar(SAP_Action action)
        {
            walker.currentDirection = Vector2.zero;

            if (action.currentPathIndex >= aStarPath.Count)
            {
                action.ReachFinalDestination(this);
                return;
            }

            if (offScreenPosMoved && action.currentPathIndex < aStarPath.Count)
            {
                timeTo = Mathf.RoundToInt(Vector2.Distance(transform.position, aStarPath[action.currentPathIndex]) / walker.walkSpeed);
                timeTo = (timeTo + RealTimeDayNightCycle.instance.currentTimeRaw) % 1440;

                if (transform.position.x < aStarPath[action.currentPathIndex].x && !walker.facingRight)
                    walker.Flip();
                else if (transform.position.x > aStarPath[action.currentPathIndex].x && walker.facingRight)
                    walker.Flip();

                offScreenPosMoved = false;
            }
            

            if (RealTimeDayNightCycle.instance.currentTimeRaw >= timeTo && !offScreenPosMoved)
            {

                offScreenPosMoved = true;
                walker.transform.position = aStarPath[action.currentPathIndex];
                walker.currentTilePosition.position = walker.currentTilePosition.GetCurrentTilePosition(walker.transform.position);
                walker.currentLevel = walker.currentTilePosition.position.z;

                if (action.currentPathIndex < aStarPath.Count)
                    action.currentPathIndex++;
                
            }
        }

        public void Deviate()
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
                //SetAStarDestination(currentFinalDestination);
            }
                

            walker.SetLastPosition();
        }
        public void Deviate(SAP_Action action)
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
                action.currentPathIndex = 0;
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



        public void OnTriggerEnter2D(Collider2D collision)
        {

            if (collision.transform.position.z != transform.position.z)
                return;

            if (collision.CompareTag("House"))
            {
                if (collision.OverlapPoint(transform.position))
                    currentNavigationNodeType = NavigationNodeType.Inside;
            }



            if (collision.gameObject.CompareTag("Player"))
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



            if (collision.CompareTag("House"))
                currentNavigationNodeType = NavigationNodeType.Outside;


            if (collision.gameObject.CompareTag("Player"))
            {
                
                inTalkRange = false;
            }
        }

        


    }
}
