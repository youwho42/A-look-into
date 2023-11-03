using System.Collections;
using System.Collections.Generic;
using QuantumTek.QuantumInventory;
using UnityEngine;

namespace Klaxon.Interactable
{
    public class InteractableAgencyPickUp : Interactable
    {
        QI_Item interactableItem;




        bool addedToInventory;

        public override void Start()
        {
            base.Start();
            interactableItem = GetComponent<QI_Item>();
            interactVerb = interactableItem.Data.Name;

        }

        public override void Interact(GameObject interactor)
        {
            base.Interact(interactor);

            StartCoroutine(InteractCo(interactor));


        }

        IEnumerator InteractCo(GameObject interactor)
        {
            interactor.GetComponent<AnimatePlayer>().TriggerPickUp();
            yield return new WaitForSeconds(0.33f);
            PlayInteractSound();

            PlayerInformation.instance.playerStats.AddToAgency(agencyReward);



            hasInteracted = false;

            WorldItemManager.instance.RemoveItemFromWorldItemDictionary(interactableItem.Data.Name, 1);
            Destroy(gameObject);

        }


        void PlayInteractSound()
        {
            if (audioManager.CompareSoundNames("PickUp-" + interactSound))
            {
                audioManager.PlaySound("PickUp-" + interactSound);
            }
        }

    }

}