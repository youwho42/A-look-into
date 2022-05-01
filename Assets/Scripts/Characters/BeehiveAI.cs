using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeehiveAI : MonoBehaviour
{
    public SpriteRenderer mainSprite;
    public DrawZasYDisplacement displacementZ;
    public GameObject beeObject;
    public int maxBees;

    public void Start()
    {
        mainSprite.gameObject.transform.localPosition = displacementZ.displacedPosition;
        SpawnBee();
    }

    void SpawnBee()
    {
        for (int i = 0; i < maxBees; i++)
        {
            var b = Instantiate(beeObject, transform.position, Quaternion.identity);
            
            
        }
        
    }
}
