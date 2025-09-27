using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    
    public BoidManager boidManager;
    
    public float neighborDistanceFarthest;
    public float neighborDistanceClosest;

    
    public Vector2 currentDirection;
    public bool inBoidPool;


    public Vector2 SteerBoid(Vector2 _currentDirection, float roamingDistance)
    {
        currentDirection = _currentDirection;
        Vector2 result = Vector2.zero;
        Vector2 desiredDirection = Vector2.zero;
        Vector2 desiredPosition = Vector2.zero;
        Vector2 desiredAvoidance = Vector2.zero;
        int total = 0;

        Vector2 currentBoidPosition = (Vector2)transform.position; 

        foreach (var boid in boidManager.allBoids)
        {
            if (boid == this || !boid.inBoidPool)
                continue;

            var boidPos = boid.transform.position;

            var sqrDist = ((Vector2)boidPos - currentBoidPosition).sqrMagnitude;
            if (sqrDist <= neighborDistanceFarthest * neighborDistanceFarthest)
            {
                desiredDirection += boid.currentDirection;
                desiredPosition += (Vector2)boidPos;

                if (sqrDist <= neighborDistanceClosest * neighborDistanceClosest)
                {
                    Vector2 avoidDirection = (currentBoidPosition - (Vector2)boidPos).normalized;
                    desiredAvoidance += avoidDirection;
                }

                total++;
            }
        }

        if (total > 1)
        {
            desiredDirection = (desiredDirection / total) - _currentDirection;
            desiredPosition = (desiredPosition / total) - currentBoidPosition;
            desiredAvoidance /= total;

            result = desiredDirection + desiredPosition + desiredAvoidance;

            currentDirection = Vector2.Lerp(currentDirection, currentDirection + result, Time.deltaTime);
        }

        float distF = NumberFunctions.GetDistanceV2((Vector2)boidManager.currentDestination.position, currentBoidPosition);

        if (distF >= roamingDistance * roamingDistance)
        {
            currentDirection += ((Vector2)boidManager.currentDestination.position - currentBoidPosition).normalized;
        }
        else if (distF <= 0.0001f) 
        {
            currentDirection += (currentBoidPosition - (Vector2)boidManager.currentDestination.position).normalized;
        }
        

        currentDirection = currentDirection.normalized;
        return currentDirection;
    }


}
