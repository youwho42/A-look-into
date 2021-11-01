using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeehiveAI : MonoBehaviour, IAnimal
{
    public SpriteRenderer mainSprite;
    public DrawZasYDisplacement displacementZ;
    public GameObject beeObject;
    public int maxBees;

    public void SetHome(Transform location)
    {
        if(location.TryGetComponent(out PlantLifeCycle plant))
        {
            transform.parent = plant.homePoint.transform;
            transform.localPosition = Vector3.zero;
            displacementZ.displacedPosition = plant.homePoint.GetComponent<DrawZasYDisplacement>().displacedPosition;
            mainSprite.gameObject.transform.localPosition = displacementZ.displacedPosition;
            plant.homeOccupiedBy = "Beehive";
        }
        SpawnBee();
    }

    void SpawnBee()
    {
        for (int i = 0; i < maxBees; i++)
        {
            var b = Instantiate(beeObject, transform);
            b.transform.localPosition = mainSprite.gameObject.transform.localPosition;
            b.GetComponent<BeeAI>().SetHome(transform);
        }
        
    }
}
