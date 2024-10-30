using Klaxon.GravitySystem;
using Klaxon.Interactable;
using Klaxon.SaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.Interactable
{
    public class InteractableDynamiballSpawner : Interactable
    {


        public GravityItemMovementFree itemToSpawn;
        
        public float spawnHeight = 1;

        GravityItemMovementFree spawnedItem;
        public override void Start()
        {
            base.Start();
            StartCoroutine(SpawnGravityItemCo());
        }

        public override void Interact(GameObject interactor)
        {
            base.Interact(interactor);
            StartCoroutine(SpawnGravityItemCo());
        }

        
        IEnumerator SpawnGravityItemCo()
        {
            yield return new WaitForSeconds(0.7f);

            if(spawnedItem == null)
                spawnedItem = Instantiate(itemToSpawn) as GravityItemMovementFree;
                
            spawnedItem.currentDirection = Vector2.zero;
            spawnedItem.transform.position = transform.position;
            spawnedItem.itemObject.localPosition = new Vector3(0, GlobalSettings.SpriteDisplacementY * spawnHeight, spawnHeight);
            spawnedItem.currentTilePosition.position = spawnedItem.currentTilePosition.GetCurrentTilePosition(spawnedItem.transform.position);
            spawnedItem.currentLevel = spawnedItem.currentTilePosition.position.z;
            spawnedItem.bounceFactor = 1;
            yield return null;
        }

    }

}