using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrasshopperAI : MonoBehaviour, IAnimal
{



    ItemGravity itemGravity;


    public CurrentState currentState;

    public enum CurrentState
    {
        isJumping,
        isAtDestination
    }

    void Start()
    {
        itemGravity = GetComponent<ItemGravity>();
    }
    void Update()
    {
        if (itemGravity.isGrounded)
        {
            Jump();
        }
    }

    void Jump()
    {
        itemGravity.positionZ = 0.5f;
        itemGravity.displacedPosition = new Vector3(0, itemGravity.spriteDisplacementY * itemGravity.positionZ, itemGravity.positionZ);
        itemGravity.itemObject.localPosition = itemGravity.displacedPosition;
    }


    public void SetHome(Transform transform)
    {
        
    }
}
