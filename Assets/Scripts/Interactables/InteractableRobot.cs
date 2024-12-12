using Klaxon.GOAD;
using QuantumTek.QuantumInventory;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.Interactable
{
    public enum RobotPriorityTypes
    {
        Ores,
        Woods,
        Seeds,
        Foods,
        Misc
    }
    public class InteractableRobot : Interactable
	{
        public bool isOpen;
        
        public QI_Inventory selfInventory;

        RobotDisplayUI robotUI;
        public int currentPriority;
        

        [Serializable]
        public class RobotPriorityItem
        {
            public RobotPriorityTypes PriorityType;
            public QI_ItemDatabase PriorityDatabase;
            public bool isActive;
        }
        public List<RobotPriorityItem> robotPriorities = new List<RobotPriorityItem>();
        public override void Start()
        {
            base.Start();
            robotUI = RobotDisplayUI.instance;
            SetCurrentPriority((RobotPriorityTypes)currentPriority);
        }

        
        public override void Interact(GameObject interactor)
        {
            base.Interact(interactor);
            if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.None)
                OpenRobot();
            else if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.RobotUI)
                CloseRobot();
        }

        void OpenRobot()
        {
            if (UIScreenManager.instance.DisplayIngameUI(UIScreenType.RobotUI, true))
            {
                isOpen = true;
                robotUI.ShowUI(this);
            }
        }

        void CloseRobot()
        {
            isOpen = false;
            UIScreenManager.instance.HideScreenUI();
            robotUI.HideUI();
        }

        public void SetCurrentPriority(RobotPriorityTypes priorityType)
        {
            for (int i = 0; i < robotPriorities.Count; i++)
            {
                robotPriorities[i].isActive = robotPriorities[i].PriorityType == priorityType;
                if (robotPriorities[i].isActive)
                    currentPriority = i;
            }
            
        }

        
    }

}