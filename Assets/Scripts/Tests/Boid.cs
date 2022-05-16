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
        foreach (var boid in boidManager.allBoids)
        {
            if (boid == this || !boid.inBoidPool)
                continue;
            var dist = Vector2.Distance(boid.transform.position, transform.position);
            if (dist <= neighborDistanceFarthest)
            {
                desiredDirection += boid.currentDirection;
                desiredPosition += (Vector2)boid.transform.position;
                if (dist <= neighborDistanceClosest)
                {
                    Vector2 avoidDirection = transform.position - boid.transform.position;
                    avoidDirection /= dist;
                    desiredAvoidance += avoidDirection;
                }

                total++;
            }
        }
        if (total > 1)
        {
            
            desiredDirection /= total;
            desiredDirection -= _currentDirection;

            desiredPosition /= total;
            desiredPosition -= (Vector2)transform.position;

            desiredAvoidance /= total;



            result += desiredDirection;
            result += desiredPosition;
            result += desiredAvoidance;

            currentDirection += result;
            
        }

        float distF = Vector2.Distance(boidManager.currentDestination.position, transform.position);

        if (distF >= roamingDistance)
        {
            Vector2 center = boidManager.currentDestination.position - transform.position;
            currentDirection += center/distF;
            
        }
        else if (distF <= .01f)
        {
            Vector2 center = transform.position - boidManager.currentDestination.position;
            currentDirection += center/distF;
            
            
        }

        currentDirection = currentDirection.normalized;
        return currentDirection;
    }

   
}
