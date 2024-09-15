using Klaxon.GravitySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        InteractAreasManager interactAreas;

        public bool isNocturnal;

        [HideInInspector]
        public UIScreenManager sleep;
        [HideInInspector]
        public Bounds bounds;
        [HideInInspector]
        public List<DrawZasYDisplacement> closestSpots = new List<DrawZasYDisplacement>();
        public float minHomeDistance = 1f;
        [HideInInspector]
        public DrawZasYDisplacement currentDisplacementSpot;

        public DrawZasYDisplacement homeDestination;


        /// <summary>
        /// Fliers
        /// </summary>
        public Vector2 minMaxFlap;
        public Vector2 minMaxGlide;
        [HideInInspector]
        public bool glide;

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
            interactAreas = GetComponentInParent<InteractAreasManager>();
            sounds = GetComponent<AnimalSounds>();
            musicItem = GetComponentInChildren<MusicGeneratorItem>();
            bounds = new Bounds(transform.position, new Vector3(4, 4, 4));
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

            if (flier.CheckDistanceToDestination() <= 0.02f)
                isDeviating = false;

            flier.SetLastPosition();

        }

        public void HandleOffScreen(GOAD_Action action)
        {

        }
        public DrawZasYDisplacement CheckForDisplacementSpot()
        {

            if (sleep.isSleeping || interactAreas == null)
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

        public void SetBoidsState(bool isInBoids)
        {
            if (flier.boid != null)
            {
                flier.useBoids = isInBoids;
                flier.boid.inBoidPool = isInBoids;
            }

        }



        public void FleePlayer(Transform playerTransform)
        {
            
        }

        public void SetActiveState(bool active)
        {
            
        }
    } 
}
