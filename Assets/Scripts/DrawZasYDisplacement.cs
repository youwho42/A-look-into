using Klaxon.Interactable;
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
    Bat, 
    Dragonfly,
    Pigeon,
    Deer,
    Bear

}

public class DrawZasYDisplacement : MonoBehaviour
{
    public SpotType spotType;

    public float positionZ;
    float spriteDisplacementY = GlobalSettings.SpriteDisplacementY;
    public Vector3 displacedPosition;

    public bool isInUse;
    public bool isDecorationSurface;

    public bool showArea;
    [ConditionalHide("showArea", true)]
    public float size;

    private void Start()
    {
        displacedPosition = new Vector3(0, spriteDisplacementY * positionZ, positionZ);
    }

    private void OnDrawGizmosSelected()
    {
        var pos = new Vector3(0, spriteDisplacementY * positionZ, positionZ) + transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, 0.1f);
        Gizmos.color = Color.cyan;
        if (showArea)
            Gizmos.DrawWireSphere(transform.position, size);
    }

    
}
