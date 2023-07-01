using Klaxon.GOAP;
using Klaxon.SAP;
using System.Collections.Generic;
using System.Linq;
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
    public List<SAP_Scheduler> agents;

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
        agents.Clear();
        var b = FindObjectsOfType<SAP_Scheduler>().ToList();
        foreach (var agent in b)
        {
            agents.Add(agent);
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
        if (agents.Count > 0)
        {
            var playerPosition = PlayerInformation.instance.player.position;
            foreach (var agent in agents)
            {
                bool state = GetPlayerDistance(agent.gameObject, playerPosition) <= 3.5f;
                agent.animator.enabled = state;
                agent.offScreen = !state;
            }
        }
    }

    float GetPlayerDistance(GameObject obj, Vector2 playerPos)
    {
        return Vector2.Distance(obj.transform.position, playerPos);
    }

}
