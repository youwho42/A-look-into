using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityItemMovement : MonoBehaviour
{
    public Transform itemObject;
    const float gravity = 20f;
    public CurrentGridLocation currentGridLocation;

    public float positionZ;
    public readonly float spriteDisplacementY = 0.27808595f;
    public Vector3 displacedPosition;
    public bool isGrounded;
    public bool onSlope;

    public SurroundingTiles surroundingTiles;
    

    Vector3 currentPosition;
    
    public LayerMask obstacleLayer;

    public Vector3Int nextTilePosition;

    

    [Range(0, 1)]
    public float bounceFriction;
    [Range(0, 10)]
    public float bounciness;

    float bounceFactor = 1;
    int dif;

    public int lastLevel;
    bool displacing;

    public float currentVelocity;
    
    private IEnumerator Start()
    {
        currentGridLocation = GetComponent<CurrentGridLocation>();
        surroundingTiles = GetComponent<SurroundingTiles>();

        yield return new WaitForSeconds(0.25f);

        currentGridLocation.UpdateLocationAndPosition();
        surroundingTiles.GetSurroundingTiles();

        lastLevel = currentGridLocation.currentLevel;
        isGrounded = true;
    }

    private void Update()
    {
        if (currentGridLocation == null)
            return;

        if (currentGridLocation.currentLevel < lastLevel && !displacing)
        {
            DisplaceDown();
        }
        int levelDiff = currentGridLocation.currentLevel - lastLevel;
        if (levelDiff >= 1 && !displacing && Mathf.Abs(displacedPosition.y) >= levelDiff)
        {
            DisplaceUp();
        }
        else if (levelDiff == 1 && !displacing && isGrounded && onSlope)
        {
            SoftDisplace();
        }


        isGrounded = itemObject.localPosition.y <= 0;
        if (isGrounded)
        {
            if (itemObject.localPosition.z != 0 ||  itemObject.localPosition.y != 0)
            itemObject.localPosition = Vector3.zero;
        }
            


        if (!isGrounded)
        {

            ApplyGravity();
        }

    }


    public void Move(Vector2 dir, float velocity)
    {

        currentVelocity = velocity;
        currentGridLocation.UpdateLocation();
        surroundingTiles.GetSurroundingTiles();

        currentPosition = transform.position;
        currentPosition = Vector2.MoveTowards(transform.position, (Vector2)transform.position + dir, Time.deltaTime * velocity);
        currentPosition.z = currentGridLocation.currentLevel;
        transform.position = currentPosition;
        
    }

    public bool CanReachNextPosition(Vector2 movement)
    {
        float distance = 0.05f;
        Vector3 checkPosition = (transform.position + (Vector3)movement * distance) - Vector3.forward;
        nextTilePosition = currentGridLocation.groundGrid.WorldToCell(checkPosition);

        Vector3Int diff = nextTilePosition - currentGridLocation.lastTilePosition;
        foreach (var tile in surroundingTiles.allCurrentDirections)
        {
            //right now, where we are, is it be a slope?
            if(tile.Key == Vector3Int.zero)
                onSlope = tile.Value.tileName.Contains("Slope");
             
            // we check if the next tile is valid or not, and only if it is not valid do we continue.
            if (tile.Key == diff && !tile.Value.isValid)
            {
                // This is where we fall of a cliff
                if (tile.Value.difference == 0)
                    return true;

                // This is where we hit a wall of height 1

                //maybe it's a slope.
                if (tile.Value.difference == 1)
                {
                    // if its's a slope, am I approaching it from the correct angle?
                    if (tile.Value.tileName.Contains("X") && diff.x != 0 || tile.Value.tileName.Contains("Y") && diff.y != 0)
                        return true;
                }

                // I don't care what height the tile is at as long as the sprite is jumping and has a y above the tile height
                if (!isGrounded && Mathf.Abs(displacedPosition.y) >= tile.Value.difference)
                    return true;
                // it's really just a wall of a height of more than one and we are not jumping 
                return false;
                
            }

        }

        // Check for gameobjects on the obstacle layer
        var hit = Physics2D.OverlapPoint(checkPosition, obstacleLayer);
        if (hit != null)
            return false;

        return true;
       
    }





    void SoftDisplace()
    {
        displacing = true;

        lastLevel = currentGridLocation.currentLevel;

        Invoke("ResetDisplacing", 0.5f);
    }

    void DisplaceUp()
    {
        displacing = true;

        dif = currentGridLocation.currentLevel - lastLevel;
        float displacement = dif * spriteDisplacementY;
        currentPosition = transform.position;
        transform.position = new Vector3(currentPosition.x, currentPosition.y + displacement, currentPosition.z);
        itemObject.localPosition = new Vector3(itemObject.localPosition.x, itemObject.localPosition.y - displacement, lastLevel - 1);

        Invoke("ResetDisplacing", 0.3f);

        lastLevel = currentGridLocation.currentLevel;
    }
    void DisplaceDown()
    {
        displacing = true;
        bounceFactor = 1;
        dif = currentGridLocation.currentLevel - lastLevel;
        float displacement = dif * spriteDisplacementY;
        currentPosition = transform.position;
        transform.position = new Vector3(currentPosition.x, currentPosition.y + displacement, currentPosition.z);
        itemObject.localPosition = new Vector3(itemObject.localPosition.x, itemObject.localPosition.y - displacement, lastLevel - 1);

        Invoke("ResetDisplacing", 0.3f);

        lastLevel = currentGridLocation.currentLevel;
    }

    void ResetDisplacing()
    {
        displacing = false;
    }

    public void Bounce(float bounceAmount)
    {
        positionZ += bounceAmount;
        displacedPosition = new Vector3(0, spriteDisplacementY * positionZ, positionZ);
        itemObject.transform.Translate(displacedPosition * Time.deltaTime);
        bounceFactor *= bounceFriction;
    }

    void ApplyGravity()
    {
        positionZ -= gravity * Time.deltaTime;
        displacedPosition = new Vector3(0, spriteDisplacementY * positionZ, positionZ);
        itemObject.Translate(displacedPosition * Time.deltaTime);

        if (itemObject.localPosition.y <= 0)
        {
            positionZ = 0;
            displacedPosition = Vector3.zero;
            itemObject.localPosition = Vector3.zero;
            if (bounceFactor >= .1f)
                Bounce((bounciness + Mathf.Abs(dif)) * bounceFactor);
        }
    }

}
