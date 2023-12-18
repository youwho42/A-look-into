using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PendulumObject : MonoBehaviour
{
    public float r1;
    public float r2;
    public float m1;
    public float m2;
    public float a1;
    public float a2;
    public float a1_v;
    public float a2_v;
    public float a1_a;
    public float a2_a;
    public float g = 1;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector2(transform.position.x, transform.position.y - r1), 0.01f);
        Gizmos.DrawWireSphere(new Vector2(transform.position.x, transform.position.y - r1-r2), 0.01f);
    }
}
