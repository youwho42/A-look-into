using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesToPlayer : MonoBehaviour
{
    public GameObject particle;
    public float speed;

    
    public void SpawnParticles(int quantity, DrawZasYDisplacement displacement)
    {
        StartCoroutine(SpawnCo(quantity, displacement));
    }
    IEnumerator SpawnCo(int quantity, DrawZasYDisplacement displacement)
    {
        
        Transform player = PlayerInformation.instance.player;
        List<GameObject> particles = new List<GameObject>();
        List<Transform> particleItems = new List<Transform>();
        List<Vector2> directions = new List<Vector2>();
        int angleIncrement = 360 / quantity;
        for (int i = 0; i < quantity; i++)
        {
            var go = Instantiate(particle, transform.position, Quaternion.identity);
            var sr = go.GetComponentInChildren<SpriteRenderer>().transform;
            sr.gameObject.transform.localPosition = displacement.displacedPosition;
            particles.Add(go);
            particleItems.Add(sr);
            directions.Add(Quaternion.Euler(0f, 0f, i * angleIncrement) * Vector2.right);
        }
        
        float timer = 0;
        
        while (timer < .3f)
        {
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].transform.position += (Vector3)directions[i] * speed * Time.deltaTime;
                Vector3 interpolatedPosition = Vector3.Lerp(particleItems[i].transform.localPosition, new Vector3(0, GlobalSettings.SpriteDisplacementY, 1.0f), timer/.3f);
                particleItems[i].transform.localPosition = interpolatedPosition;
            }
            timer += Time.deltaTime;
            yield return null;
        }
        int counter = quantity;
        IDictionary<int, bool> doneList = new Dictionary<int, bool>();
        while (counter > 0)
        {

            for (int i = 0; i < particles.Count; i++)
            {
                if (!DoneAlready(doneList, i))
                {
                    var dir = player.position - particles[i].transform.position;
                    dir = dir.normalized;
                    particles[i].transform.position += dir * (speed * 2) * Time.deltaTime;
                    var dist = Vector2.Distance(player.position, particles[i].transform.position);
                    if (dist < .03f)
                    {
                        counter--;
                        doneList.Add(i, true);
                        Destroy(particles[i]);
                    }
                }
            }
            
            yield return null;
        }
    }
    bool DoneAlready(IDictionary<int, bool> theDictionary, int theItem)
    {
        bool tmpExists = false;
        theDictionary.TryGetValue(theItem, out tmpExists);
        return tmpExists;
    }
}
