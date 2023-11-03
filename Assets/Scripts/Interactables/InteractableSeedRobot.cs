using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.Interactable
{
    public class InteractableSeedRobot : Interactable
    {
        public bool isOpen;
        SeedRobotDisplayUI seedRobotUI;
        public QI_Inventory inventory;
        public override void Start()
        {
            base.Start();
            seedRobotUI = SeedRobotDisplayUI.instance;
            inventory = GetComponent<QI_Inventory>();
        }

        public override void Interact(GameObject interactor)
        {
            base.Interact(interactor);

            if (!isOpen)
            {
                seedRobotUI.ShowContainerUI(inventory);
                isOpen = true;
            }
            else
            {
                seedRobotUI.HideContainerUI();
                isOpen = false;
            }
        }


        public virtual void PlayInteractionSound()
        {
            audioManager.PlaySound(interactSound);
        }
    }

}