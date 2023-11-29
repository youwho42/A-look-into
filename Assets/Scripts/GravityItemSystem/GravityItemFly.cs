using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Klaxon.GravitySystem
{
    public class GravityItemFly : GravityItemNew
    {
        public DrawZasYDisplacement centerOfActiveArea;

        public float flightSpeed;
        public float flyRoamingDistance;
        public Vector2 minMaxFlyZ;
        [HideInInspector]
        public Vector2 currentDestination;
        [HideInInspector]
        public Vector3 mainDestinationZ;
        [HideInInspector]
        public Vector3 currentDestinationZ;

        //[HideInInspector]
        //public Vector2 currentDirection;
        [HideInInspector]
        public Vector3 currentDirectionZ;
        [HideInInspector]
        public bool isLanding;
        [HideInInspector]
        public Tilemap waterMap;
        Vector3Int nextTilePosition;
        public SpriteRenderer characterRenderer;

        //const float spriteDisplacementY = 0.2790625f;
        [HideInInspector]
        public bool isOverWater;
        [HideInInspector]
        public bool useBoids;
        public Boid boid;
        public bool facingRight;
        [HideInInspector]
        public bool canReachNextTile;

        float lastTravelDirection;
        float travelDirectionTimer;
        Vector3 lastPosition;
        int framesStuck;
        [HideInInspector]
        public bool isStuck;
        [HideInInspector]
        public bool hasDeviatePosition;
        public bool shitSpot;

        public override void Start()
        {
            base.Start();

            SetWaterMap();

            var s = flightSpeed / 10;
            flightSpeed += Random.Range(-s, s);

        }

        public override void Update()
        {
            base.Update();

            CheckOverWater();
            SetFacingDirection();

            if (CanReachNextTile(currentDirection))
            {
                canReachNextTile = true;

                SetDirectionZ();


                if (!useBoids)
                    SetDirection();
                else
                    currentDirection = boid.SteerBoid(currentDirection, flyRoamingDistance);


                float s = Mathf.Abs(currentDirection.y) > Mathf.Abs(currentDirection.x) ? flightSpeed * 0.9f : flightSpeed;
                MoveZ(currentDirectionZ, isLanding ? flightSpeed * 2 : flightSpeed);
                Move(currentDirection, s);

            }
            else
            {
                canReachNextTile = false;
            }

            if (onObstacle)
            {
                currentDestinationZ.y = Mathf.Clamp(currentDestinationZ.y, obstacleDisplacement.y, 20);
                currentDestinationZ.z = Mathf.Clamp(currentDestinationZ.z, obstacleDisplacement.z, 20);

                if (obstacleDisplacement.z > itemObject.localPosition.z)
                {
                    SetDirectionZ();
                    MoveZ(currentDirectionZ, isLanding ? flightSpeed * 2 : flightSpeed);
                }
                    
            }
            else
                currentDestinationZ = mainDestinationZ;

        }


        public new void FixedUpdate()
        {
            base.FixedUpdate();
            
        }

        public void CheckOverWater()
        {

            SetWaterMap();
            Vector3 displace = mainDestinationZ;
            var d = currentTilePosition.position;
            isOverWater = false;
            d.z = 0;
            var tile = waterMap.GetTile(d);
            if (tile != null)
            {
                displace = new Vector3(0, mainDestinationZ.y + (spriteDisplacementY * 4), mainDestinationZ.z + 4);
                isOverWater = true;
            }

            currentDestinationZ = displace;
        }


        public bool CanReachNextTile(Vector2 direction)
        {
            if (currentTilePosition.grid == null)
                return false;

            Vector3 checkPosition = (transform.position + (Vector3)direction * checkTileDistance) - Vector3.forward;
            Vector3 doubleCheckPosition = transform.position - Vector3.forward;


            nextTilePosition = currentTilePosition.grid.WorldToCell(checkPosition);

            Vector3Int nextTileKey = nextTilePosition - currentTilePosition.position;
            if (CheckForObstacles(checkPosition, doubleCheckPosition, direction, nextTileKey))
                return false;
            if (nextTileKey == Vector3Int.zero)
                return true;


            int level = 0;

            TileInfoRequestManager.RequestTileInfo(currentTilePosition.position, TileFound);

            if (tileBlockInfo == null)
                return false;

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

                    if (Mathf.Abs(itemObject.localPosition.z) >= level)
                    {
                       currentTilePosition.position += new Vector3Int(nextTileKey.x, nextTileKey.y, level);
                        if (level < -4)
                            return false;
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
                        //if (tile.tileName.Contains("X") && nextTileKey.x == 0 || tile.tileName.Contains("Y") && nextTileKey.y == 0)
                        //    return false;

                        onSlope = true;

                        // is the slope is lower?
                        if (tile.levelZ < 0)
                            getOnSlope = true;


                    }

                    // I am on a slope
                    if (onSlope)
                    {
                        //am i walking 'off' the slope on the upper part in the right direction?
                        //if (tile.direction == Vector3Int.zero && tile.tileName.Contains("X") && nextTileKey.x == 0 || tile.direction == Vector3Int.zero && tile.tileName.Contains("Y") && nextTileKey.y == 0)
                        //{
                        //    //onCliffEdge = true;
                        //    return false;
                        //}
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

                    // This is checking if we are on that shit spot inbetween tiles 
                    if (Mathf.Abs(tile.levelZ) >= 1)
                    {
                        float difference = Mathf.Abs(_transform.position.y - currentDestination.y);
                        if (difference < spriteDisplacementY * Mathf.Abs(tile.levelZ))
                        {
                            shitSpot = true;
                        }
                    }

                    // This is where we are on top of a cliff
                    /*if (tile.Value.levelZ <= 0)
                    {
                        return false;
                    }*/

                    // This is where we hit a wall of height 1 or above
                    return false;
                }
            }

            currentTilePosition.position += new Vector3Int(nextTileKey.x, nextTileKey.y, level);



            return true;
        }

        


        public void SetFacingDirection()
        {

            
            if (currentDirection.x != 0)
            {
                bool ff = IsFacingForward();
                if (!ff)
                    travelDirectionTimer += Time.deltaTime;
                else
                    travelDirectionTimer = 0;

                if(travelDirectionTimer >= 0.1f)
                {
                    if (!ff)
                        Flip();
                    
                    travelDirectionTimer = 0;
                }
            }
        }
        
        bool IsFacingForward()
        {
            return currentDirection.x < 0f && !facingRight || currentDirection.x > 0f && facingRight;
        }

        public void Flip()
        {
            // Switch the way the player is labelled as facing
            facingRight = !facingRight;

            // Multiply the player's x local scale by -1
            Vector3 theScale = _transform.localScale;
            theScale.x *= -1;
            _transform.localScale = theScale;
        }

        public void SetRandomDestinationZ()
        {

            float randZ = Random.Range(minMaxFlyZ.x, minMaxFlyZ.y);
            mainDestinationZ = new Vector3(0, spriteDisplacementY * randZ, randZ);
            currentDestinationZ = mainDestinationZ;
            SetDirectionZ();
        }


        public void SetRandomDestination()
        {
            
            if (currentTilePosition == null || currentTilePosition.groundMap == null)
                return;
            var lastDestination = currentDestination;
            Vector2 rand = (Random.insideUnitCircle * flyRoamingDistance);
            Vector3 centerPos = centerOfActiveArea == null ? transform.position : centerOfActiveArea.transform.position;
            var possiblePos = new Vector3(centerPos.x + rand.x, centerPos.y + rand.y, 0 /*centerPos.z - 1*/);
            var d = currentTilePosition.groundMap.WorldToCell(possiblePos);
            for (int z = currentTilePosition.groundMap.size.z; z >= -5; z--)
            {
                d.z = z;
                if (currentTilePosition.groundMap.GetTile(d) != null)
                {
                    //var p = currentTilePosition.groundMap.GetCellCenterWorld(d);
                    possiblePos.z = z+1;
                    var hit = Physics2D.OverlapPoint(possiblePos, obstacleLayer, z, z);
                    if (!hit)
                    {
                        currentDestination = possiblePos;
                        break;
                    }
                    
                }

            }
            if(lastDestination == currentDestination)
            {
                SetRandomDestination();
                return;
            }
            SetRandomDestinationZ();
            SetDirection();
        }

        public void SetDirection()
        {
            var dir = currentDestination - (Vector2)_transform.position;
            dir = dir.normalized;
            currentDirection = dir;

        }
        public void SetDirectionZ()
        {
            currentDirectionZ = currentDestinationZ - itemObject.localPosition;
        }


        public void SetDestination(DrawZasYDisplacement destination)
        {
            currentDestination = destination.transform.position;
            mainDestinationZ = destination.displacedPosition;
            currentDestinationZ = mainDestinationZ;
            SetDirection();
            SetDirectionZ();
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


        public void FindDeviateDestination(int rays)
        {

            List<Vector2> directions = new List<Vector2>();
            float angleIncrement = 360f / rays;
            for (int i = 0; i < rays; i++)
            {
                Vector2 dir = Quaternion.Euler(0f, 0f, i * angleIncrement) * currentDirection;

                RaycastHit2D hit = Physics2D.Raycast(_transform.position, dir, 0.5f, obstacleLayer);
                if (hit.collider == null)
                {
                    if (dir != currentDirection)
                    {
                        dir = dir.normalized;
                        directions.Add(dir);
                    }
                }
            }
            if (directions.Count == 0)
            {
                int ra = (int)Mathf.Sign(Random.Range(-1.0f, 1.0f));
                int rb = (int)Mathf.Sign(Random.Range(-1.0f, 1.0f));
                currentDestination = (Vector2)_transform.position - new Vector2(ra * currentDirection.y, rb * currentDirection.x) * 0.15f;
                SetDirection();
                return;
            }

            if (framesStuck >= 12)
            {

                
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


        public float CheckDistanceToDestination()
        {
            float dist = Vector2.Distance(_transform.position, currentDestination);

            return dist;
        }




        void SetWaterMap()
        {
            if (waterMap != null)
                return;

            var grid = FindObjectOfType<Grid>();
            Tilemap[] maps = grid.GetComponentsInChildren<Tilemap>();
            foreach (var map in maps)
            {
                if (map.gameObject.name == "WaterTiles")
                {
                    waterMap = map;
                }
            }
        }
    } 
}
