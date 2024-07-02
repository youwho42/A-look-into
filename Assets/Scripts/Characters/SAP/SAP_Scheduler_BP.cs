using Klaxon.GravitySystem;
using Klaxon.UndertakingSystem;
using QuantumTek.QuantumInventory;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Klaxon.Interactable;


namespace Klaxon.SAP
{
    public class SAP_Scheduler_BP : MonoBehaviour, IBallPerson
    {

        public readonly int walking_hash = Animator.StringToHash("IsWalking");
        public readonly int isGrounded_hash = Animator.StringToHash("IsGrounded");
        public readonly int velocityY_hash = Animator.StringToHash("VelocityY");
        public readonly int sleeping_hash = Animator.StringToHash("IsSleeping");
        public readonly int lick_hash = Animator.StringToHash("Lick");
        public Animator animator;

        public List<SAP_Goal> goals = new List<SAP_Goal>();

        public Dictionary<string, bool> beliefs = new Dictionary<string, bool>();

        int currentGoal = -1;

        
        [HideInInspector]
        public bool currentGoalComplete;
        public string currentGoalName;
        [HideInInspector]
        public bool offScreen;
        int currentGoalTimer = -1;
        
        [HideInInspector]
        public GravityItemWalk walker;
        [HideInInspector]
        public bool isDeviating;

        List<SpriteRenderer> allSprites = new List<SpriteRenderer>();

        [HideInInspector]
        public Klaxon.Interactable.Interactable interactor;
        [HideInInspector]
        public bool hasInteracted;
        public GameObject arms;
        public LayerMask interactableLayer;
        public LayerMask obstacleLayer;

        [HideInInspector]
        public int indicatorIndex = -1;

        [HideInInspector]
        public CompleteTaskObject task;

        [HideInInspector]
        public UIScreenManager sleep;

        public enum BP_Type
        {
            Messenger,
            Seeker,
            Traveller,
            Farmer,
            TravellerHome,
            Indicator,
            Villager
        }
        public BP_Type type;

        /// <summary>
        /// Seeker information
        /// </summary>
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

        /// <summary>
        /// Traveller information
        /// </summary>
        [HideInInspector]
        public bool hasFoundDestination;
        [HideInInspector]
        public Vector3 travellerDestination;
        [HideInInspector]
        public bool justIndicated;


        /// <summary>
        /// Planter / Harvester information
        /// </summary>
        [HideInInspector]
        public QI_Inventory seedBoxInventory;
        [HideInInspector]
        public PlantingArea plantingArea;
        [HideInInspector]
        public Vector3 currentPlantDestination;
        [HideInInspector]
        public PlantLife currentHarvestable;

        void OnEnable()
        {
            GameEventManager.onTimeTickEvent.AddListener(TimeTick);
        }
        void OnDisable()
        {
            GameEventManager.onTimeTickEvent.RemoveListener(TimeTick);
        }
        public void Start()
        {
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
            SetBeliefState("PlayerClose", true);
            SetBeliefState("FiresLit", false);
            foreach (var goal in goals)
            {
                SetGoalTimer(goal);
            }

        }
        public void TimeTick(int tick)
        {
            if (currentGoal == -1)
                return;
            if (goals[currentGoal].TimeLimit > 0)
                currentGoalTimer++;
        }

        void SetGoalTimer(SAP_Goal goal)
        {
            if (goal.TimeLimitRange != Vector2.zero)
                goal.TimeLimit = Random.Range(goal.TimeLimitRange.x, goal.TimeLimitRange.y);
        }

        public void Update()
        {
            

            if (currentGoal == -1)
            {
                SetNewGoal();
            }
            else
            {
                goals[currentGoal].Action.PerformAction(this);

                if (currentGoal == -1)
                    return;

                if (goals[currentGoal].TimeLimit > 0)
                {
                    
                    if (currentGoalTimer >= goals[currentGoal].TimeLimit)
                    {
                        SetBeliefState(goals[currentGoal].TimeLimitCondition.Condition, goals[currentGoal].TimeLimitCondition.State);
                        ResetCurrentGoal();
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


            if(type == BP_Type.Seeker)
            {
                if(seekItem != null && !isSeeking)
                    SeekItem();
            }

            if (type == BP_Type.Traveller && !justIndicated && !hasFoundDestination && !GetBeliefState("QuestOver"))
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



        void SetNewGoal()
        {

            int bestOption = -1;
            int bestIndex = -1;
            for (int i = 0; i < goals.Count; i++)
            {
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



            if (bestIndex > -1)
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


        public void Disolve(bool disolveIn)
        {
            for (int i = 0; i < allSprites.Count; i++)
            {
                if (allSprites[i].gameObject.activeSelf)
                    DissolveEffect.instance.StartDissolve(allSprites[i].material, 1f, disolveIn);
            }
            if (disolveIn)
                SetBeliefState("Exist", true);
            
        }

        public float CheckPlayerDistance()
        {
            float dist = Vector2.Distance(transform.position, PlayerInformation.instance.player.position);

            return dist;
        }


        void SeekTravellerDestination()
        {
            if (hasInteracted && !hasFoundDestination)
            {
                var dist = Vector3.Distance(transform.position, travellerDestination);
                if (dist < seekRadius)
                {
                    ResetCurrentGoal();
                    hasFoundDestination = true;
                    SetBeliefState("TravellerDestinationFound", true);
                }
            }
        }


        void SeekItem()
        {
            if (hasInteracted && !task.task.IsComplete)
            {
                currentSeekItem = CheckForSeekItem();
                if (currentSeekItem != null)
                {
                    currentSeekItemLocation = GetSeekItemPosition();
                    ResetCurrentGoal();
                    isSeeking = true;
                    SetBeliefState("SeekItemFound", true);
                    
                }
            }
        }

        Vector3 GetSeekItemPosition()
        {
            Vector3 pos = currentSeekItem.transform.position;
            Vector2 dir = transform.position - pos;
            dir = dir.normalized;
            dir *= 0.055f;
            var colliders = currentSeekItem.GetComponentsInChildren<Collider2D>();
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

        GameObject CheckForSeekItem()
        {

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, seekRadius, interactableLayer);

            if (colliders.Length > 0)
            {
                
                for (int i = 0; i < colliders.Length; i++)
                {
                    
                    if (colliders[i].transform.position.z != transform.position.z)
                        continue;

                    if (colliders[i].gameObject.TryGetComponent(out QI_Item item))
                    {
                        if (item.Data == seekItem && !seekItemsFound.Contains(colliders[i].transform.position))
                        {
                            
                            return colliders[i].gameObject;
                        }

                    }
                }

            }
            return null;
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

            if (CheckPlayerDistance() > 3f)
            {
                
                isDeviating = false;
                ResetCurrentGoal();
                SetBeliefState("PlayerFar", true);
            }
                

        }

        public void HandleOffScreen(SAP_Action action)
        {
            //walker.currentDirection = Vector2.zero;
            //if (offScreenPosMoved && action.currentPathIndex < action.path.Count)
            //{
            //    timeTo = Mathf.RoundToInt(Vector2.Distance(transform.position, action.path[action.currentPathIndex].transform.position) / walker.walkSpeed);
            //    timeTo = (timeTo + RealTimeDayNightCycle.instance.currentTimeRaw) % 1440;
            //    if (transform.position.x < action.path[action.currentPathIndex].transform.position.x && !walker.facingRight)
            //        walker.Flip();
            //    else if (transform.position.x > action.path[action.currentPathIndex].transform.position.x && walker.facingRight)
            //        walker.Flip();
            //    offScreenPosMoved = false;
            //}


            //if (RealTimeDayNightCycle.instance.currentTimeRaw >= timeTo && !offScreenPosMoved)
            //{

            //    offScreenPosMoved = true;
            //    walker.transform.position = action.path[action.currentPathIndex].transform.position;
            //    walker.currentTilePosition.position = walker.currentTilePosition.GetCurrentTilePosition(walker.transform.position);
            //    walker.currentLevel = walker.currentTilePosition.position.z;
            //    if (action.currentPathIndex < action.path.Count)
            //    {
            //        lastValidNode = action.currentNode;
            //        action.currentPathIndex++;

            //    }
            //    if (action.currentPathIndex >= action.path.Count)
            //    {

            //        lastValidNode = action.currentNode;
            //        action.ReachFinalDestination(this);
            //    }
            //    else
            //    {
            //        action.currentNode = action.path[action.currentPathIndex];
            //    }

            //}
        }



        void ResetCurrentGoal()
        {
            SetGoalTimer(goals[currentGoal]);
            if (currentGoal > -1)
                goals[currentGoal].Action.EndPerformAction(this);
            currentGoal = -1;
            currentGoalComplete = false;
        }

        public void SetBeliefState(string condition, bool state)
        {
            if (!beliefs.ContainsKey(condition))
                beliefs.Add(condition, state);
            else
                beliefs[condition] = state;
        }

        public bool GetBeliefState(string condition)
        {
            if (!beliefs.ContainsKey(condition))
                return false;
            else
                return beliefs[condition];
        }

        public void SetToRemoveState()
        {
            SetBeliefState("QuestOver", true);
            currentGoalComplete = true;
        }
    }
}
