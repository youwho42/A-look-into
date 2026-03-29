using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klaxon.Interactable;

namespace Klaxon.GravitySystem
{
    
    public class GravityItemNew : MonoBehaviour
    {
        // Gravity
        [Header("Gravity and Slope GameObjects")]
        public Transform itemObject;
        public Transform itemShadow;
        public Transform slopeObject;

        protected float tileSize = GlobalSettings.TileSize;
        public const float gravity = 20f;
        protected float spriteDisplacementY = GlobalSettings.SpriteDisplacementY;
        [HideInInspector]
        public float positionZ;
        [HideInInspector]
        public Vector3 displacedPosition;
        
        [HideInInspector]
        public int lastJumpZ;
        [HideInInspector]
        public bool isGrounded;
        protected Transform _transform;

        protected bool canJump;

        [Header("Gravity Bounce")]
        [Range(0, 1)]
        public float bounceFriction;
        [Range(0, 10)]
        public float bounciness;
        [HideInInspector]
        public float bounceFactor = 0;

        // Movement

        [HideInInspector]
        public AllTilesInfoManager allTilesManager;
        [HideInInspector]
        public CurrentTilePosition currentTilePosition;

        [HideInInspector]
        public float currentVelocity;
        [HideInInspector]
        public Vector2 currentDirection;
        [HideInInspector]
        public int currentLevel;

        // surroundings
        [Header("Obstacles")]
        public LayerMask obstacleLayer;

        // slopes
        [HideInInspector]
        public bool getOnSlope;
        [HideInInspector]
        public bool getOffSlope;
        [HideInInspector]
        public bool onSlope;
        [HideInInspector]
        public float slopeDisplacement;
        [HideInInspector]
        public Vector2 slopeCollisionPoint;
        [HideInInspector]
        public Vector2 slopeCheckPosition;
        [HideInInspector]
        public Vector2 slopeDirection;

        //[HideInInspector]
        public bool isOverWater;

        [Header("Slope Detection")]
        [Range(0.0f, 0.1f)]
        public float checkTileDistance = 0.08f;
        public LayerMask groundLayer;

        public bool isWeightless;
        [HideInInspector]
        public List<TileDirectionInfo> tileBlockInfo;

        [HideInInspector]
        public bool onObstacle;
        [HideInInspector]
        public Vector3 obstacleDisplacement;

        Collider2D[] hit;
        Collider2D doubleHit;


        public bool facingRight;
        protected Vector2 collisionNormal;

       

        public virtual void Awake()
        {
            currentTilePosition = GetComponent<CurrentTilePosition>();
            _transform = GetComponent<Transform>();
        }
        public virtual void Start()
        {

            allTilesManager = AllTilesInfoManager.instance;
            
            currentTilePosition.position = currentTilePosition.GetCurrentTilePosition(transform.position);
            
            currentLevel = currentTilePosition.position.z;

            canJump = true;
        }
       
        public virtual void Update()
        {
            
            if (!isWeightless)
                SetIsGrounded();

            if (onSlope)
            {
                HandleSlopes();
            }
            else
            {
                slopeDisplacement = 0;
                slopeObject.localPosition = Vector3.zero;
            }

            
            itemShadow.localPosition = obstacleDisplacement;

            if (getOnSlope)
                ChangeLevelOnSlope();
            if (getOffSlope)
                ChangeLevelOffSlope();
            int dif = Mathf.Abs(currentTilePosition.position.z - currentLevel);
            if (dif != 0 && !onSlope && !getOnSlope && !getOffSlope)
                ChangeLevel(dif);


        }
        
        public virtual void FixedUpdate()
        {
            if (!isGrounded && !isWeightless)
                ApplyGravity();
        }

        public void TileFound(List<TileDirectionInfo> tileBlock, bool success)
        {
            
            if (success)
                tileBlockInfo = tileBlock;
            else
                Debug.LogWarning("Tile not found in tile dictionary!", this);
        }


        public void ChangeLevelOffSlope()
        {

            int dif = currentTilePosition.position.z - currentLevel;

            float displacement = dif * spriteDisplacementY;
            Vector3 currentPosition = _transform.position;
            slopeObject.localPosition = Vector3.zero;
            currentPosition = new Vector3(currentPosition.x, currentPosition.y + displacement, currentTilePosition.position.z + 1);
            _transform.position = currentPosition;

            currentLevel = currentTilePosition.position.z;
            getOffSlope = false;

        }

        public void ChangeLevelOnSlope()
        {

            int dif = currentTilePosition.position.z - currentLevel;

            float displacement = dif * spriteDisplacementY;
            Vector3 currentPosition = _transform.position;
            slopeObject.localPosition = new Vector3(0, spriteDisplacementY, 1);
            currentPosition = new Vector3(currentPosition.x, currentPosition.y + displacement, currentTilePosition.position.z + 1);
            _transform.position = currentPosition;

            currentLevel = currentTilePosition.position.z;
            getOnSlope = false;
        }

        public void ChangeLevel(float dif)
        {
            
            bounceFactor = 1;
            float displacement = dif * spriteDisplacementY;
            Vector3 currentPosition = _transform.position;
            
            if (currentTilePosition.position.z > currentLevel)
            {
                
                currentPosition = new Vector3(currentPosition.x, currentPosition.y + displacement, currentTilePosition.position.z + 1);
                _transform.position = currentPosition;
                displacedPosition = new Vector3(displacedPosition.x, displacedPosition.y - displacement, displacedPosition.z - dif);
                itemObject.localPosition = new Vector3(itemObject.localPosition.x, itemObject.localPosition.y - displacement, itemObject.localPosition.z - dif);

            }
            else if (currentTilePosition.position.z < currentLevel)
            {
                currentPosition = new Vector3(currentPosition.x, currentPosition.y - displacement, currentTilePosition.position.z + 1);
                _transform.position = currentPosition;
                displacedPosition = new Vector3(displacedPosition.x, displacedPosition.y + displacement, displacedPosition.z + dif);
                itemObject.localPosition = new Vector3(itemObject.localPosition.x, itemObject.localPosition.y + displacement, itemObject.localPosition.z + dif);

            }
            else if (currentTilePosition.position.z == currentLevel)
            {
                displacedPosition = new Vector3(displacedPosition.x, displacedPosition.y + displacement, displacedPosition.z + dif);
                itemObject.localPosition = new Vector3(itemObject.localPosition.x, displacement, dif);

            }
            currentLevel = currentTilePosition.position.z;

        }

        public void HandleSlopes()
        {
            
            slopeCheckPosition = (Vector2)_transform.position - (slopeDirection * 0.6f);
            var hitA = Physics2D.Raycast(slopeCheckPosition, slopeDirection, 0.6f, groundLayer);
            
            if (hitA.collider != null)
            {
                slopeCollisionPoint = hitA.point;
                float distA = Vector2.Distance(hitA.point, _transform.position);
                slopeDisplacement = distA / tileSize;
                slopeDisplacement = Mathf.Clamp(slopeDisplacement, 0.0f, 1.0f);
            }

            float displacementY = slopeDisplacement * spriteDisplacementY;

            slopeObject.localPosition = new Vector3(0, displacementY, slopeDisplacement);
        }


        public void Move(Vector2 dir, float velocity)
        {
            currentDirection = dir;
            currentVelocity = dir == Vector2.zero ? 0 : velocity;


            Vector3 currentPosition = _transform.position;
            currentPosition = Vector2.MoveTowards(_transform.position, (Vector2)_transform.position + dir, Time.deltaTime * currentVelocity);
            currentPosition.z = currentTilePosition.position.z + 1;
            _transform.position = currentPosition;

        }
        

        public void MoveZ(Vector3 dir, float speed)
        {

            Vector3 currentPosition = itemObject.localPosition;
            currentPosition = Vector3.MoveTowards(itemObject.localPosition, itemObject.localPosition + dir, Time.deltaTime * speed);

            itemObject.localPosition = currentPosition;

            Vector3 screenPos = Camera.main.WorldToScreenPoint(_transform.position).normalized;
            var currentZ = isOverWater ? itemObject.localPosition.z - 3 : itemObject.localPosition.z;
            
            var totalZ = (currentZ - screenPos.y) / 10;
            itemObject.localScale = new Vector3(1 + totalZ, 1 + totalZ, 1);

        }



        public void Nudge(Vector2 dir)
        {
            Vector3 currentPosition = _transform.position;
            currentPosition += (Vector3)dir * checkTileDistance;

            _transform.position = currentPosition;
        }


        public bool CheckForWaterAbove(Vector3Int tilePosition)
        {
            for (int i = tilePosition.z + 1; i < 10; i++)
            {
                var pos = new Vector3Int(tilePosition.x, tilePosition.y, i);
                if (GridManager.instance.HasWaterTile(pos))
                    return true;
                
            }
            
            return false;
        }

        public bool CheckForObstacles(Vector3 checkPosition, Vector3 doubleCheck, Vector2 direction, Vector3Int nextTileKey, bool getBounceNormal = false)
        {
            // Check for a positive gameobject on the obstacle layer
            hit = Physics2D.OverlapCircleAll(checkPosition, 0.01f, obstacleLayer, _transform.position.z, _transform.position.z);
            if (hit != null)
            {
                Vector3 highestDisplacement = Vector3.zero; ;
                bool isColliding = false;
                // do it got a thing
                for (int i = 0; i < hit.Length; i++)
                {
                    

                    if (hit[i].TryGetComponent(out DrawZasYDisplacement displacement))
                    {
                        if (displacement.positionZ == 0)
                            continue;
                        if (itemObject.localPosition.z == 0)
                        {
                            InteractableDoor door = hit[i].GetComponentInParent<InteractableDoor>();
                            if (door != null)
                            {
                                if (!door.isOpen)
                                {
                                    door.Interact(this.gameObject);
                                    return false;
                                }

                            }

                        }
                        // is our local z higher than the thing
                       
                        if (Mathf.Abs(itemObject.localPosition.z) < displacement.positionZ)
                        {
                            if (getBounceNormal)
                            {
                                RaycastHit2D hit = Physics2D.Raycast(_transform.position, direction, 0.2f, obstacleLayer);
                                if (hit)
                                    collisionNormal = hit.normal;
                            }
                            isColliding = true;
                        }
                        else
                        {
                            onObstacle = true;
                            if(displacement.displacedPosition.z > highestDisplacement.z)
                                highestDisplacement = displacement.displacedPosition;
                        }
                    }
                }

                obstacleDisplacement = highestDisplacement;

                doubleHit = Physics2D.OverlapPoint(doubleCheck, obstacleLayer, transform.position.z, transform.position.z);
                if (doubleHit == null)
                    obstacleDisplacement = Vector3.zero;
                


                return isColliding;
            }
            return false;
        }

        protected void ApplyGravity()
        {
            float gravityMultiplier = canJump ? 1 : 2;
            positionZ -= gravity * gravityMultiplier * Time.fixedDeltaTime;

            
            displacedPosition = new Vector3(0, spriteDisplacementY * positionZ, positionZ);
            float dispZ = itemObject.localPosition.z - obstacleDisplacement.z;
            
            
            itemObject.Translate(displacedPosition * Time.fixedDeltaTime);
            SetIsGrounded();

            if (itemObject.localPosition.y <= obstacleDisplacement.y)
            {
                
                positionZ = obstacleDisplacement.z;
                displacedPosition = obstacleDisplacement;
                itemObject.localPosition = obstacleDisplacement;
                JustLanded();
                
                if (bounceFactor >= .001f)
                    Bounce(bounciness * bounceFactor);

            }
        }

        protected void SetIsGrounded()
        {
            //Check and set if grounded
            // could check here for a maxFallHeight and have a stagger animation...
            float dist = NumberFunctions.GetDistanceV2(itemObject.localPosition, obstacleDisplacement);
            
            isGrounded = dist <= 0.0001f;
            if (isGrounded)
            {
                itemObject.localPosition = obstacleDisplacement;
                positionZ = obstacleDisplacement.z;
                displacedPosition = obstacleDisplacement;
            }
        }

        public virtual void JustLanded(){}


        public void Bounce(float bounceAmount)
        {
            positionZ = bounceAmount;
            displacedPosition = new Vector3(0, spriteDisplacementY * positionZ, positionZ);
            lastJumpZ = currentLevel;
            bounceFactor *= bounceFriction;
            ApplyGravity();
        }


        public void Flip()
        {
            // Switch the way the player is labelled as facing
            facingRight = !facingRight;

            // Multiply the player's x local scale by -1
            Vector3 theScale = _transform.localScale;
            theScale.x *= -1;
            _transform.localScale = theScale;
        }


    }

}
