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
        // Use a HashSet for faster lookups.
        HashSet<QI_ItemData> gatherItemDataSet = new HashSet<QI_ItemData>(gatherItemData);

        PlayerInformation playerInfo = PlayerInformation.instance;
        Collider2D nearest = null;
        float shortestDistance = float.MaxValue;
        QI_ItemData targetItem = null;

        foreach (var collider in colliders)
        {
            // Check if the collider has a GatherableItem component.
            if (!collider.gameObject.TryGetComponent(out GatherableItem potentialGatherable))
                continue;

            // Check if there’s any matching item data.
            if (!potentialGatherable.dataList.Exists(data => gatherItemDataSet.Contains(data)))
                continue;

            // Calculate the distance to the potential gatherable item.
            Vector3 collPosition = collider.ClosestPoint(position);
            float distance = Vector2.Distance(position, collPosition);

            if (distance > 0.3f || distance >= shortestDistance)
                continue;

            nearest = collider;
            shortestDistance = distance;

            // Cache the first matching item for later use.
            targetItem = potentialGatherable.dataList.Find(data => gatherItemDataSet.Contains(data));
        }

        // No valid item found.
        if (nearest == null)
        {
            playerInfo.playerAnimator.SetBool("UseEquipement", false);
            Notifications.instance.SetNewNotification(
                LocalizationSettings.StringDatabase.GetLocalizedString("Variable-Texts", "Wrong equipment"),
                null, 0, NotificationsType.Warning);
            return;
        }

        // Process the nearest item.
        if (nearest.gameObject.TryGetComponent(out GatherableItem gatherableItem))
        {
            if (gatherableItem.hasBeenHarvested)
            {
                playerInfo.playerAnimator.SetBool("UseEquipement", false);
                Notifications.instance.SetNewNotification(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Variable-Texts", "Already Harvested"),
                    null, 0, NotificationsType.Warning);
                return;
            }

            if (!playerInfo.playerInventory.CheckInventoryHasSpace(targetItem))
            {
                Notifications.instance.SetNewNotification(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Variable-Texts", "Inventory Full"),
                    null, 0, NotificationsType.Warning);
                return;
            }

            if (InteractCostReward())
            {
                playerInfo.playerActivateSpyglass.SlowTimeEvent(false);
                gatherableItem.hasBeenHarvested = true;
                gatherableItem.harvestedSticker.SetActive(true);

                MiniGameManager.instance.StartMiniGame(miniGameType, targetItem, nearest.gameObject);
            }
        }
    }

}
