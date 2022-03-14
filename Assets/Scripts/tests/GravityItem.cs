using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DetectVisibility))]
[RequireComponent(typeof(SurroundingTiles))]
[RequireComponent(typeof(CurrentGridLocation))]
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
    public CurrentGridLocation currentGridLocation;
    [HideInInspector]
    public SurroundingTiles surroundingTiles;
    [HideInInspector]
    public float currentVelocity;
    Vector2 currentDirection;

    // slopes
    protected bool onSlope;
    protected bool nextTileIsSlope;
    float slopeDisplacement;
    Vector2 slopeCollisionPoint;
    Vector2 slopeCheckPosition;
    protected Vector2 slopeDirection;

    [Header("Slope Detection")]
    [Range(0.0f, 0.1f)]
    public float checkTileDistance = 0.08f;
    public LayerMask groundLayer;

    


    public void Start()
    {
        currentGridLocation = GetComponent<CurrentGridLocation>();
        surroundingTiles = GetComponent<SurroundingTiles>();
    }
    public void Update()
    {
        
        if (onSlope)
            HandleSlopes();
        else
            slopeObject.localPosition = Vector3.zero;
    }

    public void FixedUpdate()
    {
        if (!isGrounded)
            ApplyGravity();
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(slopeCheckPosition, 0.05f);
        Gizmos.DrawRay(slopeCheckPosition, slopeDirection * 0.6f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(slopeCollisionPoint, 0.05f);
    }

    protected void Move(Vector2 dir, float velocity)
    {
        currentDirection = dir;
        currentVelocity = velocity;
        currentGridLocation.UpdateLocationAndPosition();
        surroundingTiles.GetSurroundingTiles();

        
        Vector3 currentPosition = transform.position;
        currentPosition = Vector2.MoveTowards(transform.position, (Vector2)transform.position + dir, Time.deltaTime * velocity);
        currentPosition.z = currentGridLocation.currentLevel;
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



    protected void ApplyGravity()
    {
        positionZ -= gravity * Time.fixedDeltaTime;

        displacedPosition = new Vector3(0, spriteDisplacementY * positionZ, positionZ);
        itemObject.Translate(displacedPosition * Time.fixedDeltaTime);


        if (itemObject.localPosition.y <= 0)
        {
            positionZ = 0;
            displacedPosition = Vector3.zero;
            itemObject.localPosition = Vector3.zero;

            if (bounceFactor >= .0001f)
                Bounce(bounciness * bounceFactor);

        }
    }



    protected void Bounce(float bounceAmount)
    {
        positionZ += bounceAmount;
        displacedPosition = new Vector3(0, spriteDisplacementY * positionZ, positionZ);
        itemObject.transform.Translate(displacedPosition * Time.fixedDeltaTime);
        bounceFactor *= bounceFriction;
    }


}
