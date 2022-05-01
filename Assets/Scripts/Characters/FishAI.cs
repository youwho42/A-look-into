using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishAI : MonoBehaviour, IAnimal
{
    GravityItem gravityItem;
    public float roamingArea;
    float timeToStayAtDestination;
    CanReachTileSwim swim
        ;

    public Transform home;


    [SerializeField]
    public SwimmingState currentState;


    public enum SwimmingState
    {
        isSwimming,
        isAtDestination
    }

    private void Start()
    {
        gravityItem = GetComponent<GravityItem>();

        
        swim = GetComponent<CanReachTileSwim>();
        timeToStayAtDestination = SetTimeToStayAtDestination();
        SetHome(home);
        swim.SetRandomDestination();
        swim.SetRandomDestinationZ();
    }

   

    private void Update()
    {
        if (swim.centerOfActiveArea == null)
            SetHome(transform);

        //captureCollider.offset = gravityItem.itemObject.localPosition;

        switch (currentState)
        {
            case SwimmingState.isSwimming:

                swim.Swim();
                if (Vector2.Distance(transform.position, swim.currentDestination) <= 0.001f && Vector2.Distance(gravityItem.itemObject.localPosition, swim.currentDestinationZ) <= 0.001f)
                {
                    
                    timeToStayAtDestination = SetTimeToStayAtDestination();
                    
                    currentState = SwimmingState.isAtDestination;
                }


                break;


            


            case SwimmingState.isAtDestination:

                
                timeToStayAtDestination -= Time.deltaTime;
                if (timeToStayAtDestination <= 0)
                {
                    
                    swim.SetRandomDestination();
                    
                    currentState = SwimmingState.isSwimming;

                }
                


                break;


        }
    }



    float SetTimeToStayAtDestination()
    {
        return Random.Range(0.5f, 10.0f);
    }




    public void SetHome(Transform location)
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, 5);
        Collider2D nearest = null;
        float distance = 0;

        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].CompareTag("OpenFishSpot"))
            {
                float tempDistance = Vector3.Distance(transform.position, hit[i].transform.position);
                if (nearest == null || tempDistance < distance)
                {
                    nearest = hit[i];
                    distance = tempDistance;
                }
            }

        }
        if (nearest != null)
        {
            home = nearest.transform;

        }
        swim.centerOfActiveArea = home;

    }



}
