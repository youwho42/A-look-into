using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAddThrust : MonoBehaviour
{
    public float thrustForce = 2f;
    public Vector3 thrustDirection;
    Vector3 destination;
    ItemGravity gravity;
    public bool isMoving;
    public bool bounced;

    public void Start()
    {
        gravity = GetComponent<ItemGravity>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && gravity.isGrounded)
        {
            AddThrust(thrustForce);
            destination = transform.position + thrustDirection;
        }
        /*if (!gravity.isGrounded)
        {
            MoveMainItem();
        }*/
        

    }
    
    /*void MoveMainItem()
    {
        float step = 1 * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, destination, step);
    }*/
    void AddThrust(float thrust)
    {
        gravity.positionZ += thrust;
        gravity.displacedPosition = new Vector3(0, gravity.spriteDisplacementY * gravity.positionZ, gravity.positionZ);
        gravity.itemObject.transform.Translate(gravity.displacedPosition * Time.deltaTime);
        
    }
 

   
}
