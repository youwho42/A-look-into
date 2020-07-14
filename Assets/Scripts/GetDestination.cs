using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetDestination : MonoBehaviour
{

    
    public Transform chickenPen;
    public float chickenPenRadius;

    public Vector2 SetDestination()
    {
        Vector2 rand = Random.insideUnitCircle * chickenPenRadius;
        return rand + (Vector2)chickenPen.position;

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(chickenPen.position, chickenPenRadius);
    }
}
