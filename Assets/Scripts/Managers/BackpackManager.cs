using QuantumTek.QuantumInventory;
using System.Collections;
using UnityEngine;

public class BackpackManager : MonoBehaviour
{
    public QI_Inventory playerInventory;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(2);
        GameEventManager.onEquipmentUpdateEvent.AddListener(SetInventoryBackpack);
    }

    private void OnDisable()
    {
        GameEventManager.onEquipmentUpdateEvent.RemoveListener(SetInventoryBackpack);
    }


    void SetInventoryBackpack()
    {
        var currentBackpack = EquipmentManager.instance.currentEquipment[(int)EquipmentSlot.Backpack] as EquipmentBackpackData;
        int stacks = 12;
        int additionalSlots = currentBackpack == null ? 0 : currentBackpack.additionalSlots;
        playerInventory.MaxStacks = stacks + additionalSlots;
        GameEventManager.onInventoryResetEvent.Invoke();
    }
}
