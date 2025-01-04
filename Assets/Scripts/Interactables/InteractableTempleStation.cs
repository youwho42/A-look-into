using Klaxon.GOAD;
using Klaxon.UndertakingSystem;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Klaxon.Interactable
{
    public class InteractableTempleStation : Interactable
    {
        public GameObject purpleRain;
        public ParticleSystem particleEffect;
        public List<GameObject> firesToLight = new List<GameObject>();
        [HideInInspector]
        public bool isActivated;
        public Tilemap fissureMap;

        public List<Vector3Int> fissurePositions = new List<Vector3Int>();

        CompleteTaskOnInteraction taskOnInteraction;

        public GOAD_ScriptableCondition completedCondition;

        public List<QI_CraftingIngredient> requiredItems = new List<QI_CraftingIngredient>();
        public List<GameObject> gameObjectsToEnable = new List<GameObject>();

        public override void Start()
        {
            base.Start();
            taskOnInteraction = GetComponent<CompleteTaskOnInteraction>();
        }

        public override void Interact(GameObject interactor)
        {
            base.Interact(interactor);
            if(DemoManager.instance.IsDemoVersion() && instruction != null)
            {
                HasReadHowTo();
                return;
            }
            if (!isActivated)
            {
                if (InteractCostReward() && CheckForIngredients())
                {
                    foreach (var item in requiredItems)
                    {
                        PlayerInformation.instance.playerInventory.RemoveItem(item.Item, item.Amount);
                    }
                    StartCoroutine(LightFiresCo(true));
                }
                    
                
            }

        }

        IEnumerator LightFiresCo(bool lit)
        {
            canInteract = false;
            isActivated = lit;

            //particle effect
            particleEffect.Play();

            yield return new WaitForSeconds(1f);

            // Set drops to hide at top
            var allDrops = purpleRain.GetComponentsInChildren<HideDrop>();
            foreach (var drop in allDrops)
            {
                drop.StartFade();
            }

            yield return new WaitForSeconds(5f);

            // purple rain stops
            Destroy(purpleRain);

            // close fissure
            for (int i = 0; i < fissurePositions.Count; i++)
            {
                fissureMap.SetTile(fissurePositions[i], null);
                PathRequestManager.instance.pathfinding.isometricGrid.nodeLookup[fissurePositions[i]].walkable = true;
            }

            // light flames
            foreach (var fire in firesToLight)
            {
                fire.SetActive(lit);
            }
            SetGameObjectsToEnable(true);
            // Complete task/undertaking if there is one
            
            if (taskOnInteraction != null)
                taskOnInteraction.CompleteTask();
            // and set world condition
            if (completedCondition != null)
                GOAD_WorldBeliefStates.instance.SetWorldState(completedCondition.Condition, completedCondition.State);
            
        }

        public void SetTempleFireAndRainStates(bool lit)
        {
            isActivated = lit;
            canInteract = !lit;
            foreach (var fire in firesToLight)
            {
                fire.SetActive(lit);
            }
            if (lit)
            {
                for (int i = 0; i < fissurePositions.Count; i++)
                {
                    fissureMap.SetTile(fissurePositions[i], null);
                    PathRequestManager.instance.pathfinding.isometricGrid.nodeLookup[fissurePositions[i]].walkable = true;
                }
                Destroy(purpleRain);
            }
            SetGameObjectsToEnable(lit);
        }

        void SetGameObjectsToEnable(bool state)
        {
            foreach (var item in gameObjectsToEnable)
            {
                item.SetActive(state);
            }
        }
        bool InteractCostReward()
        {
            float agency = playerInformation.statHandler.GetStatMaxModifiedValue("Agency");
            if (agency >= agencyCost)
                return true;

            Notifications.instance.SetNewNotification($"{agencyCost} <sprite name=\"Agency\">", null, 0, NotificationsType.Warning);

            //NotificationManager.instance.SetNewNotification($"{agencyCost} Agency needed", NotificationManager.NotificationType.Warning);
            return false;
        }

        public bool CheckForIngredients()
        {
            bool hasAll = true;
            foreach (var ingredient in requiredItems)
            {

                int t = PlayerInformation.instance.GetTotalInventoryQuantity(ingredient.Item);
                if (t < ingredient.Amount)
                {
                    Notifications.instance.SetNewNotification($"{ingredient.Amount - t} {ingredient.Item.localizedName.GetLocalizedString()}", null, 0, NotificationsType.Warning);
                    hasAll = false;
                }

            }
            return hasAll;
        }

        private void OnDrawGizmosSelected()
        {
            if (fissurePositions.Count < 0)
                return;

            for (int i = 0; i < fissurePositions.Count; i++)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(fissureMap.GetCellCenterWorld(fissurePositions[i]), 0.2f);
            }
        }
    } 
}
