using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{


    public List<Boid> allBoids = new List<Boid>();

    public Transform mainDestination;

    public Transform[] secondaryDestinations;
    [HideInInspector]
    public Transform currentDestination;
    [HideInInspector]
    public int currentDestinationIndex = 0;
    private void Start()
    {
        
        if (secondaryDestinations.Length > 0)
        {
            currentDestination = secondaryDestinations[0];
            currentDestinationIndex++;
            InvokeRepeating("SetNewMainDestination", 15f, 25f);
            
        }
        else
            currentDestination = mainDestination;
    }

    public void SetNewMainDestination()
    {
        currentDestination = secondaryDestinations[currentDestinationIndex];
        if (currentDestinationIndex < secondaryDestinations.Length - 1)
            currentDestinationIndex++;
        else
            currentDestinationIndex = 0;
    }
}
