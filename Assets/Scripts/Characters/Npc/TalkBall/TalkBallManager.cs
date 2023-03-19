using Klaxon.UndertakingSystem;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using System.Messaging;
using UnityEngine;

public class TalkBallManager : MonoBehaviour
{

    public static TalkBallManager instance;
    

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    public GameObject messengerPrefab;
    public GameObject appearFX;

    public void SpawnMessenger(QI_ItemData message, TalkBallMessageType messageType, UndertakingObject undertaking)
    {

        Vector2 offset = new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.0f));
        var pos = PlayerInformation.instance.player.position + (Vector3)offset;
        var mess = Instantiate(messengerPrefab, pos, Quaternion.identity);
        Instantiate(appearFX, pos, Quaternion.identity);
        mess.GetComponent<RandomColor>().SetRandomColor();
        mess.GetComponent<RandomAccessories>().PopulateList();
        mess.GetComponent<RandomAccessories>().ChooseAccessories();
        mess.GetComponent<SaveableItemEntity>().GenerateId();
        var messItem = mess.GetComponent<InteractableMessenger>();
        messItem.messageItem = message;
        messItem.type = messageType;
        messItem.undertaking = undertaking;
    }


}
