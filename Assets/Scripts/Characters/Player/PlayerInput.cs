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
    [HideInInspector]
    public bool isInUI;
    [HideInInspector]
    public bool isJumping;
    [HideInInspector]
    public bool canWalkOffCliff;





    void Update()
    {
        if(isPaused || isInUI)
        {
            movement = Vector2.zero;
        }
        else
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
            movement.y = Mathf.Clamp(movement.y, -0.578125f, 0.578125f);
            movement = movement.normalized;
            if (Input.GetKeyDown(KeyCode.LeftShift))
                isRunning = !isRunning;
            if (movement == Vector2.zero)
                isRunning = false;
            canWalkOffCliff = Input.GetKey(KeyCode.C);
            isJumping = Input.GetKeyDown(KeyCode.Space);
            
        }
        if (!isPaused || !isInUI)
        {
            usingEquippedItem = Input.GetMouseButtonDown(0);
        }
        
        if (Input.GetKeyDown(KeyCode.Escape) && UIScreenManager.instance.canChangeUI)
        {
            isPaused = !isPaused;
            LevelManager.instance.Pause(isPaused);
        }
            
    }

}
