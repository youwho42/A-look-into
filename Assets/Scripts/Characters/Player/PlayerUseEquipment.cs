using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUseEquipment : MonoBehaviour
{
    EquipmentManager equipmentManager;
    PlayerInput playerInput;
    public Animator animator;
    private void Start()
    {
        equipmentManager = EquipmentManager.instance;
        playerInput = GetComponent<PlayerInput>();
    }


    private void Update()
    {
        if (!PlayerInformation.instance.uiScreenVisible)
        {
            if (playerInput.usingEquippedItem && equipmentManager.currentEquipment[(int)EquipmentSlot.Hands] != null)
            {
                if (equipmentManager.currentEquipment[(int)EquipmentSlot.Hands].AnimationName == "Spyglass")
                {
                    
                    if (!PlayerInformation.instance.uiScreenVisible)
                    {
                        if (UIScreenManager.instance.CurrentUIScreen() != UIScreenType.PlayerUI)
                            return;
                        animator.SetBool("UseEquipement", true);
                        equipmentManager.currentEquipment[(int)EquipmentSlot.Hands].UseEquippedItem();
                       
                    }
                    
                    
                }
                else
                {
                    animator.SetBool("UseEquipement", false);
                    equipmentManager.currentEquipment[(int)EquipmentSlot.Hands].UseEquippedItem();
                    animator.SetTrigger("Swing_" + equipmentManager.currentEquipment[(int)EquipmentSlot.Hands].AnimationName);
                }
                
                
                
            }
        }
    }
}
