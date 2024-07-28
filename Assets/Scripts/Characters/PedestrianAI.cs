using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GravitySystem
{
    public class PedestrianAI : MonoBehaviour, IAnimal
    {

        float timeToStayAtDestination;

        public InteractAreasManager interactAreas;
        float detectionTimeOutTimer;
        GravityItemNew gravityItem;
        CanReachTileWalk walk;
        public Animator animator;
        float idleTimer;
        bool justClimbed;

        TreeRustling currentClimable;


        static int walking_hash = Animator.StringToHash("IsWalking");
        static int idle_hash = Animator.StringToHash("Idle");
        static int climbing_hash = Animator.StringToHash("IsClimbing");
        static int climbIdle_hash = Animator.StringToHash("ClimbIdle");
        static int isGrounded_hash = Animator.StringToHash("IsGrounded");
        static int velocityY_hash = Animator.StringToHash("VelocityY");

        bool activeState = true;

        DrawZasYDisplacement currentClimbingSpot;
        float detectionTimeOutAmount;
        SpriteRenderer animalSprite;
        [SerializeField]
        public PedestrianState currentState;
        public enum PedestrianState
        {
            isWalking,
            isAtDestination,
            isGoingToClimbSpot,
            isClimbing

        }

        private void Start()
        {

            gravityItem = GetComponent<GravityItemNew>();

            walk = GetComponent<CanReachTileWalk>();
            idleTimer = SetRandomRange(3, 10);
            timeToStayAtDestination = SetRandomRange(new Vector2(5.0f, 15.0f));
            animalSprite = gravityItem.itemObject.GetComponent<SpriteRenderer>();
        }

        bool CheckVisibility()
        {
            return animalSprite.isVisible;
        }

        private void Update()
        {
            if (!CheckVisibility())
            {
                if (Time.frameCount % 20 != 0)
                    return;
            }
            if (!activeState)
                return;

            switch (currentState)
            {
                case PedestrianState.isWalking:
                    animator.SetBool(walking_hash, true);
                    walk.Walk();



                    if (justClimbed)
                    {
                        if (currentClimbingSpot != null)
                        {
                            currentClimbingSpot.isInUse = false;
                            currentClimbingSpot = null;
                            currentClimable = null;
                        }
                        detectionTimeOutTimer += Time.deltaTime;
                        if (detectionTimeOutTimer >= detectionTimeOutAmount)
                        {
                            justClimbed = false;

                            detectionTimeOutTimer = 0;
                        }
                    }

                    if (walk.canClimb && !justClimbed && CheckVisibility())
                        CheckForClimbingArea();


                    if (Vector2.Distance(transform.position, walk.currentDestination) <= 0.01f)
                    {
                        timeToStayAtDestination = SetRandomRange(new Vector2(5.0f, 15.0f));
                        animator.SetBool(walking_hash, false);


                        currentState = PedestrianState.isAtDestination;

                    }
                    break;


                case PedestrianState.isAtDestination:
                    animator.SetBool(walking_hash, false);
                    idleTimer -= Time.deltaTime;
                    if (idleTimer <= 0)
                    {

                        idleTimer = SetRandomRange(0.2f, 5.0f);
                        animator.SetTrigger(idle_hash);
                    }

                    timeToStayAtDestination -= Time.deltaTime;


                    if (timeToStayAtDestination <= 0)
                    {
                        if (walk.isClimbing)
                        {
                            walk.ResetDestinationZ();
                            SetDirectionZ();
                            currentClimable.Affect(false);
                            animator.SetBool(climbIdle_hash, false);

                            currentState = PedestrianState.isClimbing;
                        }
                        else
                        {

                            walk.SetRandomDestination();
                            animator.SetBool(walking_hash, true);
                            currentState = PedestrianState.isWalking;
                        }


                    }



                    break;

                case PedestrianState.isGoingToClimbSpot:
                    animator.SetBool(walking_hash, true);
                    walk.Walk();
                    if (Vector2.Distance(transform.position, walk.currentDestination) <= 0.01f)
                    {
                        if (!justClimbed)
                        {
                            CheckCurrentTree();
                            if (currentClimable == null)
                            {
                                walk.SetRandomDestination();
                                animator.SetBool(walking_hash, true);
                                currentState = PedestrianState.isWalking;
                                break;
                            }
                            justClimbed = true;
                            walk.SetDestinationZ(currentClimbingSpot);

                            walk.isClimbing = true;

                            var dir = currentClimable.transform.position - transform.position;
                            var d = Mathf.Sign(dir.x);
                            walk.characterRenderer.flipX = d > 0;
                            gravityItem.isWeightless = true;
                            animator.SetBool(climbing_hash, true);
                            animator.SetBool(walking_hash, false);
                            currentState = PedestrianState.isClimbing;
                        }

                    }
                    break;



                case PedestrianState.isClimbing:

                    animator.SetBool(climbing_hash, true);
                    animator.SetBool(walking_hash, false);
                    walk.Walk();
                    if (Vector2.Distance(gravityItem.itemObject.localPosition, walk.currentDestinationZ) <= 0.01f)
                    {


                        if (walk.currentDestinationZ.z == 0)
                        {
                            timeToStayAtDestination = SetRandomRange(new Vector2(0.1f, 2.0f));
                            walk.isClimbing = false;
                            gravityItem.isWeightless = false;
                            walk.characterRenderer.flipY = false;
                            var dir = currentClimable.transform.position - transform.position;
                            var d = Mathf.Sign(dir.x);
                            walk.characterRenderer.flipX = d < 0;
                            animator.SetBool(climbing_hash, false);
                        }
                        else
                        {
                            animator.SetBool(climbIdle_hash, true);
                            timeToStayAtDestination = SetRandomRange(new Vector2(5.0f, 30.0f));
                            currentClimable.Affect(false);
                        }


                        animator.SetBool(walking_hash, false);
                        currentState = PedestrianState.isAtDestination;
                    }
                    break;


            }
        }

        void LateUpdate()
        {
            animator.SetBool(isGrounded_hash, gravityItem.isGrounded);
            animator.SetFloat(velocityY_hash, gravityItem.isGrounded ? 0 : gravityItem.displacedPosition.y);

        }
        void SetDirectionZ()
        {
            var dir = Mathf.Sign(walk.currentDirection.x);
            var dirZ = Mathf.Sign(walk.currentDirectionZ.z);
            walk.characterRenderer.flipY = dirZ < 0;
        }


        float SetRandomRange(Vector2 minMaxRange)
        {
            return Random.Range(minMaxRange.x, minMaxRange.y);
        }

        float SetRandomRange(float min, float max)
        {
            return Random.Range(min, max);
        }
        void CheckCurrentTree()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, .1f);
            if (hits.Length > 0)
            {
                foreach (var hit in hits)
                {
                    if (hit.TryGetComponent(out TreeRustling tree))
                    {
                        currentClimable = tree;
                    }

                }
            }
        }
        void CheckForClimbingArea()
        {
            currentClimbingSpot = null;
            if (interactAreas == null)
                return;
            DrawZasYDisplacement bestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector2 currentPosition = transform.position;
            foreach (var item in interactAreas.allAreas)
            {
                if (item.isInUse || item.transform.position.z != transform.position.z)
                    continue;
                Vector2 directionToTarget = (Vector2)item.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = item;
                }
            }

            if (bestTarget == null)
                return;

            walk.SetDestination(bestTarget);

            detectionTimeOutAmount = SetRandomRange(5, 30);
            currentClimbingSpot = bestTarget;
            animator.SetBool(walking_hash, true);
            currentClimbingSpot.isInUse = true;
            currentState = PedestrianState.isGoingToClimbSpot;


        }

        public void SetActiveState(bool active)
        {
            activeState = active;
        }

        public void FleePlayer(Transform playerTransform)
        {
            if (walk.isClimbing)
            {
                walk.ResetDestinationZ();
                SetDirectionZ();
                currentClimable.Affect(false);
                animator.SetBool(climbIdle_hash, false);

                currentState = PedestrianState.isClimbing;
            }
            else
            {

                walk.SetRandomDestination();
                animator.SetBool(walking_hash, true);
                currentState = PedestrianState.isWalking;
            }
        }
    }
}
