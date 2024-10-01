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

        Vector2 currentBoidPosition = (Vector2)transform.position; // Cache position

        foreach (var boid in boidManager.allBoids)
        {
            if (boid == this || !boid.inBoidPool)
                continue;

            var sqrDist = ((Vector2)boid.transform.position - currentBoidPosition).sqrMagnitude;
            if (sqrDist <= neighborDistanceFarthest * neighborDistanceFarthest)
            {
                desiredDirection += boid.currentDirection;
                desiredPosition += (Vector2)boid.transform.position;

                if (sqrDist <= neighborDistanceClosest * neighborDistanceClosest)
                {
                    Vector2 avoidDirection = currentBoidPosition - (Vector2)boid.transform.position;
                    avoidDirection /= Mathf.Sqrt(sqrDist); // Only take the square root when necessary
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

            // Use lerp to smooth the direction change
            currentDirection = Vector2.Lerp(currentDirection, currentDirection + result, Time.deltaTime);
        }

        float distF = Vector2.Distance(boidManager.currentDestination.position, currentBoidPosition);

        if (distF >= roamingDistance)
        {
            Vector2 center = (Vector2)boidManager.currentDestination.position - currentBoidPosition;
            currentDirection += center / distF;
        }
        else if (distF <= .01f)
        {
            Vector2 center = currentBoidPosition - (Vector2)boidManager.currentDestination.position;
            currentDirection += center / distF;
        }

        currentDirection = currentDirection.normalized;
        return currentDirection;
    }


}
