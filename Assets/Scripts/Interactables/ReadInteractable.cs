﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Klaxon.Interactable
{
    public class ReadInteractable : Interactable
    {


        public string coversation;



        public override void Start()
        {
            base.Start();

        }
        public override void Interact(GameObject interactor)
        {
            base.Interact(interactor);
            canInteract = false;
            Read();
        }

        private void Read()
        {

            if(UIScreenManager.instance.DisplayIngameUI(UIScreenType.DialogueUI, true))
            {
                PlayerInformation.instance.uiScreenVisible = true;
                PlayerInformation.instance.TogglePlayerInput(false);
            }
            

        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {

                canInteract = true;

            }

        }
    }

}