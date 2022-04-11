using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GravityItemMovement : MonoBehaviour
{

    

    public Transform itemObject;
    public Transform itemShadow;
    public Transform slopeObject;

    public LayerMask obstacleLayer;
    public LayerMask groundLayer;
    [Range(0.0f, 0.1f)]
    public float checkTileDistance = 0.08f;

    [HideInInspector]
    public CurrentGridLocation currentGridLocation;
    [HideInInspector]
    public SurroundingTiles surroundingTiles;
    DetectVisibility visibility;

    const float gravity = 20f;
    readonly float spriteDisplacementY = 0.2790625f;
    [HideInInspector]
    public float positionZ;
    [HideInInspector]
    public Vector3 displacedPosition;

    [HideInInspector]
    public bool isGrounded;
    bool onSlope = false;
    //bool wasOnSlope = false;
    Vector2 slopeDirection;

    Vector3 currentPosition;
    
    [HideInInspector]
    public Vector3Int nextTilePosition;

    public bool canDropOffEdges;
    [HideInInspector]
    public bool onCliffEdge;

    [Range(0, 1)]
    public float bounceFriction;
    [Range(0, 10)]
    public float bounciness;

    float bounceFactor = 0;
    int dif;

    int lastLevel;
    bool displacing;

    [HideInInspector]
    public float currentVelocity;
    Vector2 currentDirection;

    float slopeDisplacement;
    Vector2 slopeCollisionPoint;
    Vector2 slopeCheckPosition;
    float tileSize;

    private IEnumerator Start()
    {
        currentGridLocation = GetComponent<CurrentGridLocation>();
        surroundingTiles = GetComponent<SurroundingTiles>();
        visibility = GetComponent<DetectVisibility>();

        yield return new WaitForSeconds(0.25f);

        currentGridLocation.UpdateLocationAndPosition();
        surroundingTiles.GetSurroundingTiles();

        lastLevel = currentGridLocation.currentLevel;
        isGrounded = true;
        tileSize = currentGridLocation.groundGrid.cellSize.y;
    }

    private void Update()
    {
        if (currentGridLocation == null)
            return;

        
        SetIsGrounded();

        if (onSlope)
            HandleSlopes();

        
        if (currentGridLocation.currentLevel < lastLevel && !displacing)//either jumping or falling down a cliff
        {
            if (canDropOffEdges)
            {
                HardDisplaceDown();
            }
            else
            {
                SoftDisplaceDown();
            }
            
        }
        
        

        int levelDiff = currentGridLocation.currentLevel - lastLevel;
        if (levelDiff == 1 && !displacing )//i'm jumping up a tile 
        {
            if (!isGrounded)
                HardDisplaceUp();
            else
                SoftDisplaceUp();
        }
        
    }


    private void FixedUpdate()
    {
        if (!isGrounded)
            ApplyGravity();
    }



    void SetIsGrounded()
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



    public void Move(Vector2 dir, float velocity)
    {
        currentDirection = dir;
        currentVelocity = velocity;
        currentGridLocation.UpdateLocationAndPosition();
        surroundingTiles.GetSurroundingTiles();

        if (CanReachNextTile(dir)) 
        {
            currentPosition = transform.position;
            currentPosition = Vector2.MoveTowards(transform.position, (Vector2)transform.position + dir, Time.deltaTime * velocity);
            currentPosition.z = lastLevel;
            transform.position = currentPosition;
        }
    }

    bool CanReachNextTile(Vector2 movement)
    {
        
        Vector3 checkPosition = (transform.position + (Vector3)movement * checkTileDistance) - Vector3.forward;

        if (CheckForObstacles(checkPosition))
            return false;

        nextTilePosition = currentGridLocation.groundGrid.WorldToCell(checkPosition);
        Vector3Int nextTileKey = nextTilePosition - currentGridLocation.lastTilePosition;
        onCliffEdge = false;


        foreach (var tile in surroundingTiles.allCurrentDirections)
        {
            // CURRENT TILE ----------------------------------------------------------------------------------------------------
            // right now, where we are, what it be? is it be a slope?
            if (tile.Key == Vector3Int.zero)
            {
                slopeDirection = Vector2.zero;
                onSlope = tile.Value.tileName.Contains("Slope");
                if (onSlope)
                {
                    if (tile.Value.tileName.Contains("X"))
                        slopeDirection = tile.Value.tileName.Contains("0") ? new Vector2(-0.9f, -0.5f) : new Vector2(0.9f, 0.5f);
                    else
                        slopeDirection = tile.Value.tileName.Contains("0") ? new Vector2(0.9f, -0.5f) : new Vector2(-0.9f, 0.5f);
                    continue;
                }
            }


            // JUMPING! ----------------------------------------------------------------------------------------------------
            // I don't care what height the tile is at as long as the sprite is jumping and has a y above the tile height
            if (tile.Key == nextTileKey && !isGrounded && Mathf.Abs(displacedPosition.y) >= tile.Value.levelZ)
            {
                onCliffEdge = false;
                return true;
            }


            // GROUNDED! ----------------------------------------------------------------------------------------------------
            // the next tile is valid
            if (tile.Key == nextTileKey && tile.Value.isValid)
            {
                

                // if the next tile is a slope, am i approaching it in the right direction?
                if (tile.Value.tileName.Contains("Slope"))
                {
                    if (tile.Value.tileName.Contains("X") && nextTileKey.x == 0 || tile.Value.tileName.Contains("Y") && nextTileKey.y == 0)
                        return false;
                    
                }

                // I am on a slope
                if (onSlope)
                {
                    //am i walking 'off' the slope on the upper part in the right direction?
                    if (surroundingTiles.allCurrentDirections[Vector3Int.zero].tileName.Contains("X") && nextTileKey.x == 0 || surroundingTiles.allCurrentDirections[Vector3Int.zero].tileName.Contains("Y") && nextTileKey.y == 0)
                    {
                        onCliffEdge = true;
                        return false;
                    }
                }

            }

            // the next tile is NOT valid
            if (tile.Key == nextTileKey && !tile.Value.isValid)
            {
                
                // If I am on a slope, am i approaching or leaving the slope in a valid direction?
                if (onSlope)
                {
                    if (surroundingTiles.allCurrentDirections[Vector3Int.zero].tileName.Contains("X") && nextTileKey.x != 0 || surroundingTiles.allCurrentDirections[Vector3Int.zero].tileName.Contains("Y") && nextTileKey.y != 0)
                        continue;
                }

                // This is where we fall of a cliff
                if (tile.Value.levelZ == 0)
                {
                    if (canDropOffEdges)
                        return true;
                    else
                        onCliffEdge = true;
                }

                

                // This is where we hit a wall of height 1 or above
                return false;
            }
        }
        return true;
    }

    bool CheckForObstacles(Vector3 checkPosition)
    {
        // Check for gameobjects on the obstacle layer
        var hit = Physics2D.OverlapPoint(checkPosition, obstacleLayer);
        if (hit != null)
        {
            if (hit.gameObject.transform.position.z == transform.position.z)
                return true;
        }
        return false;
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
        float displacementZ = slopeDisplacement - (visibility.isHidden ? 1 : 0);
        slopeObject.localPosition = new Vector3(0, displacementY, displacementZ);

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(slopeCheckPosition, 0.05f);
        Gizmos.DrawRay(slopeCheckPosition, slopeDirection * 0.6f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(slopeCollisionPoint, 0.05f);
    }

    void HardDisplaceUp()
    {
        
        displacing = true;

        dif = currentGridLocation.currentLevel - lastLevel;
        float displacement = dif * spriteDisplacementY;
        currentPosition = transform.position;
        transform.position = new Vector3(currentPosition.x, currentPosition.y + displacement, currentGridLocation.currentLevel);

        
        itemObject.localPosition = new Vector3(itemObject.localPosition.x, !onSlope ? itemObject.localPosition.y - displacement : transform.localPosition.y, !onSlope ? lastLevel - 1: 0);
        

        Invoke(nameof(ResetDisplacing), 0.3f);

        lastLevel = currentGridLocation.currentLevel;
    }
    void SoftDisplaceUp()
    {

        displacing = true;

        dif = currentGridLocation.currentLevel - lastLevel;
        float displacement = dif * spriteDisplacementY;
        currentPosition = transform.position;
        transform.position = new Vector3(currentPosition.x, currentPosition.y + displacement, currentGridLocation.currentLevel);
        slopeObject.localPosition = Vector3.zero;

        Invoke(nameof(ResetDisplacing), 0.3f);

        lastLevel = currentGridLocation.currentLevel;
    }

    void HardDisplaceDown()
    {
        displacing = true;
        bounceFactor = 1;
        dif = currentGridLocation.currentLevel - lastLevel;
        float displacement = dif * spriteDisplacementY;
        currentPosition = transform.position;
        transform.position = new Vector3(currentPosition.x, currentPosition.y + displacement, currentGridLocation.currentLevel);


        itemObject.localPosition = new Vector3(itemObject.localPosition.x, itemObject.localPosition.y - displacement , lastLevel - 1 );

        lastLevel = currentGridLocation.currentLevel;

        Invoke(nameof(ResetDisplacing), 0.3f);
    }

    void SoftDisplaceDown()
    {
        displacing = true;
        
        dif = currentGridLocation.currentLevel - lastLevel;
        float displacement = dif * spriteDisplacementY;
        currentPosition = transform.position;
        transform.position = new Vector3(currentPosition.x, currentPosition.y + displacement, currentGridLocation.currentLevel);

        lastLevel = currentGridLocation.currentLevel;

        Invoke(nameof(ResetDisplacing), 0.3f);
    }

    void ResetDisplacing()
    {
        displacing = false;
    }

    public void Bounce(float bounceAmount)
    {
        positionZ += bounceAmount;
        displacedPosition = new Vector3(0, spriteDisplacementY * positionZ, positionZ);
        itemObject.transform.Translate(displacedPosition * Time.fixedDeltaTime);
        bounceFactor *= bounceFriction;
    }

    void ApplyGravity()
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
                Bounce((bounciness + Mathf.Abs(dif)) * bounceFactor);

        }
    }

}
