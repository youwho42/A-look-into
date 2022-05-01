using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ViewerAI : MonoBehaviour
{

    
    public float roamingSpeed;
    public float roamingDistance;
    public Tilemap groundMap;
    Vector2 currentDestination;
    Vector2 currentDirection;
    public Transform basePosition;
    public Vector2 minMaxStateTimeWait;
    float currentStateTimeWait;
    float timer;
    int currentLevel;

    public ViewerState currentState;
    public enum ViewerState
    {
        Random,
        Idle
    }
    private void Start()
    {
        transform.position = SetRandomPosition();
        SetRandomDestination();
        ChooseRandomState();
        SetCurrentLevel();
    }


    private void Update()
    {
        
        switch (currentState)
        {
            case ViewerState.Idle:
                timer += Time.deltaTime;
                if (timer >= currentStateTimeWait)
                {
                    SetRandomDestination();

                    
                    ChooseRandomState();
                }
                break;

            case ViewerState.Random:
                Move(currentDirection);
                SetCurrentLevel();
                
                if (Vector2.Distance(transform.position, currentDestination) <= 0.1f)
                {
                    
                    SetRandomDestination();

                    ChooseRandomState();
                }
                break;
        }
    }
    void SetCurrentLevel()
    {
        currentLevel = GetTileZ(transform.position).z;
        transform.position = new Vector3(transform.position.x, transform.position.y, currentLevel);
    }

    public Vector3Int GetTileZ(Vector3 position)
    {
        

        Vector3Int cellIndex = groundMap.WorldToCell(position - Vector3.forward);
        for (int i = groundMap.cellBounds.zMax; i > groundMap.cellBounds.zMin - 1; i--)
        {
            cellIndex.z = i;
            var tile = groundMap.GetTile(cellIndex);
            if (tile != null)
            {
                if (i < 0)
                    cellIndex.z = 1;
                return cellIndex;
            }


        }

        return cellIndex;

    }



    void ChooseRandomState()
    {
        timer = 0;
        int rand = UnityEngine.Random.Range(0, Enum.GetNames(typeof(ViewerState)).Length);
        currentStateTimeWait = UnityEngine.Random.Range(minMaxStateTimeWait.x, minMaxStateTimeWait.y);
        currentState = (ViewerState)rand;
    }

    public void Move(Vector2 dir)
    {
        
        Vector3 currentPosition = transform.position;
        currentPosition = Vector2.MoveTowards(transform.position, (Vector2)transform.position + dir, Time.deltaTime * roamingSpeed);
        
        transform.position = currentPosition;

    }

    public Vector2 SetRandomPosition()
    {
        Vector2 rand = (UnityEngine.Random.insideUnitCircle * 8);
        Vector2 newPosition = (Vector2)basePosition.position + rand;
        return newPosition;
    }

    public void SetRandomDestination()
    {

        Vector2 rand = (UnityEngine.Random.insideUnitCircle * (roamingDistance - 1)) + Vector2.one;
        var d = groundMap.WorldToCell(new Vector2(transform.position.x + rand.x, transform.position.y + rand.y));
        Vector2 newDestination = basePosition.position;
        for (int z = groundMap.size.z; z >= 0; z--)
        {
            d.z = z;
            if (groundMap.GetTile(d) != null)
            {
                newDestination = groundMap.GetCellCenterWorld(d);
                currentDestination = newDestination;
                
                SetDirection();
                break;
            }

        }
        currentDestination = newDestination;

        SetDirection();
    }

    public void SetDirection()
    {
        currentDirection = currentDestination - (Vector2)transform.position;
        
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, roamingDistance);
    }

}
