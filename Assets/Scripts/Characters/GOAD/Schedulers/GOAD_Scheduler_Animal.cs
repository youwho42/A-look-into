using Klaxon.GravitySystem;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Klaxon.ConversationSystem;

namespace Klaxon.GOAD
{
	public class GOAD_Scheduler_Animal : GOAD_Scheduler, IAnimal
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

        [HideInInspector]
        public AnimalSounds sounds;
        [HideInInspector]
        public MusicGeneratorItem musicItem;

        [HideInInspector]
        public bool offScreen;

        [HideInInspector]
        public GravityItemWalk walker;
        [HideInInspector]
        public GravityItemFly flier;
        [HideInInspector]
        public GravityItemJump jumper;
        [HideInInspector]
        public bool isDeviating;

        public MusicGeneratorItem musicGeneratorItem;
        public bool removeFromMusicAtHome;
        public bool hiddenWhenSleeping;
        [ConditionalHide("hiddenWhenSleeping", true)]
        public GatherableItem gatherableItem;

        

        public bool isNocturnal;

        [HideInInspector]
        public UIScreenManager sleep;
        [HideInInspector]
        public Bounds bounds;
        [HideInInspector]
        public List<DrawZasYDisplacement> closestSpots = new List<DrawZasYDisplacement>();
        public float minHomeDistance = 1f;
        public float maxHomeDistance = 5f;
        [HideInInspector]
        public DrawZasYDisplacement currentDisplacementSpot;

        public DrawZasYDisplacement homeDestination;

        public bool shouldFlee;
        [ConditionalHide("shouldFlee", true)]
        public GOAD_ScriptableCondition fleeCondition;

        public LayerMask displacementSpotLayer;
        public SpotType spotType;
        /// <summary>
        /// Fliers
        /// </summary>
        public Vector2 minMaxFlap;
        public Vector2 minMaxGlide;
        [HideInInspector]
        public bool glide;

        public Vector2 headTimeRange;



        [Header("Eating")]
        public bool canEat;
        [ConditionalHide("canEat", true)]
        public Transform eatPoint;
        [HideInInspector]
        public float xOffset;
        public List<QI_ItemData> edibleItems = new List<QI_ItemData>();
        [HideInInspector]
        public bool isEating;
        [HideInInspector]
        public GameObject currentEdible;
        int maxEatAmount = 100;
        int currentEatAmount;
        [ConditionalHide("canEat", true)]
        public GOAD_ScriptableCondition hungryCondition;
        [ConditionalHide("canEat", true)]
        public GOAD_ScriptableCondition foodFoundCondition;


        [Header("Scritching")]
        public bool canScritch;
        [HideInInspector]
        public bool isScritching;
        [HideInInspector]
        public TreeRustling currentScritchableTree;
        [ConditionalHide("canScritch", true)]
        public GOAD_ScriptableCondition wantScritchCondition;
        [ConditionalHide("canScritch", true)]
        public GOAD_ScriptableCondition scritchableFoundCondition;


        public bool hasDialogue;
        bool inTalkRange;
        float inTalkRangeTimer;
        DialogueManagerUI dialogueManager;

        [HideInInspector]
        public Transform fleeTransform;

        public override void Start()
        {
            base.Start();
            sleep = UIScreenManager.instance;

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
            bounds = new Bounds(transform.position, new Vector3(4, 4, 4));

            dialogueManager = DialogueManagerUI.instance;


            currentEatAmount = maxEatAmount;
            if (canEat)
            {
                GameEventManager.onTimeTickEvent.AddListener(RemoveEatAmount);
                SetXOffset();
            }
            if(canScritch)
                GameEventManager.onTimeTickEvent.AddListener(TryScritch);
            
            

        }
       

        private void OnDestroy()
        {
            if (canEat)
                GameEventManager.onTimeTickEvent.RemoveListener(RemoveEatAmount);
            if (canScritch)
                GameEventManager.onTimeTickEvent.RemoveListener(TryScritch);

           
        }
       

        void SetXOffset()
        {
            
            if (walker != null)
                xOffset = Mathf.Abs(walker.itemObject.localPosition.x - eatPoint.localPosition.x);
            else if (flier != null)
                xOffset = Mathf.Abs(flier.itemObject.localPosition.x - eatPoint.localPosition.x);
            else if (jumper != null)
                xOffset = Mathf.Abs(jumper.itemObject.localPosition.x - eatPoint.localPosition.x);
        }
        void RemoveEatAmount(int tick)
        {
            currentEatAmount--;
            currentEatAmount = Mathf.Clamp(currentEatAmount, 0, maxEatAmount);
            if (currentEatAmount <= maxEatAmount * 0.1f)
                SetBeliefState(hungryCondition.Condition, true);
        }

        public void AddEatAmount()
        {
            currentEatAmount += 30;
            if (currentEatAmount >= maxEatAmount * 0.9f)
                SetBeliefState(hungryCondition.Condition, false);
        }

        void TryScritch(int tick)
        {
            if (isScritching)
                return;
            if(Random.Range(0.0f, 1.0f) <= 0.018f)
                SetBeliefState(wantScritchCondition.Condition, true);

        }

        private void Update()
        {

            if (inTalkRange && hasDialogue)
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
            
            if (canEat && IsConditionMet(hungryCondition) && !isEating)
                FindFood();

            if (canScritch && IsConditionMet(wantScritchCondition) && !isScritching)
                FindScritchable();
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

        public int SetRandomRange(Vector2Int minMaxRange)
        {
            return Random.Range(minMaxRange.x, minMaxRange.y);
        }

        public float SetRandomRange(Vector2 minMaxRange)
        {
            return Random.Range(minMaxRange.x, minMaxRange.y);
        }

        public void DeviateFly()
        {
            isDeviating = true;
            if (flier.isStuck)
                flier.hasDeviatePosition = false;

            if (!flier.hasDeviatePosition)
                flier.FindDeviateDestination(30);


            flier.SetDirection();
            flier.SetDirectionZ();
            if (flier.CheckDistanceToDestination() <= 0.02f)
                isDeviating = false;

            flier.SetLastPosition();

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




        public void HandleOffScreen(GOAD_Action action)
        {

        }

        public DrawZasYDisplacement CheckForDisplacementSpot()
        {
            if (sleep.isSleeping)
                return null;
            int waterdisp = 0;
            if(flier != null)
                waterdisp = flier.enabled && flier.isOverWater ? 3 : 0; 
            
            closestSpots.Clear();
            var spots = Physics2D.OverlapCircleAll(_transform.position, maxHomeDistance, displacementSpotLayer, _transform.position.z + waterdisp, _transform.position.z + waterdisp);
            foreach (var spot in spots)
            {
                var allDisplacementSpots = spot.GetComponentsInChildren<DrawZasYDisplacement>().Where(s => s.spotType == spotType).ToList();
                closestSpots.AddRange(allDisplacementSpots);
            }

            float closestDistanceSqr = Mathf.Infinity;
            Vector2 currentPosition = _transform.position;
            DrawZasYDisplacement bestTarget = null;

            foreach (var item in closestSpots)
            {
                var hit = Physics2D.OverlapPoint(item.transform.position, LayerMask.GetMask("Obstacle"), item.transform.position.z, item.transform.position.z);
                // Skip if item is null, in use, or position is invalid
                if (item == null || item.isInUse || !GridManager.instance.GetTileValid(item.transform.position) || hit != null)
                    continue;

               
                // Calculate squared distance to avoid sqrt overhead
                float distSqr = (currentPosition - (Vector2)item.transform.position).sqrMagnitude;

                // Skip if it's too close to the home distance or not the closest
                if (distSqr < minHomeDistance * minHomeDistance || distSqr >= closestDistanceSqr)
                    continue;

                closestDistanceSqr = distSqr;
                bestTarget = item;
            }

            if (bestTarget == null)
                return null;

            currentDisplacementSpot = bestTarget;
            currentDisplacementSpot.isInUse = true;

            return bestTarget;
        }

        


        public float TurnHead()
        {
            animator.SetTrigger(idle_hash);
            return SetRandomRange(headTimeRange);
        }

        public void SetBoidsState(bool isInBoids)
        {
            if (flier.boid != null)
            {
                flier.useBoids = isInBoids;
                flier.boid.inBoidPool = isInBoids;
            }
        }

        public void SetMusic(bool state)
        {
            if (!removeFromMusicAtHome || musicGeneratorItem == null)
                return;
            if (!state)
            {
                musicGeneratorItem.RemoveFromDictionary();
                musicGeneratorItem.isActive = false;
            }
            else
            {
                musicGeneratorItem.isActive = true;
                musicGeneratorItem.AddToDictionary();
            }
        }

        public void SetHidden(bool state)
        {
            if (!hiddenWhenSleeping || gatherableItem == null)
                return;
            gatherableItem.hasBeenHarvested = state;
        }

        void InteruptCurrentGoal()
        {
            currentAction.success = false;
            SetActionComplete(true);
            currentAction.EndAction(this);
            ResetGoal();
        }

        void FindFood()
        {
            
            var hit = Physics2D.OverlapCircle(eatPoint.position, 2f, LayerMask.GetMask("Interactable"), _transform.position.z, _transform.position.z);
            if (hit != null)
            {
                if (hit.TryGetComponent(out QI_Item item))
                {
                    if (edibleItems.Contains(item.Data))
                    {
                        currentEdible = hit.gameObject;
                        isEating = true;
                        SetBeliefState(foodFoundCondition.Condition, true);
                        InteruptCurrentGoal();
                        return;
                    }
                }
            }
            SetBeliefState(foodFoundCondition.Condition, false);
        }


        void FindScritchable()
        {
            currentScritchableTree = null;
            var hits = Physics2D.OverlapCircleAll(_transform.position, 2f, LayerMask.GetMask("Gatherable"), _transform.position.z, _transform.position.z);
            
            foreach (var item in hits)
            {
                var allDisplacementSpots = item.GetComponentsInChildren<DrawZasYDisplacement>().Where(s => s.spotType == SpotType.Bear).ToList();
                if (allDisplacementSpots.Count > 0)
                {
                    currentDisplacementSpot = allDisplacementSpots[0];
                    currentScritchableTree = item.gameObject.GetComponent<TreeRustling>();
                    isScritching = true;
                    SetBeliefState(scritchableFoundCondition.Condition, true);
                    InteruptCurrentGoal();
                    break;
                }
                        
            }


            
            
        }

        public void FleePlayer(Transform interactorTransform)
        {
            if (!shouldFlee && IsConditionMet(fleeCondition))
                return;


            fleeTransform = interactorTransform;
            SetBeliefState(fleeCondition.Condition, true);
            if(currentAction != null)
                InteruptCurrentGoal();

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
                if (PlayerInformation.instance.player.position.x < _transform.position.x && walker.facingRight ||
                PlayerInformation.instance.player.position.x > _transform.position.x && !walker.facingRight)
                    walker.Flip();
            }

        }


        public void OnTriggerEnter2D(Collider2D collision)
        {

            if (collision.transform.position.z != _transform.position.z || !hasDialogue)
                return;



            if (collision.gameObject.CompareTag("Player"))
            {
                inTalkRange = true;
                animator.SetBool(walking_hash, false);
                walker.currentDirection = Vector2.zero;
                if (!animator.GetBool(sleeping_hash))
                {
                    if (collision.transform.position.x < _transform.position.x && walker.facingRight ||
                    collision.transform.position.x > _transform.position.x && !walker.facingRight)
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
