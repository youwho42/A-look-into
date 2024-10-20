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

        Vector3 checkPosition;
        Vector3 doubleCheckPosition;


        float velocity;
        Vector2 mainDirection;
        Vector3Int nextTilePosition;
        public bool canCollideWithGravityItems;

        DrawZasYDisplacement displacement;
        bool canMove;
        private new IEnumerator Start()
        {
            base.Start();
        

            yield return new WaitForSeconds(0.25f);
            displacement = GetComponent<DrawZasYDisplacement>();
        
        
        

            isGrounded = true;

        }

        public override void Update()
        {
            base.Update();
            canMove = CanReachNextTile(mainDirection);
            
                
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!canMove)
                return;
                
            Move(mainDirection, velocity);

            if (velocity > 0)
                velocity -= itemDrag * Time.fixedDeltaTime;

        }
        Vector2 GetTileNormal(Vector2Int tileDirection)
        {
            float n = GlobalSettings.TileSize;
            // BR
            if(tileDirection == Vector2Int.down)
                return new Vector2(-n, n).normalized;
            // TL
            if (tileDirection == Vector2Int.up)
                return new Vector2(n,-n).normalized;
            // TR
            if (tileDirection == Vector2Int.right)
                return new Vector2(-n, -n).normalized;
            // BL
            if (tileDirection == -Vector2Int.right)
                return new Vector2(n, n).normalized;
            //// BC
            //if (tileDirection == -Vector2Int.one)
            //{
            //    foreach (var tile in tileBlockInfo)
            //    { 
            //        if((Vector2Int)tile.direction == new Vector2Int(-1, 0) && !tile.isValid)
            //            return new Vector2(n, n).normalized;
            //        if ((Vector2Int)tile.direction == new Vector2Int(0, -1) && !tile.isValid)
            //            return new Vector2(-n, n).normalized;
            //    }
                
            //}
            //// BC
            //if (tileDirection == Vector2Int.one)
            //{
            //    foreach (var tile in tileBlockInfo)
            //    {
            //        if ((Vector2Int)tile.direction == new Vector2Int(1, 0) && !tile.isValid)
            //            return new Vector2(-n, -n).normalized;
            //        if ((Vector2Int)tile.direction == new Vector2Int(0, 1) && !tile.isValid)
            //            return new Vector2(n, -n).normalized;
            //    }

            //}
            //// LC
            //if (tileDirection == new Vector2Int(-1, 1))
            //{
            //    foreach (var tile in tileBlockInfo)
            //    {
            //        if ((Vector2Int)tile.direction == new Vector2Int(-1, 0) && !tile.isValid)
            //            return new Vector2(n, n).normalized;
            //        if ((Vector2Int)tile.direction == new Vector2Int(0, 1) && !tile.isValid)
            //            return new Vector2(n, -n).normalized;
            //    }

            //}
            //// RC
            //if (tileDirection == new Vector2Int(1, -1))
            //{
            //    foreach (var tile in tileBlockInfo)
            //    {
            //        if ((Vector2Int)tile.direction == new Vector2Int(1, 0) && !tile.isValid)
            //            return new Vector2(-n, -n).normalized;
            //        if ((Vector2Int)tile.direction == new Vector2Int(0, -1) && !tile.isValid)
            //            return new Vector2(-n, n).normalized;
            //    }

            //}
            return Vector2.zero;
        }
        void CheckBounce()
        {
            Vector3Int diff = nextTilePosition - currentTilePosition.position;
            
            foreach (var tile in tileBlockInfo)
            {
                if (tile.direction == diff)
                {
                    if (tile.levelZ >= 0)
                    {

                        mainDirection = Vector2.Reflect(mainDirection * velocity, GetTileNormal((Vector2Int)tile.direction));

                        AddMovement(mainDirection, velocity * itemBounceFriction);

                        velocity *= itemBounceFriction;

                    }
                    break;
                }
            }
            
        }
    

        bool CanReachNextTile(Vector2 direction)
        {

            checkPosition = (_transform.position + (Vector3)direction * checkTileDistance) - Vector3.forward;
            doubleCheckPosition = _transform.position - Vector3.forward;


            nextTilePosition = currentTilePosition.grid.WorldToCell(checkPosition);

            Vector3Int nextTileKey = nextTilePosition - currentTilePosition.position;
            
            if (CheckForObstacles(checkPosition, doubleCheckPosition, direction, nextTileKey, true))
            {
                
                //mainDirection = new Vector2(mainDirection.y*Mathf.Sign(collisionNormal.x), mainDirection.x* Mathf.Sign(collisionNormal.y));
                //if (collisionNormal.x > 0)
                //    mainDirection *= -1;
                mainDirection = Vector2.Reflect(mainDirection * velocity, collisionNormal);
                AddMovement(mainDirection, velocity * itemBounceFriction);
                
                return false;
            }

            int level = 0;

            TileInfoRequestManager.RequestTileInfo(currentTilePosition.position, TileFound);

            if (tileBlockInfo == null)
                return true;

            foreach (var tile in tileBlockInfo)
            {
                if (tile.direction != Vector3Int.zero)
                    continue;

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
                break;
            }

            if (nextTilePosition == currentTilePosition.position)
                return true;


            foreach (var tile in tileBlockInfo)
            {

                if (tile.direction == nextTileKey)
                    level = tile.levelZ;
                else
                    continue;
                Vector3Int doubleCheckTilePosition = currentTilePosition.grid.WorldToCell(doubleCheckPosition);

                // IN AIR! ----------------------------------------------------------------------------------------------------
                // I don't care what height the tile is at as long as the sprite is in the air and has a y above the tile height
                if (tile.direction == nextTileKey && !isGrounded && Mathf.Abs(itemObject.localPosition.z) >= level)
                {

                    ChangeObjectLocation(nextTileKey.x, nextTileKey.y, level);

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
                        
                        if (tile.levelZ > 0)
                            getOffSlope = true;
                    }

                }

                // the next tile is NOT valid
                if (tile.direction == nextTileKey && !tile.isValid)
                {
                    

                    // If I am on a slope, am i approaching or leaving the slope in a valid direction?
                    if (onSlope)
                    {
                        if (tile.direction == Vector3Int.zero && tile.tileName.Contains("X") && nextTileKey.x != 0 || tile.direction == Vector3Int.zero && tile.tileName.Contains("Y") && nextTileKey.y != 0)
                            continue;
                    }

                    // This is where we are at the top of a cliff
                    if (tile.levelZ <= 0)
                    {
                        ChangeObjectLocation(nextTileKey.x, nextTileKey.y, level);
                        return true;
                    }

                    CheckBounce();

                    // This is where we hit a wall of height 1 or above
                    return false;
                }
            }

            ChangeObjectLocation(nextTileKey.x, nextTileKey.y, level);

            return true;
        }

        void ChangeObjectLocation(int x, int y, int z)
        {

            var newPos = new Vector3Int(x, y, z);
            currentTilePosition.position += newPos;
            
        }



        public void AddMovement(Vector2 newDirection, float _velocity)
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
            
                if (gravityItem == this || gravityItem.currentLevel != currentLevel || gravityItem.itemObject.localPosition.z > displacement.positionZ|| itemObject.localPosition.z > gravityItem.itemObject.localPosition.z)
                    return;
                Vector2 direction = transform.position - collision.transform.position;
                float addedVelocity = gravityItem.currentVelocity * 0.04f;
                AddMovement(direction, gravityItem.currentVelocity != 0 ? gravityItem.currentVelocity + addedVelocity : velocity * itemBounceFriction);
                if (gravityItem.addUpsies)
                {
                    bounceFactor = 1;
                    Bounce(gravityItem.currentVelocity * 6);
                }
                
            }

        }

    }
}
