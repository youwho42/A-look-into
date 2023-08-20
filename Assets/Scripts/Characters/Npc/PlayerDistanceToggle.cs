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
    public List<SAP_Scheduler_NPC> agents;

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
        var b = FindObjectsOfType<SAP_Scheduler_NPC>().ToList();
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
                bool state = GetPlayerDistance(agent.gameObject, playerPosition) <= maxDistance;
                agent.animator.enabled = state;
                agent.offScreen = !state;
            }
        }
    }

    float GetPlayerDistance(GameObject obj, Vector3 playerPos)
    {
        
        
        return Vector3.Distance(obj.transform.position, playerPos);
    }

}
