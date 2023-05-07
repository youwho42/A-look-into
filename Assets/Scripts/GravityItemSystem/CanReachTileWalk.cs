using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GravitySystem
{
    public class CanReachTileWalk : MonoBehaviour
    {
        [HideInInspector]
        public GravityItemNew gravityItem;



        public float roamingDistance;
        [HideInInspector]
        public Vector2 currentDestination;
        [HideInInspector]
        public Vector2 currentDirection;
        //bool onSlope;

        public float walkSpeed;
        Vector3Int nextTilePosition;
        public SpriteRenderer characterRenderer;
        public Transform characterTransformFlip;
        public bool canJump;
        public float jumpHeight;
        public bool jumpAhead;
        public bool canClimb;
        public bool isClimbing;

        [HideInInspector]
        public Vector3 mainDestinationZ;
        [HideInInspector]
        public Vector3 currentDestinationZ;
        [HideInInspector]
        public Vector3 currentDirectionZ;

        const float spriteDisplacementY = 0.2790625f;

        private void Start()
        {
            gravityItem = GetComponent<GravityItemNew>();

        }
        private void FixedUpdate()
        {
            CanReachNextTile(currentDirection);
        }

        public void Walk()
        {
            



            if (CanReachNextTile(currentDirection) && !isClimbing)
            {
                gravityItem.isWeightless = false;
                gravityItem.Move(currentDirection, walkSpeed);


            }
            else
            {
                
                if (!jumpAhead)
                    SetRandomDestination();
                if (canJump && jumpAhead && gravityItem.isGrounded)
                    gravityItem.Bounce(jumpHeight);

            }


            if (canClimb && isClimbing)
            {
                SetDirectionZ();

                gravityItem.MoveZ(currentDirectionZ, walkSpeed * 2);
            }


        }


        void TileFound(List<TileDirectionInfo> tileBlock, bool success)
        {
            if (success)
                gravityItem.tileBlockInfo = tileBlock;
        }




        public bool CanReachNextTile(Vector2 direction)
        {
            if (gravityItem.currentTilePosition.grid == null)
                return false;

            jumpAhead = false;
            Vector3 checkPosition = (transform.position + (Vector3)direction * gravityItem.checkTileDistance) - Vector3.forward;
            Vector3 doubleCheckPosition = transform.position - Vector3.forward;
            if (gravityItem.CheckForObstacles(checkPosition, doubleCheckPosition, direction))
                return false;

            nextTilePosition = gravityItem.currentTilePosition.grid.WorldToCell(checkPosition);

            Vector3Int nextTileKey = nextTilePosition - gravityItem.currentTilePosition.position;

            if (nextTileKey == Vector3Int.zero)
                return true;



            int level = 0;

            TileInfoRequestManager.RequestTileInfo(gravityItem.currentTilePosition.position, TileFound);

            if (gravityItem.tileBlockInfo == null)
                return true;

            foreach (var tile in gravityItem.tileBlockInfo)
            {
                // CURRENT TILE ----------------------------------------------------------------------------------------------------
                // right now, where we are, what it be? is it be a slope?
                if (tile.direction == Vector3Int.zero)
                {

                    gravityItem.slopeDirection = Vector2.zero;
                    gravityItem.onSlope = tile.tileName.Contains("Slope");
                    if (gravityItem.onSlope)
                    {
                        
                        if (tile.tileName.Contains("X"))
                            gravityItem.slopeDirection = tile.tileName.Contains("0") ? new Vector2(-0.9f, -0.5f) : new Vector2(0.9f, 0.5f);
                        else
                            gravityItem.slopeDirection = tile.tileName.Contains("0") ? new Vector2(0.9f, -0.5f) : new Vector2(-0.9f, 0.5f);
                        continue;
                    }

                }
                if (tile.direction == nextTileKey)
                    level = tile.levelZ;
                else
                    continue;
                Vector3Int doubleCheckTilePosition = gravityItem.currentTilePosition.grid.WorldToCell(doubleCheckPosition);



                // JUMPING! ----------------------------------------------------------------------------------------------------
                // I don't care what height the tile is at as long as the sprite is jumping and has a y above the tile height
                if (tile.direction == nextTileKey)
                {



                    if (!gravityItem.isGrounded && Mathf.Abs(gravityItem.itemObject.localPosition.z) >= level)
                    {

                        gravityItem.currentTilePosition.position += new Vector3Int(nextTileKey.x, nextTileKey.y, level);

                        if (tile.tileName.Contains("Slope"))
                            gravityItem.onSlope = true;

                        return true;
                    }
                }

                // GROUNDED! ----------------------------------------------------------------------------------------------------
                // the next tile is valid



                if (tile.direction == nextTileKey && tile.isValid)
                {

                    // if the next tile is a slope, am i approaching it in the right direction?
                    if (tile.tileName.Contains("Slope"))
                    {
                        if (tile.tileName.Contains("X") && nextTileKey.x == 0 || tile.tileName.Contains("Y") && nextTileKey.y == 0)
                            return false;

                        gravityItem.onSlope = true;

                        // is the slope lower?
                        if (tile.levelZ < 0)
                            gravityItem.getOnSlope = true;


                    }

                    // I am on a slope
                    if (gravityItem.onSlope)
                    {
                        //am i walking 'off' the slope on the upper part in the right direction?
                        if (tile.direction == Vector3Int.zero && tile.tileName.Contains("X") && nextTileKey.x == 0 || tile.direction == Vector3Int.zero && tile.tileName.Contains("Y") && nextTileKey.y == 0)
                        {
                            //onCliffEdge = true;
                            return false;
                        }
                        if (tile.levelZ > 0)
                            gravityItem.getOffSlope = true;
                    }

                }

                // the next tile is NOT valid
                if (tile.direction == nextTileKey && !tile.isValid)
                {
                    if (doubleCheckTilePosition == nextTilePosition)
                    {
                        gravityItem.Nudge(direction);
                    }

                    // If I am on a slope, am i approaching or leaving the slope in a valid direction?
                    if (gravityItem.onSlope)
                    {
                        if (tile.direction == Vector3Int.zero && tile.tileName.Contains("X") && nextTileKey.x != 0 || tile.direction == Vector3Int.zero && tile.tileName.Contains("Y") && nextTileKey.y != 0)
                            continue;
                    }

                    if (tile.levelZ == 1 && !gravityItem.onSlope)
                    {
                        jumpAhead = true;
                        return false;
                    }
                        

                    if (tile.levelZ == -1)
                    {
                        gravityItem.currentTilePosition.position += new Vector3Int(nextTileKey.x, nextTileKey.y, level);
                        return true;
                    }
                    // This is where we are on top of a cliff
                    //if (tile.levelZ <= -1)
                    //{
                    //    return false;
                    //}


                    // This is where we hit a wall of height 1 or above
                    return false;
                }
            }

            gravityItem.currentTilePosition.position += new Vector3Int(nextTileKey.x, nextTileKey.y, level);



            return true;
        }


        public void SetFacingDirection(Vector2 direction)
        {
            // Set facing direction
            var dir = Mathf.Sign(direction.x);
            if(characterRenderer != null)
                characterRenderer.flipX = dir > 0;
            else
            {
                if (characterTransformFlip.localScale.x != dir)
                {
                    Vector3 theScale = characterTransformFlip.localScale;
                    theScale.x *= -1;
                    characterTransformFlip.localScale = theScale;
                }
            }
        }



        public void SetRandomDestination()
        {

            if (gravityItem == null || gravityItem.currentTilePosition.groundMap == null)
                return;

            Vector2 rand = (UnityEngine.Random.insideUnitCircle * roamingDistance);
            var d = gravityItem.currentTilePosition.groundMap.WorldToCell(new Vector2(transform.position.x + rand.x, transform.position.y + rand.y));
            for (int z = gravityItem.currentTilePosition.groundMap.cellBounds.zMax; z > gravityItem.currentTilePosition.groundMap.cellBounds.zMin - 1; z--)
            {
                d.z = z;
                if (gravityItem.currentTilePosition.groundMap.GetTile(d) != null)
                {

                    currentDestination = gravityItem.currentTilePosition.groundMap.GetCellCenterWorld(d);
                    currentDestination += new Vector2(UnityEngine.Random.Range(0f, .3f), UnityEngine.Random.Range(0f, .3f));
                    break;
                }

            }


            SetDirection();
        }

        public void SetDestination(DrawZasYDisplacement destination)
        {
            currentDestination = destination.transform.position;
            SetDirection();
        }

        public void SetWorldDestination(Vector3 destination)
        {
            currentDestination = destination;
            SetDirection();
        }

        void SetDirection()
        {
            currentDirection = currentDestination - (Vector2)gravityItem.gameObject.transform.position;
            if (!isClimbing)
                SetFacingDirection(currentDirection);
        }

        public void SetDestinationZ(DrawZasYDisplacement displacement)
        {

            mainDestinationZ = displacement.displacedPosition;
            currentDestinationZ = mainDestinationZ;
            SetDirectionZ();
        }
        public void ResetDestinationZ()
        {

            mainDestinationZ = Vector3Int.zero;
            currentDestinationZ = mainDestinationZ;
            SetDirectionZ();
        }
        public void SetDirectionZ()
        {

            currentDirectionZ = currentDestinationZ - gravityItem.itemObject.localPosition;

        }

    }
}