using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GravitySystem
{
    [RequireComponent(typeof(DetectVisibility))]

    public class GravityItemNew : MonoBehaviour
    {
        // Gravity
        [Header("Gravity and Slope GameObjects")]
        public Transform itemObject;
        public Transform itemShadow;
        public Transform slopeObject;

        const float tileSize = 0.578125f;
        public const float gravity = 20f;
        protected const float spriteDisplacementY = 0.2990625f;
        [HideInInspector]
        public float positionZ;
        [HideInInspector]
        public Vector3 displacedPosition;
        [HideInInspector]
        public bool isGrounded;
        protected Transform _transform;

        [Header("Gravity Bounce")]
        [Range(0, 1)]
        public float bounceFriction;
        [Range(0, 10)]
        public float bounciness;
        float bounceFactor = 0;

        // Movement

        [HideInInspector]
        public AllTilesInfoManager allTilesManager;
        [HideInInspector]
        public CurrentTilePosition currentTilePosition;

        [HideInInspector]
        public float currentVelocity;
        //[HideInInspector]
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

        private void Awake()
        {
            currentTilePosition = GetComponent<CurrentTilePosition>();
            _transform = GetComponent<Transform>();
        }
        public virtual void Start()
        {

            allTilesManager = AllTilesInfoManager.instance;
            
            currentTilePosition.position = currentTilePosition.GetCurrentTilePosition(transform.position);
            
            currentLevel = currentTilePosition.position.z;
            
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

            //if (!onObstacle)
            //    obstacleDisplacement = Vector3.zero;
            itemShadow.localPosition = obstacleDisplacement;

            if (getOnSlope)
                ChangeLevelOnSlope();
            if (getOffSlope)
                ChangeLevelOffSlope();

            int dif = Mathf.Abs(currentTilePosition.position.z - currentLevel);
            if (dif != 0 && !onSlope && !getOnSlope && ! getOffSlope)
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
                Debug.LogError("Tile not found in tile dictionary!");
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

        public void ChangeLevel(int dif)
        {

            bounceFactor = 1;
            float displacement = dif * spriteDisplacementY;
            Vector3 currentPosition = _transform.position;

            if (currentTilePosition.position.z > currentLevel)
            {
                currentPosition = new Vector3(currentPosition.x, currentPosition.y + displacement, currentTilePosition.position.z + 1);
                _transform.position = currentPosition;
                positionZ += dif;
                displacedPosition = new Vector3(displacedPosition.x, displacedPosition.y - displacement, displacedPosition.z - dif);
                itemObject.localPosition = new Vector3(itemObject.localPosition.x, itemObject.localPosition.y - displacement, itemObject.localPosition.z - dif);

            }
            else if (currentTilePosition.position.z < currentLevel)
            {
                currentPosition = new Vector3(currentPosition.x, currentPosition.y - displacement, currentTilePosition.position.z + 1);
                _transform.position = currentPosition;
                positionZ -= dif;
                displacedPosition = new Vector3(displacedPosition.x, displacedPosition.y + displacement, displacedPosition.z + dif);
                itemObject.localPosition = new Vector3(itemObject.localPosition.x, itemObject.localPosition.y + displacement, itemObject.localPosition.z + dif);

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

            }

            float displacementY = slopeDisplacement * spriteDisplacementY;

            slopeObject.localPosition = new Vector3(0, displacementY, slopeDisplacement);
        }

        //void HandleObstacles()
        //{
        //    slopeObject.localPosition = obstacleDisplacement;
        //    //var obstacleHit = Physics2D.OverlapPoint(transform.position, obstacleLayer, transform.position.z, transform.position.z);
        //    //if (obstacleHit != null)
        //    //{
        //    //    if (obstacleHit.TryGetComponent(out DrawZasYDisplacement displacement))
        //    //    {
        //    //        float displacementY = displacement.positionZ * spriteDisplacementY;
        //    //        slopeObject.localPosition = new Vector3(0, displacementY, displacement.positionZ);
        //    //    }
        //    //}


        //}

        


        public void Move(Vector2 dir, float velocity)
        {
            currentDirection = dir;
            currentVelocity = velocity;


            Vector3 currentPosition = _transform.position;
            currentPosition = Vector2.MoveTowards(_transform.position, (Vector2)_transform.position + dir, Time.deltaTime* currentVelocity);
            currentPosition.z = currentTilePosition.position.z + 1;
            _transform.position = currentPosition;

        }
        

        public void MoveZ(Vector3 dir, float speed)
        {

            Vector3 currentPosition = itemObject.localPosition;
            currentPosition = Vector3.MoveTowards(itemObject.localPosition, itemObject.localPosition + dir, Time.deltaTime * speed);

            itemObject.localPosition = currentPosition;

            Vector3 screenPos = Camera.main.WorldToScreenPoint(_transform.position).normalized;
            var totalZ = (itemObject.localPosition.z - screenPos.y) / 10;
            itemObject.localScale = new Vector3(1 + totalZ, 1 + totalZ, 1);

        }



        public void Nudge(Vector2 dir)
        {
            Vector3 currentPosition = _transform.position;
            currentPosition += (Vector3)dir * checkTileDistance;

            _transform.position = currentPosition;
        }
        

        public bool CheckForObstacles(Vector3 checkPosition, Vector3 doubleCheck, Vector2 direction, Vector3Int nextTileKey)
        {
            //onObstacle = false;
            
            // Check for a positive gameobject on the obstacle layer
            hit = Physics2D.OverlapCircleAll(checkPosition, 0.01f, obstacleLayer, _transform.position.z, _transform.position.z);
            if (hit != null)
            {
                bool isColliding = false;
                // do it got a thing
                for (int i = 0; i < hit.Length; i++)
                {
                    if (hit[i].TryGetComponent(out DrawZasYDisplacement displacement))
                    {
                        // is our local z higher than the thing
                        
                        if (Mathf.Abs(itemObject.localPosition.z) < displacement.positionZ)
                            isColliding = true;
                        else
                        {
                            onObstacle = true;
                            float displacementY = displacement.positionZ * spriteDisplacementY;
                            obstacleDisplacement = new Vector3(0, displacementY, displacement.positionZ);
                        }
                    }
                }



                doubleHit = Physics2D.OverlapPoint(doubleCheck, obstacleLayer, transform.position.z, transform.position.z);
                if (doubleHit == null)
                {
                    //onObstacle = false;
                    obstacleDisplacement = Vector3.zero;
                }


                return isColliding/* && !onObstacle*/;
            }
            return false;
        }

        protected void ApplyGravity()
        {
            positionZ -= gravity * Time.fixedDeltaTime;

            displacedPosition = new Vector3(0, spriteDisplacementY * positionZ, positionZ);
            
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
            float dist = Vector2.Distance(itemObject.localPosition, obstacleDisplacement);
            
            isGrounded = dist <= 0.01f;
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
            
            //itemObject.transform.Translate(displacedPosition * Time.fixedDeltaTime);
            bounceFactor *= bounceFriction;
            ApplyGravity();
        }



    }

}
