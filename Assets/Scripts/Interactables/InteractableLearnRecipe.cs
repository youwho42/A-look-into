using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuantumTek.QuantumInventory;

namespace Klaxon.Interactable
{
    public class InteractableLearnRecipe : Interactable
    {
        public QI_CraftingRecipe craftingRecipe;

        public override void Start()
        {
            base.Start();

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

            Destroy(gameObject);
            hasInteracted = false;

            WorldItemManager.instance.RemoveItemFromWorldItemDictionary(craftingRecipe.Name, 1);
            


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