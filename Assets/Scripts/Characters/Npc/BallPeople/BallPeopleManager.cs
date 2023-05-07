using Klaxon.UndertakingSystem;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using System.Messaging;
using UnityEngine;

public class BallPeopleManager : MonoBehaviour
{

    public static BallPeopleManager instance;
    

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    public GameObject messengerPrefab;
    public GameObject travelerPrefab;
    public GameObject seekerPrefab;
    public GameObject planterPrefab;
    public GameObject harvesterPrefab;
    public GameObject appearFX;
    public int lastAccessoryIndex;

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

    public void SpawnTraveller(CompleteTaskObject taskObject, GameObject travelerDestination, Vector3 position)
    {
        GameObject trav = null;
        SpawnBallPeople(travelerPrefab, out trav, position);

        var travItem = trav.GetComponent<InteractableBallPeopleTraveller>();
        travItem.undertaking.undertaking = taskObject.undertaking;
        travItem.undertaking.task = taskObject.task;
        var travelBall = trav.GetComponent<BallPeopleTravelerAI>();
        travelBall.travellerDestination = travelerDestination.transform.position;
    }

    public void SpawnSeeker(QI_ItemData item, int amount, CompleteTaskObject talkTask, CompleteTaskObject seekTask, Vector3 position)
    {
        GameObject seek = null;
        SpawnBallPeople(seekerPrefab, out seek, position);

        var seekerItem = seek.GetComponent<InteractableBallPeopleSeeker>();
        seekerItem.talkTask.undertaking = talkTask.undertaking;
        seekerItem.talkTask.task = talkTask.task;
        var seekerBall = seek.GetComponent<BallPeopleSeekerAI>();
        seekerBall.seekTask = seekTask;
        seekerBall.seekItem = item;
        seekerBall.seekAmount = amount;
    }

    public void SpawnFarmPlanter(PlantingArea plantingArea, Vector3 position)
    {
        GameObject planter = null;
        SpawnBallPeople(planterPrefab, out planter, position);

        
        var planterBall = planter.GetComponent<BallPeopleFarmPlanterAI>();
        planterBall.plantingArea = plantingArea;
        planterBall.seedBoxInventory = plantingArea.seedBox;
        
    }
    public void SpawnFarmHarvester(PlantingArea plantingArea, Vector3 position)
    {
        GameObject planter = null;
        SpawnBallPeople(harvesterPrefab, out planter, position);


        var harvesterBall = planter.GetComponent<BallPeopleFarmHarvesterAI>();
        harvesterBall.plantingArea = plantingArea;
        harvesterBall.seedBoxInventory = plantingArea.seedBox;

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

    
}
