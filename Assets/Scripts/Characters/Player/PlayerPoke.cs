using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPoke : MonoBehaviour
{

    public LayerMask pokableLayer;
    public float detectionRadius;

    float playerZ;

    PlayerInformation player;
    bool pokingStickEquipped;
    public List<EquipmentData> pokingStickData = new List<EquipmentData>();
    public Transform pokeReticle;
    public bool canPoke;
    PokableItem currentPokable;
    public PokableItem CurrentPokable { get { return currentPokable; } }
    private void Start()
    {
        GameEventManager.onEquipmentUpdateEvent.AddListener(CheckEquipment);
        player = PlayerInformation.instance;
    }

    private void OnDisable()
    {
        GameEventManager.onEquipmentUpdateEvent.RemoveListener(CheckEquipment);
    }

    void CheckEquipment()
    {
        pokingStickEquipped = false;
        foreach (var item in pokingStickData)
        {
            if (player.equipmentManager.HasCurrentHandEquipment(item))
                pokingStickEquipped = true;
        }
        pokeReticle.gameObject.SetActive(false);

    }

    private void Update()
    {
        if (pokingStickEquipped)
        {
            playerZ = player.player.position.z;
            var pos = player.playerPokableSpot.position;
            Collider2D[] hit = Physics2D.OverlapCircleAll(pos, detectionRadius, pokableLayer);
            if (hit.Length > 0)
                GetNearestItem(hit, pos);
        }
        
    }

    public void GetNearestItem(Collider2D[] colliders, Vector3 position)
    {
        Collider2D nearest = null;
        float distance = 0;

        for (int i = 0; i < colliders.Length; i++)
        {



            if (!colliders[i].gameObject.TryGetComponent(out PokableItem pokable))
                continue;

            float tempDistance = 0;


            if (colliders[i].transform.position.z > (playerZ + 0.87f))
                continue;

            tempDistance = Vector2.Distance(position, colliders[i].transform.position);


            if (tempDistance > 0.225f)
                continue;

            if (nearest == null || tempDistance < distance)
            {
                currentPokable = pokable;
                nearest = colliders[i];
                distance = tempDistance;
            }
        }
        canPoke = false;
        pokeReticle.gameObject.SetActive(false);
        if (nearest != null)
        {
            pokeReticle.gameObject.SetActive(true);
            pokeReticle.position = nearest.transform.position;
            canPoke = true;
        }
        
        //Debug.Log(nearest.name);
        //MiniGameManager.instance.StartMiniGame(miniGameType, itemData, nearest.gameObject);
    }


}
