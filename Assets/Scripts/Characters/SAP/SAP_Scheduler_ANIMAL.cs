using JetBrains.Annotations;
using Klaxon.ConversationSystem;
using Klaxon.GravitySystem;
using QuantumTek.QuantumInventory;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Klaxon.SAP
{
    public class SAP_Scheduler_ANIMAL : MonoBehaviour, IAnimal
    {

        public readonly int landed_hash = Animator.StringToHash("IsLanded");
        public readonly int walking_hash = Animator.StringToHash("IsWalking");
        public readonly int gliding_hash = Animator.StringToHash("IsGliding");
        public readonly int sleeping_hash = Animator.StringToHash("IsSleeping");
        public readonly int isGrounded_hash = Animator.StringToHash("IsGrounded");
        public readonly int isRunning_hash = Animator.StringToHash("IsRunning");
        public readonly int isSitting_hash = Animator.StringToHash("IsSitting");
        public readonly int isEating_hash = Animator.StringToHash("IsEating");
        public readonly int isScritching_hash = Animator.StringToHash("IsScritching");
        public readonly int velocityY_hash = Animator.StringToHash("VelocityY");
        public readonly int idle_hash = Animator.StringToHash("Idle");
        public readonly int climbing_hash = Animator.StringToHash("IsClimbing");
        public readonly int climbIdle_hash = Animator.StringToHash("ClimbIdle");


        public Animator animator;

        public List<SAP_Goal> goals = new List<SAP_Goal>();

        public Dictionary<string, bool> beliefs = new Dictionary<string, bool>();
        [HideInInspector]
        public int currentGoal = -1;


        [HideInInspector]
        public AnimalSounds sounds;
        [HideInInspector]
        public MusicGeneratorItem musicItem;


        [HideInInspector]
        public bool currentGoalComplete;
        public string currentGoalName;
        [HideInInspector]
        public bool offScreen;
        int currentGoalTimer = -1;

        [HideInInspector]
        public GravityItemWalk walker;
        [HideInInspector]
        public GravityItemFly flier;
        [HideInInspector]
        public GravityItemJump jumper;
        [HideInInspector]
        public bool isDeviating;

        public Vector2 minMaxFlap;
        public Vector2 minMaxGlide;
        [HideInInspector]
        public bool glide;

        
        
        public DrawZasYDisplacement currentDisplacementSpot;

        public MusicGeneratorItem musicGeneratorItem;

        public InteractAreasManager interactAreas;

        [HideInInspector]
        public RealTimeDayNightCycle dayNightCycle;

        [HideInInspector]
        public Bounds bounds;
        public bool isNocturnal;
        
        public float minHomeDistance = 1f;

        [HideInInspector]
        public List<DrawZasYDisplacement> closestSpots = new List<DrawZasYDisplacement>();
        public bool removeFromMusicAtHome;

        public bool hasDialogue;
        bool inTalkRange;
        float inTalkRangeTimer;
        DialogueManagerUI dialogueManager;
        [HideInInspector]
        public Transform fleeTransfrom;
        bool fleeing;
        [HideInInspector]
        public UIScreenManager sleep;

        [Header("Eating")]
        public bool canEat;
        [ConditionalHide("canEat", true)]
        public Transform eatPoint;
        [ConditionalHide("canEat", true)]
        public LayerMask interactableLayer;
        public List<QI_ItemData> edibleItems = new List<QI_ItemData>();
        [HideInInspector]
        public bool isEating;
        public GameObject currentEdible;

        [Header("Scritching")]
        public bool canScritch;
        [HideInInspector]
        public bool isScritching;
        [ConditionalHide("canScritch", true)]
        public LayerMask scritchableLayer;
        [ConditionalHide("canScritch", true)]
        public SpotType animalSpotType;
        [HideInInspector]
        public Transform currentScritchableItem;
        [HideInInspector]
        public Transform currentScritchablePosition;
        [HideInInspector]
        public float scritchCoolDown = 30f;
        [HideInInspector]
        public float scritchTimer;

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
            dialogueManager = DialogueManagerUI.instance;
            sleep = UIScreenManager.instance;
            scritchTimer = scritchCoolDown;
            isScritching = false;
            walker = GetComponent<GravityItemWalk>();
            if (walker != null)
            {
                walker.currentDirection = Vector2.zero;
                walker.ResetLastPosition();
            }
                
            flier = GetComponent<GravityItemFly>();
            jumper = GetComponent<GravityItemJump>();
            

            sounds = GetComponent<AnimalSounds>();
            musicItem = GetComponentInChildren<MusicGeneratorItem>();

            dayNightCycle = RealTimeDayNightCycle.instance;

            bounds = new Bounds(transform.position, new Vector3(4, 4, 4));

        }

        public int SetRandomRange(Vector2Int minMaxRange)
        {
            return Random.Range(minMaxRange.x, minMaxRange.y);
        }

        public float SetRandomRange(Vector2 minMaxRange)
        {
            return Random.Range(minMaxRange.x, minMaxRange.y);
        }


        public void TimeTick(int tick)
        {
            if (currentGoal == -1)
                return;
            if (goals[currentGoal].TimeLimit > 0)
                currentGoalTimer++;
        }

        public void Update()
        {
            


            if (inTalkRange && hasDialogue)
            {
                TalkRangeTimer();
                return;
            }

            if(flier != null) 
                SetBeliefState("OverWater", flier.isOverWater);


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

            if (FoundFood() || ScritchableFound())
                ResetCurrentGoal();
            
            if(GetBeliefState("Flee"))
            {
                if(fleeing == false)
                {
                    fleeing = true;
                    ResetCurrentGoal();
                }
            }
            else
            {
                fleeing = false;
            }


            if (currentGoalComplete && currentGoal >= 0)
            {
                goals[currentGoal].Action.EndPerformAction(this);
                currentGoal = -1;

                currentGoalComplete = false;
            }

        }

        void LateUpdate()
        {

            if (walker != null)
            {
                if (walker.enabled)
                {
                    animator.SetBool(isGrounded_hash, walker.isGrounded);
                    animator.SetFloat(velocityY_hash, walker.isGrounded ? 0 : walker.displacedPosition.y);
                }
            }
            if (jumper != null)
            {
                if (jumper.enabled)
                {
                    animator.SetBool(isGrounded_hash, jumper.isGrounded);
                    animator.SetFloat(velocityY_hash, jumper.isGrounded ? 0 : jumper.displacedPosition.y); 
                } 
            }
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
                {
                    if (goals[currentGoal].TimeLimitRange != Vector2Int.zero)
                        goals[currentGoal].TimeLimit = SetRandomRange(goals[currentGoal].TimeLimitRange);

                    currentGoalTimer = -1;
                }
                    

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

        public void DeviateWalk()
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

        public void DeviateFly()
        {
            isDeviating = true;
            if (flier.isStuck)
                flier.hasDeviatePosition = false;

            if (!flier.hasDeviatePosition)
                flier.FindDeviateDestination(30);


            flier.SetDirection();

            if (flier.CheckDistanceToDestination() <= 0.02f)
                isDeviating = false;

            flier.SetLastPosition();

        }

        void ResetCurrentGoal()
        {
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

        public DrawZasYDisplacement CheckForDisplacementSpot()
        {
            if (sleep.isSleeping)
                return null;
            closestSpots = interactAreas.QueryQuadTree(bounds);
            if (closestSpots.Count <= 0)
                return null;

            DrawZasYDisplacement bestTarget = null;
            float closestDistance = Mathf.Infinity;
            Vector2 currentPosition = transform.position;
            foreach (var item in closestSpots)
            {
                if (item == null || item.isInUse || !GridManager.instance.GetTileValid(item.transform.position))
                    continue;
                var dist = Vector2.Distance(currentPosition, item.transform.position);

                if (dist < closestDistance)
                {
                    if (dist < minHomeDistance)
                        continue;
                    closestDistance = dist;
                    bestTarget = item;
                }
            }

            if (bestTarget == null)
                return null;

            currentDisplacementSpot = bestTarget;
            currentDisplacementSpot.isInUse = true;

            return bestTarget;

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



        bool FoundFood()
        {
            if (!canEat || isEating || fleeing)
                return false;
            
            var hit = Physics2D.OverlapPoint(eatPoint.position, interactableLayer, transform.position.z, transform.position.z);
            if (hit != null)
            {
                if(hit.TryGetComponent(out QI_Item item))
                {
                    if(edibleItems.Contains(item.Data))
                    {
                        currentEdible = hit.gameObject;
                        isEating = true;
                        SetBeliefState("Eating", true);
                        return true;
                    }
                }
            }
            return false;
        }

        bool ScritchableFound()
        {

            if (!canScritch || isScritching)
                return false;

            scritchTimer -= Time.deltaTime;
            if (scritchTimer > 0)
                return false;

            currentScritchableItem = null;
            var hit = Physics2D.OverlapCircle(transform.position, 0.2f, scritchableLayer, transform.position.z, transform.position.z);
            if(hit != null)
            {
                var c = hit.gameObject.GetComponentsInChildren<DrawZasYDisplacement>();
                List<DrawZasYDisplacement> spots = new List<DrawZasYDisplacement>();
                foreach (var spot in c)
                {
                    if (spot.spotType == animalSpotType)
                        spots.Add(spot);
                }
                currentScritchablePosition = null;
                float dist = 100;
                for (int i = 0; i < spots.Count; i++)
                {
                    var d = Vector2.Distance(transform.position, spots[i].transform.position);
                    if (d < dist)
                    {
                        dist = d;
                        currentScritchableItem = hit.transform;
                        currentScritchablePosition = spots[i].transform;
                    }
                }
                if (currentScritchablePosition != null)
                {
                    isScritching = true;
                    SetBeliefState("Scritching", true);
                    return true;
                }
                    
            }
            return false;
        }

        public void FleePlayer(Transform playerTransform)
        {
            if(flier != null)
            {
                if (flier.enabled)
                    return;
            }
            fleeTransfrom = playerTransform;
            SetBeliefState("Flee", true);
        }

        public void SetActiveState(bool active)
        {
            
                

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

            if (!animator.GetBool(sleeping_hash))
            {
                if (PlayerInformation.instance.player.position.x < transform.position.x && walker.facingRight ||
                PlayerInformation.instance.player.position.x > transform.position.x && !walker.facingRight)
                    walker.Flip();
            }

        }


        public void OnTriggerEnter2D(Collider2D collision)
        {

            if (collision.transform.position.z != transform.position.z || !hasDialogue)
                return;

           
            
            if (collision.gameObject.CompareTag("Player"))
            {
                inTalkRange = true;
                animator.SetBool(walking_hash, false);
                walker.currentDirection = Vector2.zero;
                if (!animator.GetBool(sleeping_hash))
                {
                    if (collision.transform.position.x < transform.position.x && walker.facingRight ||
                    collision.transform.position.x > transform.position.x && !walker.facingRight)
                        walker.Flip();
                }

            }
        }



        public void OnTriggerExit2D(Collider2D collision)
        {
            if (!hasDialogue)
                return;
            if (collision.gameObject.CompareTag("Player"))
                inTalkRange = false;
            
        }
    }
}
