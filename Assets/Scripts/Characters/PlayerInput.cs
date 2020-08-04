using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [HideInInspector]
    public Vector2 movement;
    public bool usingEquippedItem;


    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement.y = Mathf.Clamp(movement.y, -0.5761719f, 0.5761719f);
        movement = movement.normalized;

        usingEquippedItem = Input.GetMouseButtonDown(0);
        
    }
}
