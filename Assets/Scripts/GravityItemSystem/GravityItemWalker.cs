using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

namespace Klaxon.GravitySystem
{
    public class GravityItemWalker : GravityItemNew
    {

        [Space]
        //[Header("Player Movement")]
        public float walkSpeed;
        public float runSpeed;
        public float jumpHeight;
        //PlayerInputController playerInput;
        public bool facingRight;
        [HideInInspector]
        public bool isInInteractAction;
        [HideInInspector]
        public float moveSpeed;
        [HideInInspector]
        public Vector2 currentDir;
        [HideInInspector]
        public Vector2 currentDestination;

        Vector3 checkPosition;
        Vector3 doubleCheckPosition;


        DetectVisibility visibilityCheck;
        [HideInInspector]
        public bool jumpAhead;

        Vector3Int currentPosition;

        Vector3Int nextTilePosition;
        //[HideInInspector]
        //public bool onCliffEdge;
        WorldObjectAudioManager audioManager;

        
        [HideInInspector]
        public bool hasDeviatePosition;
        
        Vector3 lastPosition;
        int framesStuck;
        public bool isStuck;
        public bool tilemapObstacle;

        private new IEnumerator Start()
        {
            base.Start();

            visibilityCheck = GetComponent<DetectVisibility>();
            audioManager = GetComponentInChildren<WorldObjectAudioManager>();
            //playerInput = GetComponent<PlayerInputController>();

            yield return new WaitForSeconds(0.25f);

            //GameEventManager.onJumpEvent.AddListener(Jump);

            isGrounded = true;

            
        }
        private void OnDisable()
        {
            //GameEventManager.onJumpEvent.RemoveListener(Jump);
        }



        private new void Update()
        {
            base.Update();



            if (currentDir.x != 0 && !isInInteractAction)
            {
                if (currentDir.x > 0.01f && !facingRight)
                    Flip();
                else if (currentDir.x < 0.01f && facingRight)
                    Flip();
            }


            moveSpeed = currentDir.x + currentDir.y;


            if (jumpAhead)
                Jump();

        }


        public new void FixedUpdate()
        {
            base.FixedUpdate();

            if (isInInteractAction || currentDir == Vector2.zero)
                return;

            if (CanReachNextTile(currentDir))
            {

                Move(currentDir,  walkSpeed);
            }


        }

        void Jump()
        {
            if (isGrounded /*&& !playerInput.isInUI*/)
                Bounce(jumpHeight);
            jumpAhead = false;
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
            tilemapObstacle = false;
            jumpAhead = false;
            checkPosition = (transform.position + (Vector3)direction * checkTileDistance) - Vector3.forward;
            doubleCheckPosition = transform.position - Vector3.forward;
            if (CheckForObstacles(checkPosition, doubleCheckPosition, direction))
                return false;

            nextTilePosition = currentTilePosition.grid.WorldToCell(checkPosition);

            Vector3Int nextTileKey = nextTilePosition - currentTilePosition.position;
            //onCliffEdge = false;


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
                        tilemapObstacle = true;
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
                        {
                            tilemapObstacle = true;
                            return false;
                        }

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
                            tilemapObstacle = true;
                            return false;
                        }
                        if (tile.levelZ > 0)
                            getOffSlope = true;
                    }

                }

                // the next tile is NOT valid
                if (tile.direction == nextTileKey && !tile.isValid)
                {


                    if (doubleCheckTilePosition == nextTilePosition && isGrounded)
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
                        ChangePlayerLocation(nextTileKey.x, nextTileKey.y, level);

                        return true;
                    }
                    if (tile.levelZ == 1 && !onSlope)
                    {
                        //currentTilePosition.position += new Vector3Int(nextTileKey.x, nextTileKey.y, level);
                        jumpAhead = true;

                        return true;
                    }

                    tilemapObstacle = true;
                    return false;
                }
            }

            ChangePlayerLocation(nextTileKey.x, nextTileKey.y, level);


            return true;
        }


        public void SetDirection()
        {
            var dir = currentDestination - (Vector2)transform.position;
            dir = dir.normalized;
            currentDir = dir;
        }


        public void FindDeviateDestination(int rays)
        {
            List<Vector2> directions = new List<Vector2>();
            float angleIncrement = 360f / rays;

            for (int i = 0; i < rays; i++)
            {
                Vector2 direction = Quaternion.Euler(0f, 0f, i * angleIncrement) * currentDir;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.5f, obstacleLayer);
                if (hit.collider == null)
                {
                    if (direction != currentDir)
                    {
                        direction = direction.normalized;
                        directions.Add(direction);
                    }
                }
            }

            float best = -1;
            int bestIndex = 0;
            int secondBestIndex = 0;
            for (int i = 0; i < directions.Count; i++)
            {
                float dot = Vector2.Dot(currentDir, directions[i]);
                if (dot > best)
                {
                    best = dot;
                    secondBestIndex = bestIndex;
                    bestIndex = i;
                }
            }
            currentDestination = (Vector2)transform.position + directions[secondBestIndex] * .3f;
            Debug.DrawLine(transform.position, currentDestination, Color.green);
            hasDeviatePosition = true;
        }

        public void SetLastPosition()
        {
            if (lastPosition != transform.position)
            {
                framesStuck = 0;
                isStuck = false;
                lastPosition = transform.position;
            }
            else
            {
                framesStuck++;
                if (framesStuck == 4)
                    isStuck = true;
            }
        }

        public void ResetLastPosition()
        {
            framesStuck = 0;
            isStuck = false;
        }

        public float CheckDistanceToDestination()
        {
            float dist = Vector2.Distance(transform.position, currentDestination);

            return dist;
        }

        public override void JustLanded()
        {
            if(audioManager != null) 
                audioManager.PlayFootstepSound();

        }


        void ChangePlayerLocation(int x, int y, int z)
        {

            var newPos = new Vector3Int(x, y, z);
            currentTilePosition.position += newPos;
            if (newPos != Vector3Int.zero)
                GameEventManager.onPlayerPositionUpdateEvent.Invoke();
        }

        public void SetFacingDirection(Vector3 direction)
        {
            var dir = Mathf.Sign(direction.x);
            
            if (transform.localScale.x != dir)
            {
                Flip();
            }
            
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


        public void SetRandomDestination(float roamingDistance)
        {

            Vector2 rand = (UnityEngine.Random.insideUnitCircle * roamingDistance);
            var d = currentTilePosition.groundMap.WorldToCell(new Vector2(transform.position.x + rand.x, transform.position.y + rand.y));
            for (int z = currentTilePosition.groundMap.cellBounds.zMax; z > currentTilePosition.groundMap.cellBounds.zMin - 1; z--)
            {
                d.z = z;
                if (currentTilePosition.groundMap.GetTile(d) != null)
                {

                    currentDestination = currentTilePosition.groundMap.GetCellCenterWorld(d);
                    currentDestination += new Vector2(UnityEngine.Random.Range(0f, .3f), UnityEngine.Random.Range(0f, .3f));
                    break;
                }
            }
        }

        public void SetWorldDestination(Vector3 destination)
        {
            currentDestination = destination;
            
        }

    }
}