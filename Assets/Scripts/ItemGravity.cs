using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGravity : MonoBehaviour
{

    public Transform itemObject;
    const float groundPlacement = 0.001f;
    const float gravity = -20f;

    public float positionZ;
    const float spriteDisplacementY = 0.27808595f;
    public Vector3 displacedPosition;
    public bool isGrounded;
    private void Start()
    {
        displacedPosition = new Vector3(0, spriteDisplacementY * positionZ, positionZ);
        itemObject.localPosition = displacedPosition;
    }

    private void Update()
    {
        if(itemObject.localPosition.z <= groundPlacement && !isGrounded)
        {
            isGrounded = true;
            positionZ = groundPlacement;
            displacedPosition = new Vector3(0, spriteDisplacementY * positionZ, positionZ);
            itemObject.localPosition = displacedPosition;
        }
        else
        {
            isGrounded = false;
            positionZ += gravity * Time.deltaTime;
            displacedPosition = new Vector3(0, spriteDisplacementY * positionZ, positionZ);
            
            itemObject.localPosition = Vector3.MoveTowards(itemObject.localPosition, displacedPosition, .5f*Time.deltaTime);
        }
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            positionZ += 3;
            displacedPosition = new Vector3(0, spriteDisplacementY * positionZ, positionZ);
            itemObject.localPosition = Vector3.MoveTowards(itemObject.localPosition, displacedPosition, 5*Time.deltaTime);
        }
    }

    void SetItemYZPosition()
    {

    }
}
