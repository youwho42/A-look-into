using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Klaxon.GravitySystem
{
    public class CanReachTileFlight : MonoBehaviour
    {
        GravityItemNew gravityItem;


        public DrawZasYDisplacement centerOfActiveArea;



        public float flightSpeed;
        public float flyRoamingDistance;
        public Vector2 minMaxFlyZ;
        [HideInInspector]
        public Vector2 currentDestination;
        [HideInInspector]
        public Vector3 mainDestinationZ;
        public Vector3 currentDestinationZ;

        [HideInInspector]
        public Vector2 currentDirection;
        [HideInInspector]
        public Vector3 currentDirectionZ;
        bool onSlope;
        public bool isLanding;
        public Tilemap waterMap;
        Vector3Int nextTilePosition;
        public SpriteRenderer characterRenderer;

        const float spriteDisplacementY = 0.2790625f;
        public bool isOverWater;
        public bool useBoids;
        public Boid boid;

        public bool canReachNextTile;


        Vector2 lastPos;


        private void Start()
        {

            gravityItem = GetComponent<GravityItemNew>();

            SetWaterMap();
            SetRandomDirection();
            if (boid != null)
                boid.currentDirection = currentDirection;

            var s = flightSpeed / 10;
            flightSpeed += UnityEngine.Random.Range(-s, s);
        }


        public void Fly()
        {
            
            CheckOverWater();
            //if (lastPos != (Vector2)transform.position)
            //    lastPos = transform.position;
            //else
            //{
            //    if(Selection.Contains(gameObject))
            //        Debug.Log("still and in flight");
            //}
                

            if (CanReachNextTile(currentDirection))
            {
                canReachNextTile = true;
                gravityItem.isWeightless = true;
                SetDirectionZ();

                if (!useBoids)
                    SetDirection();
                else
                    SetFacingDirection(currentDirection);

                gravityItem.MoveZ(currentDirectionZ, isLanding ? flightSpeed * 3 : flightSpeed);
                gravityItem.Move(currentDirection, flightSpeed);

                if (useBoids)
                    currentDirection = boid.SteerBoid(currentDirection, flyRoamingDistance);
            }
            else
            {
                canReachNextTile = false;
                if (useBoids)
                {
                    var desiredDirection = Vector2.Perpendicular(currentDirection) * Mathf.Sign(currentDirection.y);
                    desiredDirection -= currentDirection;
                    currentDirection += desiredDirection;
                    currentDirection = currentDirection.normalized;

                }
                else
                {
                    SetRandomDestination();
                }

                if (Vector2.Distance(gravityItem.itemObject.localPosition, currentDestinationZ) <= 0.01f)
                {
                    SetRandomDestination();
                    SetRandomDestinationZ();
                }

                gravityItem.MoveZ(currentDirectionZ, isLanding ? flightSpeed * 3 : flightSpeed);
            }

            if (!isLanding)
            {
                if (!useBoids)
                {
                    if (Vector2.Distance(transform.position, currentDestination) <= 0.01f)
                    {
                        SetRandomDestination();
                    }
                }
                else
                {
                    if (boid.currentDirection == Vector2.zero)
                        SetRandomDirection();
                }
                if (Vector2.Distance(gravityItem.itemObject.localPosition, currentDestinationZ) <= 0.01f)
                {
                    SetRandomDestinationZ();
                }
                if (!canReachNextTile)
                    SetRandomDestination();
            }
            

        }

        void TileFound(List<TileDirectionInfo> tileBlock, bool success)
        {
            if (success)
                gravityItem.tileBlockInfo = tileBlock;
        }



        public void CheckOverWater()
        {

            SetWaterMap();
            Vector3 displace = mainDestinationZ;
            var d = gravityItem.currentTilePosition.position;
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
            if (gravityItem.currentTilePosition.grid == null)
                return false;

            Vector3 checkPosition = (transform.position + (Vector3)direction * gravityItem.checkTileDistance) - Vector3.forward;
            Vector3 doubleCheckPosition = transform.position - Vector3.forward;
            

            nextTilePosition = gravityItem.currentTilePosition.grid.WorldToCell(checkPosition);

            Vector3Int nextTileKey = nextTilePosition - gravityItem.currentTilePosition.position;
            if (gravityItem.CheckForObstacles(checkPosition, doubleCheckPosition, direction, nextTileKey))
                return false;
            if (nextTileKey == Vector3Int.zero)
                return true;


            int level = 0;

            TileInfoRequestManager.RequestTileInfo(gravityItem.currentTilePosition.position, TileFound);

            if (gravityItem.tileBlockInfo == null)
                return false;

            foreach (var tile in gravityItem.tileBlockInfo)
            {
                // CURRENT TILE ----------------------------------------------------------------------------------------------------
                // right now, where we are, what it be? is it be a slope?
                if (tile.direction == Vector3Int.zero)
                {

                    gravityItem.slopeDirection = Vector2.zero;
                    onSlope = tile.tileName.Contains("Slope");
                    if (onSlope)
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

                    if (Mathf.Abs(gravityItem.itemObject.localPosition.z) >= level)
                    {
                        gravityItem.currentTilePosition.position += new Vector3Int(nextTileKey.x, nextTileKey.y, level);
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
                        if (tile.tileName.Contains("X") && nextTileKey.x == 0 || tile.tileName.Contains("Y") && nextTileKey.y == 0)
                            return false;

                        onSlope = true;

                        // is the slope is lower?
                        if (tile.levelZ < 0)
                            gravityItem.getOnSlope = true;


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
                    if (onSlope)
                    {
                        if (tile.direction == Vector3Int.zero && tile.tileName.Contains("X") && nextTileKey.x != 0 || tile.direction == Vector3Int.zero && tile.tileName.Contains("Y") && nextTileKey.y != 0)
                            continue;
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

            gravityItem.currentTilePosition.position += new Vector3Int(nextTileKey.x, nextTileKey.y, level);



            return true;
        }



        void SetRandomDirection()
        {
            currentDirection = new Vector2(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f));
            currentDirection = currentDirection.normalized;
            SetFacingDirection(currentDirection);
        }


        public void SetFacingDirection(Vector2 direction)
        {
            // Set facing direction
            //Vector2 dir = currentDestination - (Vector2)transform.position;
            var dir = Mathf.Sign(direction.x);
            characterRenderer.flipX = dir > 0;
        }

        public void SetRandomDestinationZ()
        {

            float randZ = UnityEngine.Random.Range(minMaxFlyZ.x, minMaxFlyZ.y);
            mainDestinationZ = new Vector3(0, spriteDisplacementY * randZ, randZ);
            currentDestinationZ = mainDestinationZ;
            SetDirectionZ();
        }


        public void SetRandomDestination()
        {
            if (gravityItem == null || gravityItem.currentTilePosition == null || gravityItem.currentTilePosition.groundMap == null)
                return;
            Vector2 rand = (UnityEngine.Random.insideUnitCircle * flyRoamingDistance);
            var d = gravityItem.currentTilePosition.groundMap.WorldToCell(new Vector2(centerOfActiveArea.transform.position.x + rand.x, centerOfActiveArea.transform.position.y + rand.y));
            for (int z = gravityItem.currentTilePosition.groundMap.size.z; z >= 0; z--)
            {
                d.z = z;
                if (gravityItem.currentTilePosition.groundMap.GetTile(d) != null)
                {
                    currentDestination = gravityItem.currentTilePosition.groundMap.GetCellCenterWorld(d);
                    break;
                }

            }


            SetDirection();
        }



        public void SetDestination(DrawZasYDisplacement destination, bool useZ)
        {
            currentDestination = destination.transform.position;
            if (useZ)
            {
                mainDestinationZ = destination.displacedPosition;
                currentDestinationZ = mainDestinationZ;
                SetDirectionZ();
            }
            else
            {
                SetRandomDestinationZ();
            }
            SetDirection();
        }

        public void SetDirectionZ()
        {

            currentDirectionZ = currentDestinationZ - gravityItem.itemObject.localPosition;

        }
        public void SetDirection()
        {
            currentDirection = currentDestination - (Vector2)transform.position;
            currentDirection = currentDirection.normalized;
            if (Vector2.Distance(currentDestination, transform.position) >= 0.01f)
                SetFacingDirection(currentDirection);
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


