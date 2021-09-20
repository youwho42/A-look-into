using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGridLocation : MonoBehaviour
{

    CurrentGridLocation currentGridLocation;
    
    private void Start()
    {
        currentGridLocation = GetComponent<CurrentGridLocation>();
        currentGridLocation.UpdateLocationAndPosition();

    }

    private void Update()
    {

        if (Time.frameCount % 31 == 0)
        {
        
            currentGridLocation.UpdateLocation();

        
        }
        

    }

}
