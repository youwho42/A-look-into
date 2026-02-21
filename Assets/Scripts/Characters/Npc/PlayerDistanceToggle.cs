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



    Camera cam;
    float maxDistance = 6;
    public List<GameObject> animals;
    

    public List<GOAD_Scheduler_NPC> GOADAgents;
    public List<GOAD_Scheduler_CF> GOADCokernutFlump;

    float distance;
    PlayerInformation playerInformation;

    private void Start()
    {
        cam = Camera.main;
        PopulateLists();
        playerInformation = PlayerInformation.instance;
        //InvokeRepeating("CheckPlayerDistance", 0.0f, 0.5f);
        GameEventManager.onPlayerPositionUpdateEvent.AddListener(CheckPlayerDistance);
        CheckPlayerDistance(Vector3Int.zero);
    }

    private void OnDestroy()
    {
        GameEventManager.onPlayerPositionUpdateEvent.RemoveListener(CheckPlayerDistance);
    }



    public void PopulateLists()
    {
        animals.Clear();
        var a = FindObjectsByType<GOAD_Scheduler_Animal>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<IAnimal>().ToList();
        foreach (var animalComponent in a)
        {
            animals.Add((animalComponent as MonoBehaviour).gameObject);
        }
        
        GOADAgents.Clear();
        var c = FindObjectsByType<GOAD_Scheduler_NPC>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
        foreach (var agent in c)
        {
            GOADAgents.Add(agent);
        }

        GOADCokernutFlump.Clear();
        var d = FindObjectsByType<GOAD_Scheduler_CF>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
        foreach (var agent in d)
        {
            GOADCokernutFlump.Add(agent);
        }

    }

   
    public void AddAnimal(GameObject animal)
    {
        if (playerInformation == null)
            playerInformation = PlayerInformation.instance;
        //if (playerInformation.player == null)
        //    return;
        if (!animals.Contains(animal))
        {
            animals.Add(animal);  
            SetAnimalActiveBasedOnDistance(animal, playerInformation.player.position);
        }
            
    }
    public void RemoveAnimal(GameObject animal)
    {
        if (animals.Contains(animal))
            animals.Remove(animal);
    }

    private void CheckPlayerDistance(Vector3Int playerMapPosition)
    {
        float camOrtho = cam == null ? 1 : Mathf.Clamp(cam.orthographicSize, 1, 5);
        distance = maxDistance * camOrtho;
        distance = distance * distance;
        if (animals.Count > 0 || GOADAgents.Count > 0)
        {

            foreach (var animal in animals)
            {
                SetAnimalActiveBasedOnDistance(animal, playerInformation.player.position);
            }

            foreach (var agent in GOADAgents)
            {
                SetGOADAgentPropertiesBasedOnDistance(agent, playerInformation.player.position, maxDistance);
            }

            foreach (var agent in GOADCokernutFlump)
            {
                SetGOADAgentPropertiesBasedOnDistance(agent, playerInformation.player.position, maxDistance);
            }

        }
    }

    void SetAnimalActiveBasedOnDistance(GameObject obj, Vector3 playerPos)
    {
        if (obj == null)
            return;
        
        bool state = NumberFunctions.GetDistanceV3(obj.transform.position, playerPos) <= distance;
        if(obj.activeInHierarchy != state)
            obj.SetActive(state);
        Debug.Log(state);
    }

    void SetGOADAgentPropertiesBasedOnDistance(GOAD_Scheduler_NPC agent, Vector3 playerPos, float maxDist)
    {
        if (agent == null)
            return;
        
        float dist = NumberFunctions.GetDistanceV3(agent.transform.position, playerPos);
        bool state = dist <= distance || !agent.offScreen;
        bool nearPlayer = dist <= 1.5f;
        agent.animator.enabled = state;
        agent.offScreen = !state;
        agent.nearPlayer = nearPlayer;
    }

    void SetGOADAgentPropertiesBasedOnDistance(GOAD_Scheduler_CF agent, Vector3 playerPos, float maxDist)
    {
        if (agent == null)
            return;
       
        float dist = NumberFunctions.GetDistanceV3(agent.transform.position, playerPos);
        bool state = dist <= distance || !agent.offScreen;
        bool nearPlayer = dist <= 1.5f;
        agent.animator.enabled = state;
        agent.offScreen = !state;
        agent.nearPlayer = nearPlayer;
    }

    
}
