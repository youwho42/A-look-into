using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWalk : MonoBehaviour
{
    public float moveBaseSpeed;
    public float mainSpeed=2;
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

    public bool flipSpriteRenderer = true;
    public bool isWalking;
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



        t += Time.deltaTime;
        Vector2 m1 = Vector2.Lerp(mainPoints[0], mainPoints[1], t);
        Vector2 m2 = Vector2.Lerp(mainPoints[1], mainPoints[2], t);

        Vector2 newPosM = Vector2.Lerp(m1, m2, t);

        //Vector3 finalPosM = new Vector3(newPosM.x, newPosM.y, currentGridLocation.currentLevel);


        //thisTransform.position = new Vector3(newPosM.x, newPosM.y, currentGridLocation.currentLevel);
        transform.position = Vector2.MoveTowards(transform.position, newPosM, Time.deltaTime * mainSpeed);
        transform.position = new Vector3(transform.position.x, transform.position.y, currentGridLocation.currentLevel);
        currentGridLocation.UpdateLocation();
        
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
        Vector2 dir = GetDirection();
        // Set facing direction
        if (flipSpriteRenderer)
        {
            
            var direction = Mathf.Sign(dir.x);
            characterRenderer.flipX = direction > 0;
        }
        else
        {
            
            var direction = Mathf.Sign(dir.x);
            int d = direction < 0 ? 1 : -1;
            transform.localScale = new Vector3(d, 1, 1);
        }
        
    }

    public Vector2 GetDirection()
    {
        var dir = currentDestination - transform.position;
        return dir;
    }
}
