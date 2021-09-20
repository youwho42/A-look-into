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
                equipmentManager.currentEquipment[(int)EquipmentSlot.Hands].UseEquippedItem();
                animator.SetTrigger("Swing");
            }
        }
    }
}
