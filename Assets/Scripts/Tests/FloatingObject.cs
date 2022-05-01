using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FloatingObject : MonoBehaviour
{
    Vector3 currentDestination;
    float t;
    float timeToReachTarget;
    public float floatingSpeed;
    public float floatingDistanceMax;
    Vector2[] mainPoints = null;
    public float pathAngle;
    bool destinationIsValid;

    public Tilemap waterTiles;


    private void Start()
    {
        mainPoints = new Vector2[3];
        destinationIsValid = false;
        StartCoroutine(SetNewDestination());
    }
    private void Update()
    {
        if(destinationIsValid)
            Move();

        if (Vector2.Distance(transform.position, currentDestination) <= 0.001f)
        {
            Debug.Log("reached...");
            StopCoroutine(SetNewDestination());
            StartCoroutine(SetNewDestination());
        }
            
    }

    public void Move()
    {



        t += Time.deltaTime/* / timeToReachTarget*/;
        Vector2 m1 = Vector2.Lerp(mainPoints[0], mainPoints[1], t);
        Vector2 m2 = Vector2.Lerp(mainPoints[1], mainPoints[2], t);
        


        Vector2 newPosM = Vector2.Lerp(m1, m2, t);
       

        transform.position = Vector2.MoveTowards(transform.position, newPosM, Time.deltaTime * floatingSpeed);
        transform.position = new Vector3(transform.position.x, transform.position.y, 1);

    }
    

    
    IEnumerator SetNewDestination()
    {
        
        bool isValidDestination = false;
        while (!isValidDestination)
        {
            Vector2 rand = Random.insideUnitCircle * floatingDistanceMax;
            Vector3 possibleDestination = new Vector3(transform.position.x + rand.x, transform.position.y + rand.y, transform.position.z);
            
            Vector3Int tilePos = waterTiles.WorldToCell(possibleDestination - Vector3.forward);
            if(waterTiles.GetTile(tilePos) != null)
            {
                
                currentDestination = possibleDestination;
                float distanceA = Vector2.Distance(transform.position, currentDestination);
                SetAngledPath(currentDestination, distanceA * floatingSpeed);
                destinationIsValid = true;
                isValidDestination = true;
                yield return null;
            }

            yield return null;

        }
        yield return null;
        
    }

    void SetAngledPath(Vector2 mainDestination, float time)
    {
        
        t = 0;
        timeToReachTarget = time;

        mainPoints[0] = transform.position;
        mainPoints[2] = mainDestination;
        mainPoints[1] = mainPoints[0] + (mainPoints[2] - mainPoints[0]) / 2 + Vector2.up * pathAngle;
        
    }
}
