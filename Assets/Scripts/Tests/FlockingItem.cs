using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingItem : MonoBehaviour
{
    public float speed;
    public FlockManager flockManager;
    Vector2 averageDirection;
    Vector2 averagePosition;

    public float neighborDistanceFarthest;
    public float neighborDistanceClosest;
    public float maxCenterDistance;
    bool goingHome;
    private void Start()
    {
        speed += Random.Range(-.2f, .2f);
    }



    public Vector2 ApplyFlockingRules(Vector2 direction)
    {

        Vector2 vCenter = Vector2.zero;
        Vector2 vAvoid = Vector2.zero;
        
        Vector2 mainDestination = flockManager.mainDestination.position;

        float dist;

        int groupSize = 0;
        foreach (var item in flockManager.allAnimals)
        {
            if(item!=this)
            {

                dist = Vector2.Distance(item.transform.position, transform.position);
                if (dist <= neighborDistanceFarthest)
                {
                    vCenter += (Vector2)item.transform.position;
                    groupSize++;

                    if (dist <= neighborDistanceClosest)
                    {
                        vAvoid = vAvoid + (Vector2)(transform.position - item.transform.position);
                    }

                    
                }

                
            }
        }
        if (Vector2.Distance(transform.position, mainDestination) >= maxCenterDistance)
        {
           
            Vector2 newDirection = mainDestination - (Vector2)transform.position;

            return newDirection;
        }

        if (groupSize > 0)
        {
            
            vCenter = vCenter / groupSize + (mainDestination - (Vector2)transform.position);
            Vector2 newDirection = (vCenter + vAvoid) - (Vector2)transform.position;
            return newDirection;
        }
        
        
                
        

        return direction;
    }
}
