using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFlight : MonoBehaviour
{

    public float flyBaseSpeed;
    public float mainSpeed = 2;
    public Transform centerOfActiveArea;
    public string activeAreaBaseTagName;
    public Vector2 minMaxZRange;
    public Transform characterSprite;
    SpriteRenderer characterRenderer;
    const float groundPlacement = 0.001f;
    
    public float positionZ;
    public readonly float spriteDisplacementY = 0.27808595f;
    public Vector3 displacedPosition;

    public Vector3 currentDestination;
    public Vector3 destinationZ;

    Vector2[] mainPoints = null;
    Vector3[] subPoints = null;


    public float pathAngle;

    float t;
    float timeToReachTarget;
    Transform thisTransform;

    CurrentGridLocation currentGridLocation;

    float totalZ;
    Camera cam;
    public bool followingPlayer;

    private void Start()
    {
        cam = Camera.main;
        characterRenderer = characterSprite.GetComponent<SpriteRenderer>();
        currentGridLocation = GetComponent<CurrentGridLocation>();
        currentGridLocation.UpdateLocationAndPosition();
        thisTransform = transform;

        if (centerOfActiveArea == null)
            GetNearestBase();
        

        mainPoints = new Vector2[3];
        subPoints = new Vector3[3];
        
        SetRandomDestination();
        
    }

    public void Move()
    {

        

        t += Time.deltaTime/* / timeToReachTarget*/;
        Vector2 m1 = Vector2.Lerp(mainPoints[0], mainPoints[1], t);
        Vector2 m2 = Vector2.Lerp(mainPoints[1], mainPoints[2], t);
        Vector3 s1 = Vector3.Lerp(subPoints[0], subPoints[1], t);
        Vector3 s2 = Vector3.Lerp(subPoints[1], subPoints[2], t);


        Vector2 newPosM = Vector2.Lerp(m1, m2, t);
        Vector3 newPosS = Vector3.Lerp(s1, s2, t);

        thisTransform.position = Vector2.MoveTowards(thisTransform.position, newPosM, Time.deltaTime * mainSpeed);
        thisTransform.position = new Vector3(thisTransform.position.x, thisTransform.position.y, currentGridLocation.currentLevel);
        characterSprite.localPosition = Vector3.MoveTowards(characterSprite.localPosition, newPosS, Time.deltaTime * mainSpeed);
        /*thisTransform.position = new Vector3(newPosM.x, newPosM.y, currentGridLocation.currentLevel);*/
        //characterSprite.localPosition = newPosS;


        // setting size depending on z height of sprite and screen y position
        Vector3 screenPos = cam.WorldToScreenPoint(thisTransform.position).normalized;
        totalZ = (characterSprite.localPosition.z - screenPos.y) / 10;
        characterSprite.localScale = new Vector3(1 + totalZ, 1 + totalZ, 1);
        
        currentGridLocation.UpdateLocation();
        
    }

   

    void SetAngledPath(Vector2 mainDestination, Vector3 subDestination, float time)
    {
        SetFacingDirection();
        t = 0;
        timeToReachTarget = time;

        mainPoints[0] = thisTransform.position;
        mainPoints[2] = mainDestination;
        mainPoints[1] = mainPoints[0] + (mainPoints[2] - mainPoints[0]) / 2 + Vector2.up * pathAngle;
        subPoints[0] = characterSprite.localPosition;
        subPoints[2] = subDestination;
        subPoints[1] = subPoints[0] + (subPoints[2] - subPoints[0]) / 2 + Vector3.up * pathAngle/3;
    }

    public void SetRandomDestination(float dist = 1)
    {
        if (centerOfActiveArea == null)
            GetNearestBase();

        Vector2 rand = Random.insideUnitCircle * dist;
        currentDestination = new Vector3(centerOfActiveArea.position.x + rand.x, centerOfActiveArea.position.y + rand.y, thisTransform.position.z);
        float randZ = Random.Range(minMaxZRange.x, minMaxZRange.y);
        destinationZ = new Vector3(0, spriteDisplacementY * randZ, randZ);
        float distanceA = Vector2.Distance(thisTransform.position, currentDestination);
        float distanceB = Vector2.Distance(characterSprite.localPosition, destinationZ);
        float distance = distanceA > distanceB ? distanceA : distanceB;
        SetAngledPath(currentDestination, destinationZ, distance * flyBaseSpeed);
    }

    public void SetDestination(Vector3 mainDestination, Vector3 subDestination)
    {
        currentDestination = mainDestination;
        destinationZ = subDestination;
        float distance = Vector2.Distance(thisTransform.position, currentDestination);
        SetAngledPath(currentDestination, destinationZ, distance * flyBaseSpeed);
    }

    public void SetPosition(Vector3 location)
    {
        thisTransform.position = location;
        SetRandomDestination();
    }

    public void SetFacingDirection()
    {
        // Set facing direction
        Vector2 dir = currentDestination - thisTransform.position;
        var direction = Mathf.Sign(dir.x);
        characterRenderer.flipX = direction > 0;
    }

    public void GetNearestBase()
    {
        if (followingPlayer)
        {
            centerOfActiveArea = PlayerInformation.instance.player.transform;
            return;
        }
        

        Collider2D[] hit = Physics2D.OverlapCircleAll(thisTransform.position, 10);
        if (hit.Length > 0)
        {
            // Find nearest item.
            Collider2D nearest = null;
            float distance = 0;

            for (int i = 0; i < hit.Length; i++)
            {
                if (hit[i].CompareTag(activeAreaBaseTagName))
                {
                    float tempDistance = Vector3.Distance(thisTransform.position, hit[i].transform.position);
                    if (nearest == null || tempDistance < distance)
                    {
                        nearest = hit[i];
                        distance = tempDistance;
                    }
                }

            }
            if (nearest != null)
            {
                centerOfActiveArea = nearest.transform;
                
            }
        }
        centerOfActiveArea =  PlayerInformation.instance.player;
        

    }
}
