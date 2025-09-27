using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

namespace Klaxon.GravitySystem
{
	public class GravityItemSwim : GravityItemNew
	{

        public DrawZasYDisplacement centerOfActiveArea;

        public float swimSpeed;
        public float swimRoamingDistance;
        public Vector2 minMaxSwimZ;
        [HideInInspector]
        public Vector3 currentDestination;
        [HideInInspector]
        public Vector3 mainDestinationZ;
        [HideInInspector]
        public Vector3 currentDestinationZ;

        
        [HideInInspector]
        public Vector3 currentDirectionZ;
        
        [HideInInspector]
        public Tilemap waterMap;
        Vector3Int nextTilePosition;
        public SpriteRenderer characterRenderer;


        [HideInInspector]
        public bool useBoids;
        public Boid boid;
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

            var s = swimSpeed / 10;
            swimSpeed += Random.Range(-s, s);

        }

        public override void Update()
        {

            base.Update();

            SetFacingDirection();

            if (CanReachNextTile(currentDirection))
            {
                canReachNextTile = true;

                SetDirectionZ();

                if (!useBoids)
                    SetDirection();
                else
                    currentDirection = boid.SteerBoid(currentDirection, swimRoamingDistance);


                float s = Mathf.Abs(currentDirection.y) > Mathf.Abs(currentDirection.x) ? swimSpeed * 0.9f : swimSpeed;
                MoveZ(currentDirectionZ, swimSpeed);
                Move(currentDirection, s);

            }
            else
            {
                canReachNextTile = false;
                
            }

            //if (onObstacle)
            //{
            //    currentDestinationZ.y = Mathf.Clamp(currentDestinationZ.y, obstacleDisplacement.y, 20);
            //    currentDestinationZ.z = Mathf.Clamp(currentDestinationZ.z, obstacleDisplacement.z, 20);

            //    if (obstacleDisplacement.z > itemObject.localPosition.z)
            //    {
            //        SetDirectionZ();
            //        MoveZ(currentDirectionZ, swimSpeed);
            //    }

            //}
            //else
            //{
            //    if (!isOverWater)
            //        currentDestinationZ = mainDestinationZ;
            //}


        }


        //void TileFound(List<TileDirectionInfo> tileBlock, bool success)
        //{
        //    if (success)
        //        tileBlockInfo = tileBlock;
        //}


        public bool CanReachNextTile(Vector2 direction)
        {

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
                    Vector3Int waterZ = new Vector3Int(currentTilePosition.position.x, currentTilePosition.position.y, currentTilePosition.position.z + 3); ;
                    if (waterMap.GetTile(waterZ + tile.direction) == null)
                        return false;
                    //if (Mathf.Abs(itemObject.localPosition.z) >= level)
                    //{
                    //    currentTilePosition.position += new Vector3Int(nextTileKey.x, nextTileKey.y, level);

                    //    if (tile.tileName.Contains("Slope"))
                    //        onSlope = true;

                    //    return true;
                    //}
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
                            //onCliffEdge = true;
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

                    // If I am on a slope, am i approaching or leaving the slope in a valid direction?
                    if (onSlope)
                    {
                        if (tile.direction == Vector3Int.zero && tile.tileName.Contains("X") && nextTileKey.x != 0 || tile.direction == Vector3Int.zero && tile.tileName.Contains("Y") && nextTileKey.y != 0)
                            continue;
                    }

                    // This is where we are at he bottom of a cliff
                    if (tile.levelZ > 0)
                    {
                        return false;
                    }
                    // This is where we hit a wall of height 1 or above
                    return false;
                }
            }

            currentTilePosition.position += new Vector3Int(nextTileKey.x, nextTileKey.y, level);



            return true;
        }

        public void SetRandomDestination()
        {

            if (currentTilePosition == null || currentTilePosition.groundMap == null)
                return;
            var lastDestination = currentDestination;
            Vector2 rand = (Random.insideUnitCircle * (swimRoamingDistance * 0.33f));
            Vector3 centerPos = centerOfActiveArea == null ? transform.position : centerOfActiveArea.transform.position;
            var possiblePos = new Vector3(centerPos.x + rand.x, centerPos.y + rand.y, transform.position.z /*0*/ /*centerPos.z - 1*/);
            var d = currentTilePosition.groundMap.WorldToCell(possiblePos);
            for (int z = currentTilePosition.groundMap.size.z; z >= -5; z--)
            {
                d.z = z;
                if (currentTilePosition.groundMap.GetTile(d) != null)
                {
                    //var p = currentTilePosition.groundMap.GetCellCenterWorld(d);
                    possiblePos.z = z + 1;
                    var hit = Physics2D.OverlapPoint(possiblePos, obstacleLayer, z, z);
                    if (!hit && GridManager.instance.GetTileValid(possiblePos))
                    {
                        currentDestination = possiblePos;
                        break;
                    }

                }

            }
            if (lastDestination == currentDestination)
            {
                SetRandomDestination();
                return;
            }
            SetRandomDestinationZ();
            SetDirection();
        }

        public void SetRandomDestinationZ()
        {

            float randZ = Random.Range(minMaxSwimZ.x, minMaxSwimZ.y);
            mainDestinationZ = new Vector3(0, spriteDisplacementY * randZ, randZ);
            currentDestinationZ = mainDestinationZ;
            SetDirectionZ();
        }


        public void SetDirection()
        {
            Vector3 flierPos = _transform.position;
            if (isOverWater && flierPos.z != currentDestination.z)
            {
                float zOff = currentDestination.z - flierPos.z;
                zOff = spriteDisplacementY * zOff;
                flierPos.y += zOff;
            }

            var dir = (Vector2)currentDestination - (Vector2)flierPos;
            dir = dir.normalized;
            currentDirection = dir;

        }

        public void SetDirectionZ()
        {
            currentDirectionZ = currentDestinationZ - itemObject.localPosition;
            currentDirectionZ = currentDirectionZ.normalized;
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



        public void SetFacingDirection()
        {


            if (currentDirection.x != 0)
            {
                bool ff = IsFacingForward();
                if (!ff)
                    travelDirectionTimer += Time.deltaTime;
                else
                    travelDirectionTimer = 0;

                if (travelDirectionTimer >= 0.1f)
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

        void SetWaterMap()
        {
            if (waterMap != null)
                return;

            var grid = FindFirstObjectByType<Grid>();
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