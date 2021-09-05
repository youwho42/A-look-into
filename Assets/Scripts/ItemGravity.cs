using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGravity : MonoBehaviour
{


    public Transform itemObject;
    const float groundPlacement = 0.001f;
    const float gravity = -20f;

    float positionZ;
    const float spriteDisplacementY = 0.27808595f;
    Vector3 displacedPosition;
    bool isGrounded;
    private void Start()
    {
        displacedPosition = new Vector3(0, spriteDisplacementY * positionZ, positionZ);
        itemObject.localPosition = displacedPosition;
    }

    private void Update()
    {
        isGrounded = itemObject.localPosition.y <= groundPlacement;
        displacedPosition = Vector3.zero;
        if (!isGrounded)
        {
            positionZ += gravity * Time.deltaTime;
            displacedPosition = new Vector3(0, spriteDisplacementY * positionZ, positionZ);
            itemObject.transform.Translate(displacedPosition * Time.deltaTime);
        } 
        else
        {
            positionZ = 0;
            itemObject.localPosition = new Vector3(0, spriteDisplacementY * positionZ, positionZ + 0.01f);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            positionZ += 10f;
            displacedPosition = new Vector3(0, spriteDisplacementY * positionZ, positionZ);
            itemObject.transform.Translate(displacedPosition * Time.deltaTime);
        }
    }

    
}
