using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class NpcMovement : MonoBehaviour
{
    [HideInInspector]
    public Vector3 direction;
    public float moveSpeed = 30.0f;
    CharacterMovement characterMovement;
    
    

    private void Start()
    {
        
    
        characterMovement = GetComponent<CharacterMovement>();
        
    }

    private void FixedUpdate()
    {
        characterMovement.Move(direction, moveSpeed);
    }

    
}
