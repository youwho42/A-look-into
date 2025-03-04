using Klaxon.GOAD;
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
    //public List<SAP_Scheduler_NPC> agents;

    public List<GOAD_Scheduler_NPC> GOADAgents;
    public List<GOAD_Scheduler_CF> GOADCokernutFlump;

    private void Start()
    {
        PopulateLists();
        
        InvokeRepeating("CheckPlayerDistance", 0.0f, 0.5f);
    }
    


    public void PopulateLists()
    {
        animals.Clear();
        var a = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IAnimal>().ToList();
        foreach (var animalComponent in a)
        {
            animals.Add((animalComponent as MonoBehaviour).gameObject);
        }
        
        GOADAgents.Clear();
        var c = FindObjectsByType<GOAD_Scheduler_NPC>(FindObjectsSortMode.None).ToList();
        foreach (var agent in c)
        {
            GOADAgents.Add(agent);
        }

        GOADCokernutFlump.Clear();
        var d = FindObjectsByType<GOAD_Scheduler_CF>(FindObjectsSortMode.None).ToList();
        foreach (var agent in d)
        {
            GOADCokernutFlump.Add(agent);
        }

    }

    //private void CheckPlayerDistance(List<GameObject> objects)
    //{
        
    //    foreach (var obj in objects)
    //    {
    //        obj.SetActive(true);
    //    }

    //}

    private void CheckPlayerDistance()
    {
        if (animals.Count > 0 || GOADAgents.Count > 0)
        {
            var playerPosition = PlayerInformation.instance.player.position;

            foreach (var animal in animals)
            {
                SetAnimalActiveBasedOnDistance(animal, playerPosition, maxDistance);
            }

            foreach (var agent in GOADAgents)
            {
                SetGOADAgentPropertiesBasedOnDistance(agent, playerPosition, maxDistance);
            }

            foreach (var agent in GOADCokernutFlump)
            {
                SetGOADAgentPropertiesBasedOnDistance(agent, playerPosition, maxDistance);
            }

        }
    }

    void SetAnimalActiveBasedOnDistance(GameObject obj, Vector3 playerPos, float maxDist)
    {
        if (obj == null)
            return;
        bool state = GetPlayerDistance(obj.transform, playerPos) <= maxDist * maxDist;
        obj.SetActive(state);
    }

    void SetGOADAgentPropertiesBasedOnDistance(GOAD_Scheduler_NPC agent, Vector3 playerPos, float maxDist)
    {
        if (agent == null)
            return;
        float dist = GetPlayerDistance(agent.transform, playerPos);
        bool state = dist <= maxDist * maxDist || !agent.offScreen;
        bool nearPlayer = dist <= 1.5f;
        agent.animator.enabled = state;
        agent.offScreen = !state;
        agent.nearPlayer = nearPlayer;
    }

    void SetGOADAgentPropertiesBasedOnDistance(GOAD_Scheduler_CF agent, Vector3 playerPos, float maxDist)
    {
        if (agent == null)
            return;
        float dist = GetPlayerDistance(agent.transform, playerPos);
        bool state = dist <= maxDist * maxDist || !agent.offScreen;
        bool nearPlayer = dist <= 1.5f;
        agent.animator.enabled = state;
        agent.offScreen = !state;
        agent.nearPlayer = nearPlayer;
    }

    public float GetPlayerDistance(Transform objTransform, Vector3 playerPos)
    {
        return (objTransform.position - playerPos).sqrMagnitude;
    }



    
}
