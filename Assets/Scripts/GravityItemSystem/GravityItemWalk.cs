
using System.Collections.Generic;

using UnityEngine;

namespace Klaxon.GravitySystem
{
    public class GravityItemWalk : GravityItemNew
    {

        [Space]
        public DrawZasYDisplacement centerOfActivity;
        [Space]
        //[Header("Player Movement")]
        public float walkSpeed;
        public float runSpeed;
        public float jumpHeight;
        //PlayerInputController playerInput;

        [HideInInspector]
        public bool isInInteractAction;
        [HideInInspector]
        public float moveSpeed;
        //[HideInInspector]
        //public Vector2 currentDir;
        [HideInInspector]
        public Vector2 currentDestination;

        Vector3 checkPosition;
        Vector3 doubleCheckPosition;



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
        Vector3 lastValidPosition;

        int framesStuck;
        [HideInInspector]
        public bool isStuck;
        [HideInInspector]
        public bool tilemapObstacle;

        public bool canClimb;
        [HideInInspector]
        public bool isClimbing;
        [HideInInspector]
        public bool isRunning;

        [HideInInspector]
        public Vector3 mainDestinationZ;
        [HideInInspector]
        public Vector3 currentDestinationZ;
        [HideInInspector]
        public Vector3 currentDirectionZ;
        public SpriteRenderer characterRenderer;

        public bool shitSpot;



        public override void Start()
        {
            base.Start();

            audioManager = GetComponentInChildren<WorldObjectAudioManager>();
            isGrounded = true;
        }



        public override void Update()
        {

            base.Update();

            if (Mathf.Approximately(currentDirection.x, 0))
                currentDirection.x = 0;
            if (Mathf.Approximately(currentDirection.y, 0))
                currentDirection.y = 0;

            if (isInInteractAction || currentDirection == Vector2.zero && !isClimbing)
                return;
            var finalSpeed = isRunning ? runSpeed : walkSpeed;
            if (CanReachNextTile(currentDirection))
                Move(currentDirection, finalSpeed);





            if (!Mathf.Approximately(currentDirection.x, 0) && !isInInteractAction)
            {
                if (currentDirection.x > 0.01f && !facingRight)
                    Flip();
                else if (currentDirection.x < -0.01f && facingRight)
                    Flip();
            }

            if (canClimb && isClimbing)
            {

                SetDirectionZ();

                MoveZ(currentDirectionZ, walkSpeed * 2);
            }

            moveSpeed = currentDirection.x + currentDirection.y;


            if (jumpAhead)
                Jump();

            if (Mathf.Approximately(currentDirection.x, 0) || Mathf.Approximately(currentDirection.y, 0))
                isStuck = false;
        }


        void Jump()
        {
            if (isGrounded /*&& !playerInput.isInUI*/)
                Bounce(jumpHeight);
            jumpAhead = false;
        }



        bool CanReachNextTile(Vector2 direction)
        {
            shitSpot = false;
            tilemapObstacle = false;
            jumpAhead = false;
            checkPosition = (_transform.position + (Vector3)direction * checkTileDistance) - Vector3.forward;
            doubleCheckPosition = _transform.position - Vector3.forward;


            nextTilePosition = currentTilePosition.grid.WorldToCell(checkPosition);

            Vector3Int nextTileKey = nextTilePosition - currentTilePosition.position;
            //onCliffEdge = false;
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

                if (nextTilePosition == currentTilePosition.position)
                    return true;

                

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
                            return true;
                    }

                    // This is checking if we are on that shit spot inbetween tiles 
                    if (Mathf.Abs(tile.levelZ) >= 1)
                    {
                        float difference = Mathf.Abs(_transform.position.y - currentDestination.y);
                        if (difference < spriteDisplacementY * Mathf.Abs(tile.levelZ))
                        {
                            shitSpot = true;
                        }
                    }

                    if (tile.levelZ == -1 || tile.levelZ == -2)
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
            var dir = currentDestination - (Vector2)_transform.position;
            dir = dir.normalized;
            currentDirection = dir;
        }


        public void FindDeviateDestination(int rays)
        {

            List<Vector2> directions = new List<Vector2>();
            float angleIncrement = 360f / rays;
            for (int i = 0; i < rays; i++)
            {
                Vector2 dir = Quaternion.Euler(0f, 0f, i * angleIncrement) * currentDirection;

                RaycastHit2D hit = Physics2D.Raycast(_transform.position, dir, 0.5f, obstacleLayer);
                bool ignoreCollider = false;
                if (hit.collider != null)
                {
                    if (hit.collider.TryGetComponent(out DrawZasYDisplacement displacement))
                    {
                        if (displacement.positionZ <= 0)
                            ignoreCollider = true;
                    }
                }
                if (hit.collider == null || ignoreCollider)
                {

                    var d = _transform.position + ((Vector3)dir * .6f);

                    if (dir != currentDirection)
                    {
                        dir = dir.normalized;
                        directions.Add(dir);
                    }
                }
            }
            //if (directions.Count == 0)
            //{
            //    do
            //    {
            //        int ra = (int)Mathf.Sign(Random.Range(-1.0f, 1.0f));
            //        int rb = (int)Mathf.Sign(Random.Range(-1.0f, 1.0f));
            //        currentDestination = (Vector2)_transform.position - new Vector2(ra * currentDirection.y, rb * currentDirection.x) * 0.15f;

            //    } while (!GridManager.instance.GetTileValid(new Vector3(currentDestination.x, currentDestination.y, _transform.position.z)));
            //    //int ra = (int)Mathf.Sign(Random.Range(-1.0f, 1.0f));
            //    //int rb = (int)Mathf.Sign(Random.Range(-1.0f, 1.0f));
            //    //currentDestination = (Vector2)_transform.position - new Vector2(ra * currentDirection.y, rb * currentDirection.x) * 0.15f;
            //    SetDirection();
            //    return;
            //}

            if (framesStuck >= 30 || directions.Count == 0)
            {

                jumpAhead = true;
                int ra = Random.value < .5 ? 1 : -1;
                int rb = Random.value < .5 ? 1 : -1;
                float d = Random.Range(0.05f, 0.25f);
                currentDestination = (Vector2)_transform.position - new Vector2(ra * currentDirection.y, rb * currentDirection.x) * d;
                hasDeviatePosition = true;
                SetDirection();
                framesStuck = 0;
                return;
            }
            float best = -1;
            int bestIndex = 0;
            int secondBestIndex = 0;
            for (int i = 0; i < directions.Count; i++)
            {
                float dot = Vector2.Dot(currentDirection, directions[i]);
                if (dot > best)
                {
                    best = dot;
                    secondBestIndex = bestIndex;
                    bestIndex = i;
                }
            }

            currentDestination = (Vector2)_transform.position + directions[secondBestIndex] * .3f;
            SetDirection();
            hasDeviatePosition = true;
        }

        public void SetLastPosition()
        {

            if (!LevelManager.instance.inPauseMenu)
            {
                if (lastPosition != _transform.position)
                {
                    ResetLastPosition();
                    lastPosition = _transform.position;
                }
                else
                {
                    framesStuck++;
                    if (framesStuck >= 4)
                        isStuck = true;
                }
            }
        }

        public void ResetLastPosition()
        {
            framesStuck = 0;
            isStuck = false;
        }

        public float CheckDistanceToDestination()
        {

            float dist = Vector2.Distance(_transform.position, currentDestination);

            return dist;
        }

        public override void JustLanded()
        {
            if (audioManager != null)
                audioManager.PlayFootstepSound();

        }


        void ChangePlayerLocation(int x, int y, int z)
        {

            var newPos = new Vector3Int(x, y, z);
            currentTilePosition.position += newPos;

        }

        public void SetFacingDirection(Vector3 direction)
        {
            var dir = Mathf.Sign(direction.x);

            if (_transform.localScale.x != dir)
            {
                Flip();
            }

        }





        public Vector3 GetTileWorldPosition(Vector3Int tile)
        {
            var tileworldpos = currentTilePosition.groundMap.GetCellCenterWorld(tile);
            return tileworldpos;
        }


        public void SetRandomDestination(float roamingDistance)
        {

            Vector2 rand = (Random.insideUnitCircle * roamingDistance);
            Vector3 center = centerOfActivity == null ? _transform.position : centerOfActivity.transform.position;
            var d = currentTilePosition.groundMap.WorldToCell(new Vector2(center.x + rand.x, center.y + rand.y));
            for (int z = currentTilePosition.groundMap.cellBounds.zMax; z > currentTilePosition.groundMap.cellBounds.zMin - 1; z--)
            {
                d.z = z;
                if (currentTilePosition.groundMap.GetTile(d) != null)
                {

                    currentDestination = GetTileWorldPosition(d);
                    currentDestination += new Vector2(Random.Range(-.2f, .2f), Random.Range(-.2f, .2f));
                    break;
                }
            }
        }



        public void SetWorldDestination(Vector3 destination)
        {
            if (shitSpot)
            {
                destination = transform.position;
                shitSpot = false;
            }
            currentDestination = destination;

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

            currentDirectionZ = currentDestinationZ - itemObject.localPosition;

        }


    }

}
