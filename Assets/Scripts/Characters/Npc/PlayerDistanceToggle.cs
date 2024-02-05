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

    


    float maxDistance = 6;
    public List<GameObject> animals;
    public List<SAP_Scheduler_NPC> agents;

    private void Start()
    {
        PopulateLists();
        InvokeRepeating("CheckPlayerDistance", 0.0f, 0.5f);
    }

    

    public void PopulateLists()
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

    private void CheckPlayerDistance(List<GameObject> objects)
    {
        
        
        foreach (var obj in objects)
        {
            obj.SetActive(true);
        }

            
        
    }

    private void CheckPlayerDistance()
    {
        if (animals.Count > 0 || agents.Count > 0)
        {
            var playerPosition = PlayerInformation.instance.player.position;

            foreach (var animal in animals)
            {
                SetAnimalActiveBasedOnDistance(animal, playerPosition, maxDistance);
            }

            foreach (var agent in agents)
            {
                SetAgentPropertiesBasedOnDistance(agent, playerPosition, maxDistance);
            }

            

        }
    }

    void SetAnimalActiveBasedOnDistance(GameObject obj, Vector3 playerPos, float maxDist)
    {
        if (obj == null)
            return;
        bool state = GetPlayerDistance(obj.transform, playerPos) <= maxDist;
        obj.SetActive(state);
    }

    void SetAgentPropertiesBasedOnDistance(SAP_Scheduler_NPC agent, Vector3 playerPos, float maxDist)
    {
        if (agent == null)
            return;
        bool state = GetPlayerDistance(agent.transform, playerPos) <= maxDist;
        agent.animator.enabled = state;
        agent.offScreen = !state;
    }

    

    public float GetPlayerDistance(Transform objTransform, Vector3 playerPos)
    {
        return Vector3.Distance(objTransform.position, playerPos);
    }



    
}
