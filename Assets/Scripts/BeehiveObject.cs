using UnityEngine;
using System.Collections.Generic;
using Klaxon.GravitySystem;
using System.Collections;


public class BeehiveObject : MonoBehaviour
{
    DrawZasYDisplacement displacement;

    public Transform beehiveTransform;
    public Transform beehiveSprite;
    public List<GravityItemFly> bees = new List<GravityItemFly>();

    public void SetBeehive(DrawZasYDisplacement ZasYdisplacement)
    {
        displacement = ZasYdisplacement;
        beehiveTransform.position = displacement.gameObject.transform.position;
        beehiveSprite.localPosition = displacement.displacedPosition;
        var distanceToggle = PlayerDistanceToggle.instance;
        foreach (var bee in bees)
        {
            bee.gameObject.transform.position = displacement.transform.position;
            distanceToggle.AddAnimal(bee.gameObject);
        }
    }

    public void DestroyBeehive()
    {
        var distanceToggle = PlayerDistanceToggle.instance;
        foreach (var bee in bees)
        {
            distanceToggle.AddAnimal(bee.gameObject);
        }


        Destroy(gameObject);
        
    }
    public void FadeAll()
    {
        StartCoroutine("FadeAllCo");
    }
    IEnumerator FadeAllCo()
    {
        float maxTime = 3.5f;
        float timer = 0.0f;
        beehiveSprite.GetComponent<SpriteRenderer>().enabled = false;

        while (timer < maxTime)
        {
            timer += Time.deltaTime;
            float a = timer / maxTime;
            Color c = Color.white;
            c.a = Mathf.Abs(a - 1);
            foreach (var bee in bees)
            {
                bee.characterRenderer.color = c;
            }

            yield return null;
        }

        yield return null;
        Destroy(gameObject);
    }
}
