using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrasshopperAI : MonoBehaviour, IAnimal
{



    ItemGravity itemGravity;
    ItemAddThrust thrust;
    Vector2 destination;
    public CurrentState currentState;

    public enum CurrentState
    {
        isJumping,
        isAtDestination
    }

    void Start()
    {
        itemGravity = GetComponent<ItemGravity>();
        thrust = GetComponent<ItemAddThrust>();
    }
    /*void Update()
    {
        if (itemGravity.isGrounded)
        {
            thrust.AddThrust(0.5f);
            GetRandomDestination();
        }
        else
        {
            MoveMainItem();
        }
    }*/


    void MoveMainItem()
    {
        float step = 1 * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, destination, step); ;
    }

    void GetRandomDestination()
    {
        destination = transform.position + (Vector3)Random.insideUnitCircle * 0.3f;
        
    }

    public void SetHome(Transform transform)
    {
        
    }
}
