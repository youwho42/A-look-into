using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishAI : MonoBehaviour, IAnimal
{
    GravityItem gravityItem;
    public float roamingArea;
    float timeToStayAtDestination;
    CanReachTileSwim swim;

    

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
        swim.SetRandomDestination();
        swim.SetRandomDestinationZ();
    }

   

    private void Update()
    {
        

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




    



}
