using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawZasYDisplacement : MonoBehaviour
{
    public float positionZ;
    readonly float spriteDisplacementY = 0.27808595f;
    public Vector3 displacedPosition;
    private void Start()
    {
        displacedPosition = new Vector3(0, spriteDisplacementY * positionZ, positionZ);
    }

    private void OnDrawGizmosSelected()
    {
        var pos = new Vector3(0, spriteDisplacementY * positionZ, positionZ);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, 0.1f);
    }
}
