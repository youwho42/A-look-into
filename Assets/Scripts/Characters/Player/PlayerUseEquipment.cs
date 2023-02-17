using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUseEquipment : MonoBehaviour
{
    EquipmentManager equipmentManager;
    
    public Animator animator;
    private void Start()
    {
        equipmentManager = EquipmentManager.instance;
        
        GameEventManager.onUseEquipmentEvent.AddListener(UseEquipment);
    }

    private void OnDisable()
    {
        GameEventManager.onUseEquipmentEvent.RemoveListener(UseEquipment);
    }

    

    void UseEquipment()
    {
        if (PlayerInformation.instance.uiScreenVisible || equipmentManager.currentEquipment[(int)EquipmentSlot.Hands] == null)
            return;

        if (equipmentManager.currentEquipment[(int)EquipmentSlot.Hands].AnimationName == "Spyglass")
        {
            if (PlayerInformation.instance.playerActivateSpyglass.selectedAnimal == null)
                return;
            animator.SetBool("UseEquipement", true);
            equipmentManager.currentEquipment[(int)EquipmentSlot.Hands].UseEquippedItem();
            
        }
        else
        {
            animator.SetBool("UseEquipement", false);
            animator.SetTrigger("Swing_" + equipmentManager.currentEquipment[(int)EquipmentSlot.Hands].AnimationName);
            equipmentManager.currentEquipment[(int)EquipmentSlot.Hands].UseEquippedItem();

        }

    }
}
