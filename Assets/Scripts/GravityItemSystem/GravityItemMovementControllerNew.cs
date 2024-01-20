using Klaxon.StatSystem;
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
        [HideInInspector]
        public float finalSpeed;
        public float jumpHeight;
        PlayerInputController playerInput;
        public bool isInInteractAction;
        [HideInInspector]
        public float moveSpeed;
        

        Vector3 checkPosition;
        Vector3 doubleCheckPosition;

        public StatObject speedStat;
        public StatObject jumpStat;

        DetectVisibility visibilityCheck;


        Vector3Int currentPosition;

        Vector3Int nextTilePosition;
        [HideInInspector]
        public bool onCliffEdge;
        WorldObjectAudioManager audioManager;

        Vector3 lastValidPosition;


        private new IEnumerator Start()
        {
            base.Start();
            
            visibilityCheck = GetComponent<DetectVisibility>();
            audioManager = GetComponentInChildren<WorldObjectAudioManager>();
            playerInput = GetComponent<PlayerInputController>();

            yield return new WaitForSeconds(0.25f);

            GameEventManager.onJumpEvent.AddListener(Jump);
            
            isGrounded = true;

        }
        private void OnDisable()
        {
            GameEventManager.onJumpEvent.RemoveListener(Jump);
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


            moveSpeed = playerInput.movement.x + playerInput.movement.y;




        }


        public new void FixedUpdate()
        {
            base.FixedUpdate();
            
            if (isInInteractAction)
                return;

            if (CanReachNextTile(playerInput.movement))
            {
                finalSpeed = playerInput.isRunning ? runSpeed * speedStat.GetModifiedMax() : walkSpeed * speedStat.GetModifiedMax();
                Move(playerInput.movement, finalSpeed);
            }


        }

        void Jump()
        {
            if (!canJump)
                return;

            if (isGrounded && !playerInput.isInUI && !PlayerInformation.instance.isSitting)
                Bounce(jumpHeight * jumpStat.GetModifiedMax());
        }

        //void TileFound(List<TileDirectionInfo> tileBlock, bool success)
        //{
        //    if (success)
        //        tileBlockInfo = tileBlock;
        //    else
        //        Debug.LogError("Tile not found in tile dictionary!");
        //}

        bool CanReachNextTile(Vector2 direction)
        {
            

            checkPosition = (_transform.position + (Vector3)direction * checkTileDistance) - Vector3.forward;
            doubleCheckPosition = _transform.position - Vector3.forward;
            

            nextTilePosition = currentTilePosition.grid.WorldToCell(checkPosition);

            Vector3Int nextTileKey = nextTilePosition - currentTilePosition.position;
            onCliffEdge = false;

            if (CheckForObstacles(checkPosition, doubleCheckPosition, direction, nextTileKey))
                return false;

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
                    if (tile.isValid)
                        lastValidPosition = _transform.position;
                    else
                    {
                        _transform.position = lastValidPosition;
                        return false;
                    }

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
                    // this prevents the player from jumping off too high of a cliff
                    if (tile.levelZ < -2)
                    {
                        onCliffEdge = true;
                        return false;
                    }

                    if (!isGrounded && Mathf.Abs(itemObject.localPosition.z) >= level)
                    {
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
                        p.z += item.levelZ;
                        Gizmos.DrawWireSphere(GetTileWorldPosition(p), 0.1f);
                    }
                    Gizmos.color = Color.white;
                    Gizmos.DrawWireSphere(GetTileWorldPosition(currentTilePosition.position), 0.1f);
                }
            }

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Maze"))
                canJump = false;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Maze"))
                canJump = true;
        }


    }
}
