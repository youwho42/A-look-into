using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.GravitySystem
{
    public class GravityItemMovementControllerNew : GravityItemNew
    {

        [Space]
        [Header("Player Movement")]
        public float walkSpeed;
        public float runSpeed;
        public float jumpHeight;
        PlayerInput playerInput;
        public bool facingRight;
        public bool isInInteractAction;
        [HideInInspector]
        public float moveSpeed;


        Vector3 checkPosition;
        Vector3 doubleCheckPosition;


        DetectVisibility visibilityCheck;


        Vector3Int currentPosition;

        Vector3Int nextTilePosition;
        [HideInInspector]
        public bool onCliffEdge;
        WorldObjectAudioManager audioManager;


        private new IEnumerator Start()
        {
            base.Start();

            visibilityCheck = GetComponent<DetectVisibility>();
            audioManager = GetComponentInChildren<WorldObjectAudioManager>();
            playerInput = GetComponent<PlayerInput>();

            yield return new WaitForSeconds(0.25f);


            
            isGrounded = true;

        }



        private new void Update()
        {
            base.Update();



            if (playerInput.movement.x != 0 && !isInInteractAction)
            {
                if (playerInput.movement.x > 0.01f && !facingRight)
                    Flip();
                else if (playerInput.movement.x < 0.01f && facingRight)
                    Flip();
            }

            if (isGrounded && playerInput.isJumping)
                Bounce(jumpHeight);

            moveSpeed = playerInput.movement.x + playerInput.movement.y;




        }


        public new void FixedUpdate()
        {
            base.FixedUpdate();
            
            if (isInInteractAction)
                return;

            if (CanReachNextTile(playerInput.movement))
            {
                
                Move(playerInput.movement, (playerInput.isRunning ? runSpeed : walkSpeed));
            }


        }


        void TileFound(List<TileDirectionInfo> tileBlock, bool success)
        {
            if (success)
                tileBlockInfo = tileBlock;
            else
                Debug.LogError("Tile not found in tile dictionary!");
        }

        bool CanReachNextTile(Vector2 direction)
        {


            checkPosition = (transform.position + (Vector3)direction * checkTileDistance) - Vector3.forward;
            doubleCheckPosition = transform.position - Vector3.forward;
            if (CheckForObstacles(checkPosition, doubleCheckPosition, direction))
                return false;
            
            nextTilePosition = currentTilePosition.grid.WorldToCell(checkPosition);

            Vector3Int nextTileKey = nextTilePosition - currentTilePosition.position;
            onCliffEdge = false;


            int level = 0;

            TileInfoRequestManager.RequestTileInfo(currentTilePosition.position, TileFound);

            if (tileBlockInfo == null)
                return true;

            

            foreach (var tile in tileBlockInfo)
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



                // JUMPING! ----------------------------------------------------------------------------------------------------
                // I don't care what height the tile is at as long as the sprite is jumping and has a y above the tile height
                if (tile.direction == nextTileKey)
                {
                    if (tile.levelZ < -2)
                    {
                        onCliffEdge = true;
                        return false;
                    }

                    if (/*isGrounded && playerInput.canWalkOffCliff && level <= 0 || */!isGrounded && Mathf.Abs(itemObject.localPosition.z) >= level)
                    {
                        //currentTilePosition.position += new Vector3Int(nextTileKey.x, nextTileKey.y, level);
                        ChangePlayerLocation(nextTileKey.x, nextTileKey.y, level);

                        if (tile.tileName.Contains("Slope"))
                            onSlope = true;

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

                        onSlope = true;

                        // is the slope is lower?
                        if (tile.levelZ < 0)
                            getOnSlope = true;


                    }

                    // I am on a slope
                    if (onSlope)
                    {
                        //am i walking 'off' the slope on the upper part in the right direction?
                        if (tile.direction == Vector3Int.zero && tile.tileName.Contains("X") && nextTileKey.x == 0 || tile.direction == Vector3Int.zero && tile.tileName.Contains("Y") && nextTileKey.y == 0)
                        {
                            onCliffEdge = true;
                            return false;
                        }
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

                    // If I am approaching a slope, am i approaching or leaving the slope in a valid direction?
                    if (onSlope)
                    {
                        if (tile.direction == Vector3Int.zero && tile.tileName.Contains("X") && nextTileKey.x != 0 || tile.direction == Vector3Int.zero && tile.tileName.Contains("Y") && nextTileKey.y != 0)
                            continue;
                    }

                    if (tile.levelZ == -1)
                    {
                        //currentTilePosition.position += new Vector3Int(nextTileKey.x, nextTileKey.y, level);
                        ChangePlayerLocation(nextTileKey.x, nextTileKey.y, level);

                        return true;
                    }
                    // This is where we are on top of a cliff
                    if (tile.levelZ <= -1)
                    {
                        onCliffEdge = true;
                    }

                    

                    // This is where we hit a wall of height 1 or above
                    return false;
                }
            }

            //currentTilePosition.position += new Vector3Int(nextTileKey.x, nextTileKey.y, level);
            ChangePlayerLocation(nextTileKey.x, nextTileKey.y, level);


            return true;
        }


        public override void JustLanded()
        {
            audioManager.PlayFootstepSound();
            
        }


        void ChangePlayerLocation(int x, int y, int z)
        {

            var newPos = new Vector3Int(x, y, z);
            currentTilePosition.position += newPos;
            if (newPos != Vector3Int.zero)
                GameEventManager.onPlayerPositionUpdateEvent.Invoke();
        }


        
        public void Flip()
        {
            // Switch the way the player is labelled as facing
            facingRight = !facingRight;

            // Multiply the player's x local scale by -1
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }




        public Vector3 GetTileWorldPosition(Vector3Int tile)
        {
            var tileworldpos = currentTilePosition.groundMap.GetCellCenterWorld(tile);
            return tileworldpos;
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(slopeCheckPosition, 0.05f);
            Gizmos.DrawRay(slopeCheckPosition, slopeDirection * 0.6f);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(slopeCollisionPoint, 0.05f);

            if (tileBlockInfo.Count > 0)
            {


                if (tileBlockInfo.Count > 0)
                {
                    foreach (var item in tileBlockInfo)
                    {
                        Gizmos.color = item.isValid ? Color.green : Color.red;

                        var p = item.direction + currentTilePosition.position;

                        Gizmos.DrawWireSphere(GetTileWorldPosition(p), 0.1f);
                    }
                    Gizmos.color = Color.white;
                    Gizmos.DrawWireSphere(GetTileWorldPosition(currentTilePosition.position), 0.1f);
                }
            }

        }




    }
}
