using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGravity : MonoBehaviour
{

    public Transform itemObject;
    const float groundPlacement = 0.001f;
    const float gravity = 20f;

    public float positionZ;
    public readonly float spriteDisplacementY = 0.27808595f;
    public Vector3 displacedPosition;
    public bool isGrounded;
   

    private void Start()
    {
        ResetStartingPosition(10);
    }

    private void Update()
    {
        isGrounded = itemObject.localPosition.y <= groundPlacement;
        
        if (!isGrounded)
        {
            positionZ -= gravity * Time.deltaTime;
            displacedPosition = new Vector3(0, spriteDisplacementY * positionZ, positionZ);
            itemObject.localPosition = displacedPosition;
            //itemObject.Translate(displacedPosition * Time.deltaTime);
        }
        
     


    }

    public void ResetStartingPosition(float posZ)
    {
        positionZ = posZ;
        displacedPosition = new Vector3(0, spriteDisplacementY * positionZ, positionZ);
        itemObject.localPosition = displacedPosition;
    }
    

}
