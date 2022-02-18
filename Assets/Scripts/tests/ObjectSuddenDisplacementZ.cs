using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSuddenDisplacementZ : MonoBehaviour
{


    public Transform worldObject;

    CurrentGridLocation currentGridLocation;
    public Transform currentDestination;
    Vector3 currentPosition;

    public float speed;

    private void Start()
    {
        currentGridLocation = GetComponent<CurrentGridLocation>();
    }

    private void Update()
    {
        currentGridLocation.UpdateLocationAndPosition();
        Move();
    }
    private void Move()
    {
        currentPosition = transform.position;
        currentPosition = Vector2.MoveTowards(transform.position, new Vector3(currentDestination.position.x, currentDestination.position.y, currentGridLocation.currentLevel), Time.deltaTime * speed);
        currentPosition.z = currentGridLocation.currentLevel;
        transform.position = currentPosition;
    }

}
