using BezierSolution;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralLegAnimation : MonoBehaviour
{
    [Serializable]
    public class Leg
    {
        public Transform legBase;
        public Transform legIK;
        public Transform legIdleTarget;
        public bool isEnqueued;
        public bool isMoving;
        
    }
    

    Queue<IEnumerator> legsToMove = new Queue<IEnumerator>(); //queue of legs that will need to move
    public List<Leg> legs = new List<Leg>(); //the legs that will actually move
    public Dictionary<Leg, Vector3> legPositions = new Dictionary<Leg, Vector3>();
    public BezierSpline footPath;
    Coroutine legPosCo;
    bool isRunning;
    public float stepDistance;
    CharacterWalk walk;
    private void Start()
    {
        walk = GetComponent<CharacterWalk>();
        foreach (var leg in legs)
        {
            leg.legIK.position = leg.legIdleTarget.position;
            legPositions.Add(leg, leg.legIK.position);
            
        }
    }

    private void LateUpdate()
    {
        CheckLegPositions();
    }

    

    void CheckLegPositions()
    {
        bool canMove = true;
        foreach (var leg in legs)
        {
            if (leg.isMoving)
                canMove = false;
        }
        if (legsToMove.Count > 0 && canMove)
        {
            StartCoroutine(legsToMove.Dequeue());

        }
        if (walk.isWalking)
        {
            foreach (var leg in legs)
            {
                if (legPositions.TryGetValue(leg, out Vector3 pos))
                    leg.legIK.position = pos;

                if (!leg.isEnqueued)
                {
                    var footZeroPosition = (Vector2)leg.legBase.position + Vector2.down * .1f;
                    var newPosition = footZeroPosition + walk.GetDirection().normalized * stepDistance / 2;
                    float dist = Vector2.Distance(leg.legIK.position, footZeroPosition);
                    if (dist > stepDistance)
                    {
                        
                        leg.isEnqueued = true;
                        legsToMove.Enqueue(SetLegPosition(leg, newPosition));
                    }

                }
            }
        }
        else
        {
            foreach (var leg in legs)
            {
                float dist = Vector2.Distance(leg.legIK.position, leg.legIdleTarget.position);
                if (dist>=0.05f)
                {
                    if  (!leg.isEnqueued)
                    {
                        
                        leg.isEnqueued = true;
                        legsToMove.Enqueue(SetLegPosition(leg, (Vector2)leg.legIdleTarget.position));
                    }
                }
            }
        }


    }
  
    Vector2[] SetAngledPath(Leg leg, Vector2 mainDestination)
    {

        Vector2[] mainPoints = new Vector2[3];
        mainPoints[0] = leg.legIK.position;
        mainPoints[2] = mainDestination;
        mainPoints[1] = mainPoints[0] + (mainPoints[2] - mainPoints[0]) / 2 + Vector2.up * 0.1f;
        return mainPoints;
    }

    IEnumerator SetLegPosition(Leg leg, Vector3 position)
    {
        Vector2[] mainPoints = SetAngledPath(leg, position);
        leg.isMoving = true;
        
        //Vector3 startPosition = leg.legIK.position;
        float t = 0;
        float stepDuration = .4f;
        while (t < stepDuration)
        {

            t += Time.deltaTime / stepDuration;
            Vector2 m1 = Vector2.Lerp(mainPoints[0], mainPoints[1], t);
            Vector2 m2 = Vector2.Lerp(mainPoints[1], mainPoints[2], t);
            
            Vector2 newPosM = Vector2.Lerp(m1, m2, t);

            leg.legIK.position = newPosM;
            

            yield return null;
        }
        legPositions[leg] = position;
        leg.isMoving = false;
        leg.isEnqueued = false;
        
    }

    
}
