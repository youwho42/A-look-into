using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    


    [HideInInspector]
    public Vector2 movement;
    [HideInInspector]
    public bool usingEquippedItem;
    [HideInInspector]
    public bool isRunning;
    [HideInInspector]
    public bool isPaused;





    void Update()
    {
        if (!isPaused)
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
            movement.y = Mathf.Clamp(movement.y, -0.5761719f, 0.5761719f);
            movement = movement.normalized;
            if (Input.GetKeyDown(KeyCode.LeftShift))
                isRunning = !isRunning;
            if (movement == Vector2.zero)
                isRunning = false;
            usingEquippedItem = Input.GetMouseButtonDown(0); 
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            LevelManager.instance.Pause(isPaused);
        }
            
    }

}
