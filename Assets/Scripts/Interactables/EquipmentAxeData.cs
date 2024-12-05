using QuantumTek.QuantumInventory;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

[CreateAssetMenu(menuName = "Quantum Tek/Quantum Inventory/Equipment Item/AxeItem", fileName = "New Axe Item")]
public class EquipmentAxeData : EquipmentData
{

    public List<QI_ItemData> gatherItemData = new List<QI_ItemData>();
    public MiniGameType miniGameType;
    public LayerMask gathererLayer;
    public float detectionRadius;

    public override void UseEquippedItem()
    {
        base.UseEquippedItem();

        var player = PlayerInformation.instance;
        if (LevelManager.instance.inPauseMenu || player.playerInput.isInUI)
            return;


        Collider2D[] hit = Physics2D.OverlapCircleAll(player.player.position, detectionRadius, gathererLayer, player.player.position.z, player.player.position.z);
        if (hit.Length > 0)
            GetNearestItem(hit, player.player.position);
        else
            player.playerAnimator.SetBool("UseEquipement", false);
    }


    public void GetNearestItem(Collider2D[] colliders, Vector3 position)
    {
        // Find nearest item.
        Collider2D nearest = null;
        float distance = 0;
        PlayerInformation playerInfo = PlayerInformation.instance;
        for (int i = 0; i < colliders.Length; i++)
        {

            float tempDistance = Vector2.Distance(position, colliders[i].transform.position);
            

            if (tempDistance > 0.3f)
                continue;

            if (nearest == null || tempDistance < distance)
            {
                nearest = colliders[i];
                distance = tempDistance;
            }
        }

        
        if (nearest != null)
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
                                continue;
                            
                            if (!playerInfo.playerInventory.CheckInventoryHasSpace(itemData))
                            {
                                Notifications.instance.SetNewNotification(LocalizationSettings.StringDatabase.GetLocalizedString($"Variable-Texts", "Inventory Full"), null, 0, NotificationsType.Warning);
                                return;
                            }

                            none = false;
                            if (InteractCostReward())
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
                    }
                }
                else
                {
                    playerInfo.playerAnimator.SetBool("UseEquipement", false);
                    Notifications.instance.SetNewNotification(LocalizationSettings.StringDatabase.GetLocalizedString($"Variable-Texts", "Already Harvested"), null, 0, NotificationsType.Warning);
                }
            }

            
        }
    }

}
