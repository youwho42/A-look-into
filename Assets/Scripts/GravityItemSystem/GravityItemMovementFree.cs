using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GravitySystem { 
    public class GravityItemMovementFree : GravityItemNew
    {


        [Range(0, 2)]
        public float itemDrag;
        [Range(0, 1)]
        public float itemBounceFriction;

        float velocity;
        Vector2 mainDirection;
        Vector3Int nextTilePosition;
        public bool canCollideWithGravityItems;

        DrawZasYDisplacement displacement;

        private new IEnumerator Start()
        {
            base.Start();
        

            yield return new WaitForSeconds(0.25f);
            displacement = GetComponent<DrawZasYDisplacement>();
        
        
        

            isGrounded = true;

        }

        private new void Update()
        {
            base.Update();
        
        }
        public new void FixedUpdate()
        {
            base.FixedUpdate();
            if (!CanReachNextTile(mainDirection))
            {
                CheckBounce();
            
            }

            Move(mainDirection, velocity);

            if (velocity > 0)
                velocity -= itemDrag * Time.fixedDeltaTime;

        }

        void CheckBounce()
        {
            Vector3Int diff = nextTilePosition - currentTilePosition.position;
            List<TileDirectionInfo> tileBlock;
            allTilesManager.allTilesDictionary.TryGetValue(currentTilePosition.position, out tileBlock);
            foreach (var tile in tileBlock)
            {
                if (tile.direction == diff)
                {
                    if (tile.levelZ >= 0)
                    {
                        mainDirection = new Vector2(mainDirection.y, mainDirection.x);
                        if (diff.x > 0)
                            mainDirection *= -1;

                        velocity *= itemBounceFriction;

                    }
                }
            }
        }
    

        bool CanReachNextTile(Vector2 direction)
        {
        

            Vector3 checkPosition = (transform.position + (Vector3)direction * checkTileDistance) - Vector3.forward;
            Vector3 doubleCheckPosition = transform.position - Vector3.forward;

            if (CheckForObstacles(checkPosition, doubleCheckPosition, direction))
            {
                mainDirection *= -1;
                AddMovement(mainDirection,  velocity * itemBounceFriction);
                return false;
            }
            

            nextTilePosition = currentTilePosition.grid.WorldToCell(checkPosition);
            Vector3Int nextTileKey = nextTilePosition - currentTilePosition.position;
        

        

            int level = 0;
            List<TileDirectionInfo> tileBlock;
            allTilesManager.allTilesDictionary.TryGetValue(currentTilePosition.position, out tileBlock);
            if (tileBlock == null)
                return true;
            foreach (var tile in tileBlock)
            {
                // CURRENT TILE ----------------------------------------------------------------------------------------------------
                // right now, where we are, what it be? is it be a slope?
                if (tile.direction == Vector3Int.zero)
                {
                    slopeDirection = Vector2.zero;
                    onSlope = tile.tileName.Contains("Slope");
                    if (onSlope)
                    {
                        if (tile.tileName.Contains("X"))
                            slopeDirection = tile.tileName.Contains("0") ? new Vector2(-0.9f, -0.5f) : new Vector2(0.9f, 0.5f);
                        else
                            slopeDirection = tile.tileName.Contains("0") ? new Vector2(0.9f, -0.5f) : new Vector2(-0.9f, 0.5f);
                        continue;
                    }

                }
                if (tile.direction == nextTileKey)
                    level = tile.levelZ;
                else
                    continue;
                Vector3Int doubleCheckTilePosition = currentTilePosition.grid.WorldToCell(doubleCheckPosition);

                // IN AIR! ----------------------------------------------------------------------------------------------------
                // I don't care what height the tile is at as long as the sprite is in the air and has a y above the tile height
                if (tile.direction == nextTileKey && !isGrounded && Mathf.Abs(itemObject.localPosition.z) >= level)
                {

                    currentTilePosition.position += new Vector3Int(nextTileKey.x, nextTileKey.y, level);

                    if (tile.tileName.Contains("Slope"))
                        onSlope = true;

                    return true;
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

                        onSlope = true;

                        // is the slope is lower?
                        if (tile.levelZ < 0)
                            getOnSlope = true;


                    }

                    // I am on a slope
                    if (onSlope)
                    {
                        //am i walking 'off' the slope on the upper part in the right direction?
                        /*if (surroundingTiles.allCurrentDirections[Vector3Int.zero].tileName.Contains("X") && nextTileKey.x == 0 || surroundingTiles.allCurrentDirections[Vector3Int.zero].tileName.Contains("Y") && nextTileKey.y == 0)
                        {
                            onCliffEdge = true;
                            return false;
                        }*/
                        if (tile.levelZ > 0)
                            getOffSlope = true;
                    }

                }

                // the next tile is NOT valid
                if (tile.direction == nextTileKey && !tile.isValid)
                {
                    if (doubleCheckTilePosition == nextTilePosition)
                    {
                        Nudge(direction);
                    }

                    // If I am on a slope, am i approaching or leaving the slope in a valid direction?
                    if (onSlope)
                    {
                        if (tile.direction == Vector3Int.zero && tile.tileName.Contains("X") && nextTileKey.x != 0 || tile.direction == Vector3Int.zero && tile.tileName.Contains("Y") && nextTileKey.y != 0)
                            continue;
                    }

                    // This is where we are at the top of a cliff
                    if (tile.levelZ <= 0)
                    {
                        currentTilePosition.position += new Vector3Int(nextTileKey.x, nextTileKey.y, level);
                        return true;
                    }
                    

                    // This is where we hit a wall of height 1 or above
                    return false;
                }
            }

            currentTilePosition.position += new Vector3Int(nextTileKey.x, nextTileKey.y, level);

            return true;
        }

    


        void AddMovement(Vector2 newDirection, float _velocity)
        {
            mainDirection = newDirection.normalized;
            velocity = _velocity;
        }

       private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider is CompositeCollider2D)
                return;
       
            if (collision.gameObject.TryGetComponent(out GravityItemNew gravityItem) && canCollideWithGravityItems)
            {
            
                if (gravityItem.currentLevel != currentLevel || gravityItem.itemObject.localPosition.z > displacement.positionZ)
                    return;
                Vector2 direction = transform.position - collision.transform.position;
            
                AddMovement(direction, gravityItem.currentVelocity != 0 ? gravityItem.currentVelocity : velocity * itemBounceFriction);
            }

            if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
            {
                var direction = transform.position - collision.transform.position;
                AddMovement(direction, velocity * itemBounceFriction);
            }

        }


        /*private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
            {
                var direction = transform.position - collision.transform.position;
                AddMovement(direction, velocity * itemBounceFriction);
            }
        }
    */

    }
}
