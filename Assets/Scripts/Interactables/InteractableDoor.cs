using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.Interactable
{
    public class InteractableDoor : Interactable
    {
        
        public enum DoorStates
        {
            UpperLeft,
            UpperRight,
            LowerLeft,
            LowerRight
        }
        

        [Serializable]
        public struct DoorObject
        {
            public DoorStates doorState;
            public GameObject doorObject;
        }
        public GameObject doorClosed;
        public List<DoorObject> doorObjects = new List<DoorObject>();
        public DoorStates upperState;
        public DoorStates lowerState;
        public bool isOpen;
        float maxOpenTime = 5;
        
        


        public override void Start()
        {
            base.Start();
            DisableOpenDoors();

        }

        

        public override void Interact(GameObject interactor)
        {
            base.Interact(interactor);

            InteractWithDoor(interactor);
        }

        void InteractWithDoor(GameObject interactor)
        {
            
            
            
            PlayInteractionSound();
            isOpen = !isOpen;

            if (!isOpen)
            {
                CloseDoor();
                return;
            }

            bool upper = interactor.transform.position.y < transform.position.y;
            OpenDoor(upper ? upperState : lowerState);
            
            interactVerb = "Close";
            
            doorClosed.SetActive(false);
            Invoke("CloseDoor", maxOpenTime);
            
        }

        void OpenDoor(DoorStates state)
        {
            foreach (var door in doorObjects)
            {
                door.doorObject.SetActive(door.doorState == state);
            }
        }
        

        void CloseDoor()
        {
            isOpen = false;
            interactVerb = "Open";
            DisableOpenDoors();
            doorClosed.SetActive(true);
        }

        private void DisableOpenDoors()
        {
            foreach (var door in doorObjects)
            {
                door.doorObject.SetActive(false);
            }
        }


        public virtual void PlayInteractionSound()
        {
            audioManager.PlaySound(interactSound);
        }
    }

}