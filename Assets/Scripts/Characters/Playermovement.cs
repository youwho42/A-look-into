using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playermovement : MonoBehaviour
{

    public float moveSpeed;
    
    PlayerInput playerInput;
    CharacterMovement characterMovement; 

    // Start is called before the first frame update
    void Start()
    {
        
        playerInput = GetComponent<PlayerInput>();
        characterMovement = GetComponent<CharacterMovement>();
    }

    

    void FixedUpdate()
    {

        characterMovement.Move(playerInput.movement, moveSpeed);
        
    }

    

}
