using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{

    Rigidbody2D body;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }
    public void Move(Vector3 movement, float moveSpeed)
    {
        Vector2 move = movement * moveSpeed * Time.fixedDeltaTime;
        body.velocity = move;
        if (movement == Vector3.zero)
        {
            body.velocity = Vector2.zero;
        }
    }
}
