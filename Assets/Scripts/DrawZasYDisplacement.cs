using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpotType
{
    None,
    Flower,
    Sparrow,
    Crow,
    Mouse,
    Fish,
    Squirrel,
    Chicken,
    Owl, 
    Bat

}

public class DrawZasYDisplacement : MonoBehaviour
{
    public SpotType spotType;

    public float positionZ;
    readonly float spriteDisplacementY = 0.2790625f;
    public Vector3 displacedPosition;

    public bool isInUse;

    private void Start()
    {
        displacedPosition = new Vector3(0, spriteDisplacementY * positionZ, positionZ);
    }

    private void OnDrawGizmosSelected()
    {
        var pos = new Vector3(0, spriteDisplacementY * positionZ, positionZ) + transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, 0.1f);
    }
}
