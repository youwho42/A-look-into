using QuantumTek.QuantumInventory;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

[CreateAssetMenu(menuName = "Quantum Tek/Quantum Inventory/Equipment Item/GathererItem", fileName = "New Gatherer Item")]
public class EquipmentGatherData : EquipmentData
{

    public List<QI_ItemData> gatherItemData = new List<QI_ItemData>();
    public LayerMask gathererLayer;
    public float detectionRadius;
    
    public override void UseEquippedItem()
    {
        base.UseEquippedItem();
        
        var player = PlayerInformation.instance;
        if (LevelManager.instance.inPauseMenu || player.playerInput.isInUI)
            return;


        Vector3 pos = player.player.position;
        

        Collider2D[] hit = Physics2D.OverlapCircleAll(pos, detectionRadius, gathererLayer);
        if (hit.Length > 0)
            GetNearestItem(hit, pos);
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

            float tempDistance = 0;

            if (colliders[i].CompareTag("Water"))
            {
                if (position.z == 1)
                    tempDistance = NumberFunctions.GetDistanceV2(position, colliders[i].ClosestPoint(position));
            }
            else
            {
                tempDistance = NumberFunctions.GetDistanceV2(position, colliders[i].transform.position);
            }
                
            if (nearest == null || tempDistance < distance)
            {
                nearest = colliders[i];
                distance = tempDistance;
            }
        }
        
        // Found an object, no minigame required.
        if (nearest != null)
        {
            // If it is a gatherable item (water, smells...)
            if (nearest.gameObject.TryGetComponent(out GatherableItem nearestItemList))
            {
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
                        if (nearestItemList.RemoveItem())
                        {
                            if (playerInfo.playerInventory.AddItem(itemData, 1, false))
                            {
                                EquipmentManager.instance.UnEquipAndDestroy(0);
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
    
}
