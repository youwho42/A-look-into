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
        if (equipmentManager.currentEquipment[(int)EquipmentSlot.Hands] == null || UIScreenManager.instance.GetCurrentUI() != UIScreenType.None)
            return;
        
        if (equipmentManager.currentEquipment[(int)EquipmentSlot.Hands].AnimationName == "Spyglass")
        {
            
            if (PlayerInformation.instance.playerActivateSpyglass.selectedAnimal == null)
                return;
            
            animator.SetBool("UseEquipement", true);
            equipmentManager.currentEquipment[(int)EquipmentSlot.Hands].UseEquippedItem();
            
        }
        else if (equipmentManager.currentEquipment[(int)EquipmentSlot.Hands].AnimationName == "Poke")
        {
            equipmentManager.currentEquipment[(int)EquipmentSlot.Hands].UseEquippedItem();
            animator.SetTrigger(equipmentManager.currentEquipment[(int)EquipmentSlot.Hands].AnimationName);
        }
        else
        {
            animator.SetBool("UseEquipement", false);
            animator.SetTrigger("Swing_" + equipmentManager.currentEquipment[(int)EquipmentSlot.Hands].AnimationName);
            equipmentManager.currentEquipment[(int)EquipmentSlot.Hands].UseEquippedItem();
        }

    }
}
