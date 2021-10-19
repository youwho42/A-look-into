using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWalk : MonoBehaviour
{
    public float moveBaseSpeed;
    public Transform centerOfActiveArea;


    public Vector3 currentDestination;


    Vector2[] mainPoints = null;

    public float pathAngle;

    float t;
    float timeToReachTarget;
    Transform thisTransform;

    CurrentGridLocation currentGridLocation;
    public Transform characterSprite;
    SpriteRenderer characterRenderer;


    private void Start()
    {
        characterRenderer = characterSprite.GetComponent<SpriteRenderer>();
        currentGridLocation = GetComponent<CurrentGridLocation>();
        currentGridLocation.UpdateLocationAndPosition();
        thisTransform = transform;
        mainPoints = new Vector2[3];

        SetRandomDestination(); 
    }

    public void Move()
    {



        t += Time.deltaTime / timeToReachTarget;
        Vector2 m1 = Vector2.Lerp(mainPoints[0], mainPoints[1], t);
        Vector2 m2 = Vector2.Lerp(mainPoints[1], mainPoints[2], t);

        Vector2 newPosM = Vector2.Lerp(m1, m2, t);

        thisTransform.position = new Vector3(newPosM.x, newPosM.y, currentGridLocation.currentLevel);

        if (Time.frameCount % 10 == 0)
        {
            currentGridLocation.UpdateLocation();
        }
    }

    void SetAngledPath(Vector2 mainDestination, float time)
    {
        SetFacingDirection();
        t = 0;
        timeToReachTarget = time;

        mainPoints[0] = transform.position;
        mainPoints[2] = mainDestination;
        mainPoints[1] = mainPoints[0] + (mainPoints[2] - mainPoints[0]) / 2 + Vector2.up * pathAngle;

    }

    public void SetRandomDestination(float dist = 1)
    {
        Vector2 rand = Random.insideUnitCircle * dist;
        currentDestination = new Vector3(centerOfActiveArea.position.x + rand.x, centerOfActiveArea.position.y + rand.y, transform.position.z);

        float distance = Vector2.Distance(transform.position, currentDestination);


        SetAngledPath(currentDestination, distance * moveBaseSpeed);
    }

    public void SetDestination(Vector3 mainDestination, Vector3 subDestination)
    {
        currentDestination = mainDestination;

        float distance = Vector2.Distance(transform.position, currentDestination);
        SetAngledPath(currentDestination, distance * moveBaseSpeed);
    }

    public void SetFacingDirection()
    {
        // Set facing direction
        Vector2 dir = currentDestination - transform.position;
        var direction = Mathf.Sign(dir.x);
        characterRenderer.flipX = direction > 0;
    }
}
