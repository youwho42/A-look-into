using System.Collections;
using System.Collections.Generic;
using System.EnterpriseServices.Internal;
using System.Linq;
using System.Web.UI.WebControls;
using UnityEngine;

public class PlayerDistanceToggle : MonoBehaviour
{

    public static PlayerDistanceToggle instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }


    float maxDistance = 5;
    public List<GameObject> animals;

    private void Start()
    {
        PopulateAnimalList();
        InvokeRepeating("CheckPlayerDistance",0.0f, 0.5f);
    }

    public void PopulateAnimalList()
    {
        animals.Clear();
        var a = FindObjectsOfType<MonoBehaviour>().OfType<IAnimal>().ToList();
        foreach (var animalComponent in a)
        {
            animals.Add((animalComponent as MonoBehaviour).gameObject);
        }
    }

    private void CheckPlayerDistance()
    {
        if (animals.Count > 0)
        {
            var playerPosition = PlayerInformation.instance.player.position;
            foreach (var animal in animals)
            {
                animal.SetActive(GetPlayerDistance(animal, playerPosition) <= maxDistance);
            }
        }
    }

    float GetPlayerDistance(GameObject obj, Vector2 playerPos)
    {
        return Vector2.Distance(obj.transform.position, playerPos);
    }

}
