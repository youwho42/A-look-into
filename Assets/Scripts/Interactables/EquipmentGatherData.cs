using Klaxon.StatSystem;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;

[CreateAssetMenu(menuName = "Quantum Tek/Quantum Inventory/Equipment Item/GathererItem", fileName = "New Gatherer Item")]
public class EquipmentGatherData : EquipmentData
{

    
    
    public List<QI_ItemData> gatherItemData = new List<QI_ItemData>();
    public MiniGameType miniGameType;
    public LayerMask gathererLayer;
    public float detectionRadius;
    public bool takeSample;
    public bool oneUse;

    //public float playerEnergyCost;
    public StatChanger statChanger;
    

    public override void UseEquippedItem()
    {
        base.UseEquippedItem();
        Vector3 pos;
        var player = PlayerInformation.instance;
        if (LevelManager.instance.inPauseMenu || player.playerInput.isInUI && !player.playerActivateSpyglass.SpyglassAiming)
            return;

        bool isSpyglass = false;
        if (this.AnimationName == "Spyglass")
        {
            pos = player.playerActivateSpyglass.GetSelectedAnimalPosition();
            
            if (Mathf.Sign(pos.x - player.player.position.x) < 0 && player.playerController.facingRight || 
                Mathf.Sign(pos.x - player.player.position.x) > 0 && !player.playerController.facingRight)
                player.playerController.Flip();
            pos.z = 0;
            isSpyglass = true;
        }
        else
        {
            pos = player.player.position;
        }

        Collider2D[] hit = Physics2D.OverlapCircleAll(pos, detectionRadius, gathererLayer);
        if (hit.Length > 0)
            GetNearestItem(hit, isSpyglass, pos);
        else
            player.playerAnimator.SetBool("UseEquipement", false);
    }

    public void GetNearestItem(Collider2D[] colliders, bool isSpyglass, Vector3 position)
    {
        // Find nearest item.
        Collider2D nearest = null;
        float distance = 0;
        PlayerInformation playerInfo = PlayerInformation.instance;
        for (int i = 0; i < colliders.Length; i++)
        {

            float tempDistance = 0;

            if (colliders[i].CompareTag("Water"))
            {
                if (position.z == 1)
                    tempDistance = Vector2.Distance(position, colliders[i].ClosestPoint(position));
            }
            else
            {
                if (position.z != colliders[i].transform.position.z && !isSpyglass)
                    continue;

                tempDistance = Vector2.Distance(position, colliders[i].transform.position);
            }
                
            if (tempDistance > 0.3f && this.AnimationName != "Spyglass")
                continue;

            if (nearest == null || tempDistance < distance)
            {
                nearest = colliders[i];
                distance = tempDistance;
            }
        }
        
        // Found an object, no minigame required.
        if (nearest != null)
        {
            if (miniGameType == MiniGameType.None)
            {
                // If it is a singular item (sticks, flowers...)
                if (nearest.gameObject.TryGetComponent(out QI_Item nearestItem))
                {
                    for (int i = 0; i < gatherItemData.Count; i++)
                    {
                        if (gatherItemData[i] != nearestItem.Data)
                        {
                            continue;
                        }
                        if (playerInfo.playerInventory.AddItem(nearestItem.Data, 1, false))
                        {

                            if (!takeSample)
                                Destroy(nearest.gameObject);
                            else
                                EquipmentManager.instance.UnEquipAndDestroy(0);
                            return;
                        }

                    }
                }// If it is a gatherable item (water, smells...)
                else if (nearest.gameObject.TryGetComponent(out GatherableItem nearestItemList))
                {
                    
                    foreach (QI_ItemData itemData in nearestItemList.dataList)
                    {
                        for (int i = 0; i < gatherItemData.Count; i++)
                        {
                            if (gatherItemData[i] != itemData)
                            {
                                continue;
                            }
                            if (nearestItemList.RemoveItem())
                            {
                                if (playerInfo.playerInventory.AddItem(itemData, 1, false))
                                {

                                    if (!takeSample)
                                    {
                                        Destroy(nearest.gameObject);
                                    }
                                    else if (oneUse)
                                    {

                                        EquipmentManager.instance.UnEquipAndDestroy(0);
                                    }
                                    return;
                                }
                            }

                        }
                    }
                }
            } // Found an object, minigame required.
            else
            {
                // If gatherable item (wood, ore...)
                if (nearest.gameObject.TryGetComponent(out GatherableItem nearestItemList))
                {
                    if (!nearestItemList.hasBeenHarvested)
                    {

                        bool none = true;
                        foreach (QI_ItemData itemData in nearestItemList.dataList)
                        {
                            for (int i = 0; i < gatherItemData.Count; i++)
                            {
                                if (gatherItemData[i] != itemData)
                                {
                                    continue;
                                }
                                if (!playerInfo.playerInventory.CheckInventoryHasSpace(itemData))
                                {
                                    Notifications.instance.SetNewNotification(LocalizationSettings.StringDatabase.GetLocalizedString($"Variable-Texts", "Inventory Full"), null, 0, NotificationsType.Warning);
                                    return;
                                }

                                none = false;
                                if (InteractCostReward(playerInfo))
                                {

                                    playerInfo.playerActivateSpyglass.SlowTimeEvent(false);
                                    nearestItemList.hasBeenHarvested = true;
                                    nearestItemList.harvestedSticker.SetActive(true);
                                    MiniGameManager.instance.StartMiniGame(miniGameType, itemData, nearest.gameObject);
                                    break;
                                }
                            }
                        }
                        if (none)
                        {
                            playerInfo.playerAnimator.SetBool("UseEquipement", false);
                            Notifications.instance.SetNewNotification(LocalizationSettings.StringDatabase.GetLocalizedString($"Variable-Texts", "Wrong equipment"), null, 0, NotificationsType.Warning);
                            
                            //NotificationManager.instance.SetNewNotification($"You cannot gather {nearestItemList.dataList[0].Name} with the {EquipmentManager.instance.currentEquipment[(int)EquipmentSlot.Hands].Name}.", NotificationManager.NotificationType.Warning);
                        }

                    }
                    else
                    {
                        playerInfo.playerAnimator.SetBool("UseEquipement", false);
                        Notifications.instance.SetNewNotification(LocalizationSettings.StringDatabase.GetLocalizedString($"Variable-Texts", "Already Harvested"), null, 0, NotificationsType.Warning);

                        //NotificationManager.instance.SetNewNotification($"You already harvested from this object today.", NotificationManager.NotificationType.Warning);

                    }
                }
                
            }
        }
    }
    bool InteractCostReward(PlayerInformation pInfo)
    {
        if (pInfo.statHandler.GetStatCurrentModifiedValue("Bounce") >= Mathf.Abs(statChanger.Amount))
        {
            pInfo.statHandler.ChangeStat(statChanger);
            //PlayerInformation.instance.playerStats.RemoveFromBounce(playerEnergyCost);
            return true;
        }
        pInfo.playerAnimator.SetBool("UseEquipement", false);
        Notifications.instance.SetNewNotification(LocalizationSettings.StringDatabase.GetLocalizedString($"Variable-Texts", "Missing bounce"), null, 0, NotificationsType.Warning);

        //NotificationManager.instance.SetNewNotification("You are missing Yellow Bar stuff to do this.");
        return false;
    }
}
