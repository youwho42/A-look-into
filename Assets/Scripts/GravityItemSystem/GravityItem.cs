using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DetectVisibility))]
[RequireComponent(typeof(SurroundingTilesInfo))]

public class GravityItem : MonoBehaviour
{
    // Gravity
    [Header("Gravity and Slope GameObjects")]
    public Transform itemObject;
    public Transform itemShadow;
    public Transform slopeObject;

    const float tileSize = 0.578125f;
    const float gravity = 20f;
    protected const float spriteDisplacementY = 0.2790625f;
    [HideInInspector]
    public float positionZ;
    [HideInInspector]
    public Vector3 displacedPosition;
    [HideInInspector]
    public bool isGrounded;

    [Header("Gravity Bounce")]
    [Range(0, 1)]
    public float bounceFriction;
    [Range(0, 10)]
    public float bounciness;
    float bounceFactor = 0;

    // Movement
    
    [HideInInspector]
    public SurroundingTilesInfo surroundingTiles;
    [HideInInspector]
    public float currentVelocity;
    Vector2 currentDirection;
    protected int currentLevel;

    // surroundings
    [Header("Obstacles")]
    public LayerMask obstacleLayer;

    // slopes
    public bool getOnSlope;
    public bool getOffSlope;
    public bool onSlope;
    float slopeDisplacement;
    Vector2 slopeCollisionPoint;
    Vector2 slopeCheckPosition;
    public Vector2 slopeDirection;

    [Header("Slope Detection")]
    [Range(0.0f, 0.1f)]
    public float checkTileDistance = 0.08f;
    public LayerMask groundLayer;

    public bool isWeightless;


    public void Start()
    {
   
        surroundingTiles = GetComponent<SurroundingTilesInfo>();
        surroundingTiles.currentTilePosition = surroundingTiles.GetTileZ(transform.position);
        surroundingTiles.GetSurroundingTiles();
        currentLevel = surroundingTiles.currentTilePosition.z;

    }
    public void Update()
    {
        if (!isWeightless)
            SetIsGrounded();
        
        if (onSlope)
            HandleSlopes();
        else
        {
            slopeDisplacement = 0;
            slopeObject.localPosition = Vector3.zero;
        }
            

        if (getOnSlope)
            ChangeLevelOnSlope();
        if (getOffSlope)
            ChangeLevelOffSlope();

        int dif = Mathf.Abs(surroundingTiles.currentTilePosition.z - currentLevel);
        if (dif != 0)
            ChangeLevel(dif);
    }

    public void FixedUpdate()
    {
        if (!isGrounded && !isWeightless)
            ApplyGravity();
    }

    
    void ChangeLevelOffSlope()
    {

        int dif = surroundingTiles.currentTilePosition.z - currentLevel;

        float displacement = dif * spriteDisplacementY;
        Vector3 currentPosition = transform.position;
        slopeObject.localPosition = Vector3.zero;
        currentPosition = new Vector3(currentPosition.x, currentPosition.y + displacement, surroundingTiles.currentTilePosition.z + 1);
        transform.position = currentPosition;

        currentLevel = surroundingTiles.currentTilePosition.z;
        getOffSlope = false;

    }

    void ChangeLevelOnSlope()
    {
        
        int dif = surroundingTiles.currentTilePosition.z - currentLevel;

        float displacement = dif * spriteDisplacementY;
        Vector3 currentPosition = transform.position;
        slopeObject.localPosition = new Vector3(0, spriteDisplacementY, 1);
        currentPosition = new Vector3(currentPosition.x, currentPosition.y + displacement, surroundingTiles.currentTilePosition.z + 1);
        transform.position = currentPosition;

        currentLevel = surroundingTiles.currentTilePosition.z;
        getOnSlope = false;
    }

    void ChangeLevel(int dif)
    {
        
        bounceFactor = 1;
        float displacement = dif * spriteDisplacementY;
        Vector3 currentPosition = transform.position;
        
        if (surroundingTiles.currentTilePosition.z > currentLevel)
        {
            currentPosition = new Vector3(currentPosition.x, currentPosition.y + displacement, surroundingTiles.currentTilePosition.z + 1);
            transform.position = currentPosition;
            positionZ += dif;
            displacedPosition = new Vector3(displacedPosition.x, displacedPosition.y - displacement, displacedPosition.z - dif);
            itemObject.localPosition = new Vector3(itemObject.localPosition.x, itemObject.localPosition.y - displacement, itemObject.localPosition.z - dif);
            
        }
        else if (surroundingTiles.currentTilePosition.z < currentLevel)
        {
            currentPosition = new Vector3(currentPosition.x, currentPosition.y - displacement, surroundingTiles.currentTilePosition.z + 1);
            transform.position = currentPosition;
            positionZ -= dif;
            displacedPosition = new Vector3(displacedPosition.x, displacedPosition.y + displacement, displacedPosition.z + dif);
            itemObject.localPosition = new Vector3(itemObject.localPosition.x, itemObject.localPosition.y + displacement, itemObject.localPosition.z + dif);
            
        }
        currentLevel = surroundingTiles.currentTilePosition.z;

    }

    void HandleSlopes()
    {
       
        slopeCheckPosition = (Vector2)transform.position - (slopeDirection * 0.6f);
        RaycastHit2D hitA = Physics2D.Raycast(slopeCheckPosition, slopeDirection, 0.6f, groundLayer);

        if (hitA.collider != null)
        {
            slopeCollisionPoint = hitA.point;
            float distA = Vector2.Distance(hitA.point, transform.position);
            slopeDisplacement = distA / tileSize;

        }

        float displacementY = slopeDisplacement * spriteDisplacementY;
        
        slopeObject.localPosition = new Vector3(0, displacementY, slopeDisplacement);
    }

    

    public void Move(Vector2 dir, float velocity)
    {
        currentDirection = dir;
        currentVelocity = velocity;
        
        
        Vector3 currentPosition = transform.position;
        currentPosition = Vector2.MoveTowards(transform.position, (Vector2)transform.position + dir, Time.deltaTime * velocity);
        currentPosition.z = surroundingTiles.currentTilePosition.z + 1;
        transform.position = currentPosition;
        
    }

    public void MoveZ(Vector3 dir, float speed)
    {
        
        Vector3 currentPosition = itemObject.localPosition;
        currentPosition = Vector3.MoveTowards(itemObject.localPosition, itemObject.localPosition + dir, Time.deltaTime * speed);
        
        itemObject.localPosition  = currentPosition;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position).normalized;
        var totalZ = (itemObject.localPosition.z - screenPos.y) / 10;
        itemObject.localScale = new Vector3(1 + totalZ, 1 + totalZ, 1);

    }

    

    public void Nudge(Vector2 dir)
    {
        Vector3 currentPosition = transform.position;
        currentPosition += (Vector3)dir * checkTileDistance;
        
        transform.position = currentPosition;
    }
    protected void SetIsGrounded()
    {
        //Check and set if grounded
        float dist = Vector2.Distance(itemObject.localPosition, itemShadow.localPosition);
        isGrounded = dist <= 0.01f;
        if (isGrounded)
        {
            itemObject.localPosition = Vector3.zero;
            positionZ = 0;
            displacedPosition = Vector3.zero;
        }
    }

    public bool CheckForObstacles(Vector3 checkPosition, Vector3 doubleCheck, Vector2 direction)
    {
        // Check for a positive gameobject on the obstacle layer
        var hit = Physics2D.OverlapPoint(checkPosition, obstacleLayer, transform.position.z, transform.position.z);
        
        if (hit != null)
        {
           
            
            // do it got a thing
            if (hit.TryGetComponent(out DrawZasYDisplacement displacement))
            {
                // is our local z higher than the thing
                if (Mathf.Abs(itemObject.localPosition.z) >= displacement.positionZ)
                    return false;

            }
            var doubleHit = Physics2D.OverlapPoint(doubleCheck, obstacleLayer, transform.position.z, transform.position.z);
            if (doubleHit != null)
                Nudge(direction);
            return true;
           

        }
        return false;
    }

    protected void ApplyGravity()
    {
        positionZ -= gravity * Time.fixedDeltaTime;

        displacedPosition = new Vector3(0, spriteDisplacementY * positionZ, positionZ);
        itemObject.Translate(displacedPosition * Time.fixedDeltaTime);
        SetIsGrounded();

        if (itemObject.localPosition.y <= 0)
        {
            positionZ = 0;
            displacedPosition = Vector3.zero;
            itemObject.localPosition = Vector3.zero;

            if (bounceFactor >= .001f)
                Bounce(bounciness * bounceFactor);

        }
    }



    public void Bounce(float bounceAmount)
    {
        positionZ = bounceAmount;
        displacedPosition = new Vector3(0, spriteDisplacementY * positionZ, positionZ);
        //itemObject.transform.Translate(displacedPosition * Time.fixedDeltaTime);
        bounceFactor *= bounceFriction;
        ApplyGravity();
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(slopeCheckPosition, 0.05f);
        Gizmos.DrawRay(slopeCheckPosition, slopeDirection * 0.6f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(slopeCollisionPoint, 0.05f);
    }


}
