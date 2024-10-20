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
            Villager
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
        #region IndicatorBP
        [HideInInspector]
        public int indicatorIndex;
        [HideInInspector]
        public bool justIndicated;
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
                    DissolveEffect.instance.StartDissolve(allSprites[i].material, 1f, disolveIn);
            }
            
        }


        public bool CheckNearPlayer(float maxDistance)
        {
            float dist = (_transform.position - player.player.position).sqrMagnitude;

            return dist <= maxDistance * maxDistance;
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
                var dist = Vector3.Distance(_transform.position, BPHomeDestination);
                if (dist < 1.8f)
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
