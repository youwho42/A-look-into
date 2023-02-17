using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Quantum Tek/Quantum Inventory/Equipment Item/GathererItem", fileName = "New Gatherer Item")]
public class EquipmentGatherData : EquipmentData
{

    
    public List<QI_ItemData> gatherItemData = new List<QI_ItemData>();
    public MiniGameType miniGameType;
    public LayerMask gathererLayer;
    public float detectionRadius;
    public bool takeSample;
    public bool oneUse;

    public float playerEnergyCost;

    

    public override void UseEquippedItem()
    {
        base.UseEquippedItem();
        Vector3 pos;
        
        if(this.AnimationName == "Spyglass")
        {
           
            //var mouse = Mouse.current.position.ReadValue();
            //pos = Camera.main.ScreenToWorldPoint(mouse);
            pos = PlayerInformation.instance.playerActivateSpyglass.GetSelectedAnimalPosition();
            
            if (Mathf.Sign(pos.x - PlayerInformation.instance.player.position.x) < 0 && PlayerInformation.instance.playerController.facingRight || 
                Mathf.Sign(pos.x - PlayerInformation.instance.player.position.x) > 0 && !PlayerInformation.instance.playerController.facingRight)
                PlayerInformation.instance.playerController.Flip();
            pos.z = 0;
        }
        else
        {
            pos = PlayerInformation.instance.player.position;
        }
        Collider2D[] hit = Physics2D.OverlapCircleAll(pos, detectionRadius, gathererLayer);
        if (hit.Length > 0)
        {
            GetNearestItem(hit);
        }
        else
        {
            PlayerInformation.instance.playerAnimator.SetBool("UseEquipement", false);
        }
    }

    public void GetNearestItem(Collider2D[] colliders)
    {
        // Find nearest item.
        Collider2D nearest = null;
        float distance = 0;

        for (int i = 0; i < colliders.Length; i++)
        {
            
            float tempDistance = Vector3.Distance(PlayerInformation.instance.player.position, colliders[i].transform.position);
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
                        if (PlayerInformation.instance.playerInventory.AddItem(nearestItem.Data, 1, false))
                        {

                            if (!takeSample)
                                Destroy(nearest.gameObject);
                            else
                                EquipmentManager.instance.UnEquipAndDestroy(0);
                        }

                    }
                }// If it is a gatherable item (water...)
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
                                if (PlayerInformation.instance.playerInventory.AddItem(itemData, 1, false))
                                {

                                    if (!takeSample)
                                    {
                                        Destroy(nearest.gameObject);
                                    }
                                    else if (oneUse)
                                    {

                                        EquipmentManager.instance.UnEquipAndDestroy(0);
                                    }

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
                    
                    if (!nearestItemList.hasBeenHarvested && !PlayerInformation.instance.uiScreenVisible)
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
                                none = false;
                                if (InteractCostReward())
                                {
                                    PlayerInformation.instance.playerActivateSpyglass.SlowTimeEvent(false);
                                    nearestItemList.hasBeenHarvested = true;
                                    PlayerInformation.instance.uiScreenVisible = true;
                                    MiniGameManager.instance.StartMiniGame(miniGameType, itemData, nearest.gameObject);
                                }
                            }
                        }
                        if (none)
                        {
                            PlayerInformation.instance.playerAnimator.SetBool("UseEquipement", false);
                            NotificationManager.instance.SetNewNotification($"Cannot gather {nearestItemList.dataList[0].Name} with the {EquipmentManager.instance.currentEquipment[(int)EquipmentSlot.Hands].Name}.");
                        }
                        
                    }
                    else
                    {
                        PlayerInformation.instance.playerAnimator.SetBool("UseEquipement", false);
                        NotificationManager.instance.SetNewNotification($"You already used the {EquipmentManager.instance.currentEquipment[(int)EquipmentSlot.Hands].Name} on this {nearestItemList.dataList[0].Name} today.");

                    }
                }
                
            }
        }
    }
    bool InteractCostReward()
    {
        if (PlayerInformation.instance.playerStats.playerAttributes.GetAttributeValue("Bounce") >= playerEnergyCost)
        {

            PlayerInformation.instance.playerStats.RemoveFromBounce(playerEnergyCost);
            return true;
        }
        PlayerInformation.instance.playerAnimator.SetBool("UseEquipement", false);
        NotificationManager.instance.SetNewNotification("You are missing Yellow Bar stuff to do this.");
        return false;
    }
}
