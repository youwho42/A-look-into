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
    public GameObject travellerPrefab;
    public GameObject appearFX;
    public int lastAccessoryIndex;

    public void SpawnMessenger(QI_ItemData message, BallPeopleMessageType messageType, UndertakingObject undertaking)
    {
        GameObject mess = null;
        SpawnBallPeople(messengerPrefab, out mess);
        
        var messItem = mess.GetComponent<InteractableBallPeopleMessenger>();
        messItem.messageItem = message;
        messItem.type = messageType;
        messItem.undertaking = undertaking;
    }

    public void SpawnTraveller(UndertakingObject undertaking, GameObject travellerDestination)
    {
        GameObject trav = null;
        SpawnBallPeople(travellerPrefab, out trav);

        var travItem = trav.GetComponent<InteractableBallPeopleTraveller>();
        travItem.undertaking.undertaking = undertaking;
        travItem.undertaking.task = undertaking.Tasks[0];
        var travelBall = trav.GetComponent<BallPeopleTravellerAI>();
        travelBall.travellerDestination = travellerDestination.transform.position;
    }


    void SpawnBallPeople(GameObject prefab, out GameObject ballPerson)
    {
        Vector2 offset = new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.0f));
        var pos = PlayerInformation.instance.player.position + (Vector3)offset;
        var mess = Instantiate(prefab, pos, Quaternion.identity);
        Instantiate(appearFX, pos, Quaternion.identity);
        mess.GetComponent<RandomColor>().SetRandomColor();
        mess.GetComponent<RandomAccessories>().PopulateList();
        mess.GetComponent<RandomAccessories>().ChooseAccessories();
        mess.GetComponent<SaveableItemEntity>().GenerateId();
        ballPerson = mess;
    }

}