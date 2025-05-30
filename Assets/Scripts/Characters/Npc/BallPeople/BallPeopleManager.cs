using Klaxon.GOAD;
using Klaxon.Interactable;
using Klaxon.SaveSystem;
using Klaxon.UndertakingSystem;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

public class BallPeopleManager : MonoBehaviour
{

    public static BallPeopleManager instance;
    

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    

    public GameObject messengerPrefab;
    public GameObject travellerPrefab;
    public GameObject travellerHomePrefab;
    public GameObject seekerPrefab;
    public GameObject planterPrefab;
    public GameObject harvesterPrefab;
    public GameObject indicatorPrefab;
    public GameObject locationerPrefab;
    public GameObject homeBoundPrefab;
    public GameObject appearFX;
    public float lastColorA;
    public System.Random random = new System.Random();
    
    public Queue<int> accessoryIndexQueue = new Queue<int>();

    public void SpawnMessenger(QI_ItemData message, BallPeopleMessageType messageType, UndertakingObject undertaking, QI_CraftingRecipe craftingRecipe, Vector3 position)
    {
        GameObject mess = null;
        SpawnBallPeople(messengerPrefab, out mess, position);
        
        var messItem = mess.GetComponent<InteractableBallPeopleMessenger>();
        messItem.messageItem = message;
        messItem.type = messageType;
        messItem.undertaking = undertaking;
        messItem.craftingRecipe = craftingRecipe;
    }

    public void SpawnVillager(CompleteTaskObject taskObject, Vector3 position)
    {
        Vector3 offset = new Vector2(Random.Range(-0.2f, 0.2f), -0.2f);
        GameObject villager = null;

        SpawnBallPeople(homeBoundPrefab, out villager, position + offset);

        if (taskObject.undertaking != null)
        {
            var v = villager.GetComponent<InteractableBallPeopleVillager>();
            v.undertaking.undertaking = taskObject.undertaking;
            v.undertaking.task = taskObject.task; 
        }
        var villagerBall = villager.GetComponent<GOAD_Scheduler_BP>();
        villagerBall.BPHomeDestination = position;

    }


    public void SpawnTraveller(CompleteTaskObject taskObject, GameObject travelerDestination, Vector3 position)
    {
        GameObject trav = null;
        SpawnBallPeople(travellerPrefab, out trav, position);

        var travItem = trav.GetComponent<InteractableBallPeopleTraveller>();
        travItem.undertaking.undertaking = taskObject.undertaking;
        travItem.undertaking.task = taskObject.task;
        var travelBall = trav.GetComponent<GOAD_Scheduler_BP>();
        travelBall.BPHomeDestination = travelerDestination.transform.position;
    }

    public void SpawnTravellerHome(Vector3 travelerDestination, Vector3 position, int accessoryIndex, Color color)
    {
        GameObject trav = null;
        SpawnBallPeople(travellerHomePrefab, out trav, position);

        var travelBall = trav.GetComponent<GOAD_Scheduler_BP>();
        travelBall.BPHomeDestination = travelerDestination;

        var travelAccessory = trav.GetComponent<RandomAccessories>();
        travelAccessory.SetAccessories(accessoryIndex);

        var travelColor = trav.GetComponent<RandomColor>();
        travelColor.SetColor(color.r, color.g, color.b);
    }

    public void SpawnSeeker(QI_ItemData item, int amount, CompleteTaskObject talkTask, CompleteTaskObject seekTask, Vector3 position)
    {
        GameObject seek = null;
        SpawnBallPeople(seekerPrefab, out seek, position);

        var seekerItem = seek.GetComponent<InteractableBallPeopleSeeker>();
        seekerItem.talkTask.undertaking = talkTask.undertaking;
        seekerItem.talkTask.task = talkTask.task;
        var seekerBall = seek.GetComponent<GOAD_Scheduler_BP>();
        seekerBall.task = seekTask;
        seekerBall.seekItem = item;
        seekerBall.seekAmount = amount;
    }

    public void SpawnFarmPlanter(PlantingArea plantingArea, Vector3 position)
    {
        
        Vector3 offset = new Vector2(0.0f, -0.13f);
        
        GameObject planter = null;
        SpawnBallPeople(planterPrefab, out planter, position + offset);

        
        var planterBall = planter.GetComponent<GOAD_Scheduler_BP>();
        planterBall.plantingArea = plantingArea;
        planterBall.seedBoxInventory = plantingArea.seedBox;
        
    }
    public void SpawnFarmHarvester(PlantingArea plantingArea, Vector3 position)
    {
        Vector3 offset = new Vector2(0.0f, -0.13f);

        GameObject planter = null;
        SpawnBallPeople(harvesterPrefab, out planter, position + offset);


        var harvesterBall = planter.GetComponent<GOAD_Scheduler_BP>();
        harvesterBall.plantingArea = plantingArea;
        harvesterBall.seedBoxInventory = plantingArea.seedBox;

    }

    public void SpawnIndicator(int indicatorIndex, Color color, Vector3 position)
    {
        GameObject ind = null;
        SpawnBallPeople(indicatorPrefab, out ind, position);

        ind.GetComponent<GOAD_Scheduler_BP>().indicatorIndex = indicatorIndex;
        ind.GetComponent<GOAD_Scheduler_BP>().hasInteracted = true;
        ind.GetComponent<RandomColor>().SetColor(color.r, color.g, color.b);
        
    }

    public void SpawnLocationer(UndertakingObject undertaking, Transform location, Vector3 position, LocalizedString title, LocalizedString description)
    {
        GameObject loc = null;
        SpawnBallPeople(locationerPrefab, out loc, position);
        var interactiball = loc.GetComponent<InteractableBallPeopleLocationer>();
        interactiball.talkTask.undertaking = undertaking;
        interactiball.messageTitle = title;
        interactiball.messageDescription = description;
        loc.GetComponent<GOAD_Scheduler_BP>().locationerLocation = location;

    }



    void SpawnBallPeople(GameObject prefab, out GameObject ballPerson, Vector3 position)
    {
        //Vector2 offset = new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.0f));
        //var pos = position + (Vector3)offset;
        var mess = Instantiate(prefab, position, Quaternion.identity);
        Instantiate(appearFX, position, Quaternion.identity);
        
        mess.GetComponent<RandomColor>().SetRandomColor();
        mess.GetComponent<RandomAccessories>().PopulateList();
        mess.GetComponent<RandomAccessories>().ChooseAccessories();
        if(mess.TryGetComponent(out SaveableItemEntity saveItem))
            saveItem.GenerateId();
        ballPerson = mess;
    }

    public void GenerateRandomList(int amount)
    {
        List<int> temp = new List<int>();
        for (int i = 0; i < amount; i++)
        {
            temp.Add(i);
        }
        for (int i = 0; i < amount; i++)
        {
            int index = Random.Range(0, temp.Count);
            accessoryIndexQueue.Enqueue(temp[index]);
            temp.RemoveAt(index);
        }
    }

    

}
