using QuantumTek.QuantumInventory;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

[CreateAssetMenu(menuName = "Quantum Tek/Quantum Inventory/Equipment Item/SpyglassItem", fileName = "New Spyglass Item")]
public class EquipmentSpyglassData: EquipmentData
{
    public List<QI_ItemData> gatherItemData = new List<QI_ItemData>();
    public MiniGameType miniGameType;
    public LayerMask gathererLayer;
    public float detectionRadius;

    public override void UseEquippedItem()
    {
        base.UseEquippedItem();

        var player = PlayerInformation.instance;
        if (LevelManager.instance.inPauseMenu || player.playerInput.isInUI && !player.playerActivateSpyglass.SpyglassAiming)
            return;

        Vector3 pos = player.playerActivateSpyglass.GetSelectedAnimalPosition();

        if (Mathf.Sign(pos.x - player.player.position.x) < 0 && player.playerController.facingRight ||
            Mathf.Sign(pos.x - player.player.position.x) > 0 && !player.playerController.facingRight)
            player.playerController.Flip();
        pos.z = 0;

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
            float tempDistance = Vector2.Distance(position, colliders[i].transform.position);

            if (nearest == null || tempDistance < distance)
            {
                nearest = colliders[i];
                distance = tempDistance;
            }
        }


        // Found an object, no minigame required.
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
                            
                            none = false;
                            if (InteractCostReward())
                            {

                                playerInfo.playerActivateSpyglass.SlowTimeEvent(false);
                                nearestItemList.SetAsHarvested();
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
