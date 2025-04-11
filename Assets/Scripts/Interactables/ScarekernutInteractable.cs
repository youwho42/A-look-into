using QuantumTek.QuantumInventory;
using System.Collections;
using UnityEngine;

namespace Klaxon.Interactable
{
    public class ScarekernutInteractable : Interactable
    {
        public float maxDistance;
        QI_Item interactableItem;
        QI_ItemData pickUpItem;
        public GameObject visibleRadius;

        public override void Start()
        {
            base.Start();
            interactableItem = GetComponent<QI_Item>();
            //interactVerb = LocalizationSettings.StringDatabase.GetLocalizedString($"Items-{interactableItem.Data.Type.ToString()}", interactableItem.Data.Name);
            pickUpItem = interactableItem.Data.pickUpItem == null ? interactableItem.Data : interactableItem.Data.pickUpItem;
            //visibleRadius.SetActive(false);
        }
        public override void SetInteractVerb()
        {
            interactVerb = localizedInteractVerb.GetLocalizedString();
            //interactVerb = LocalizationSettings.StringDatabase.GetLocalizedString($"Items-{interactableItem.Data.Type}", interactableItem.Data.Name);
        }

        public override void LongInteract(GameObject interactor)
        {
            base.Interact(interactor);
            if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.None)
                StartCoroutine(InteractCo(interactor));


        }

        IEnumerator InteractCo(GameObject interactor)
        {
            var playerInventory = PlayerInformation.instance.playerInventory;
            interactor.GetComponent<AnimatePlayer>().TriggerPickUp();
            yield return new WaitForSeconds(0.33f);


            
            
            if (playerInventory.AddItem(pickUpItem, 1, false))
            {
                PlayInteractSound(true);
                Notifications.instance.SetNewNotification("", pickUpItem, 1, NotificationsType.Inventory);
              
                if (pickUpItem.placementGumption != null)
                    PlayerInformation.instance.statHandler.RemoveModifiableModifier(pickUpItem.placementGumption);
                Destroy(gameObject);
                

            }
            else
            {
                PlayInteractSound(false);
            }

            hasInteracted = false;


            WorldItemManager.instance.RemoveItemFromWorldItemDictionary(interactableItem.Data.Name, 1);
        }


        void PlayInteractSound(bool success)
        {
            string sound = success ? interactSound : interactFailSound;
            if (audioManager.CompareSoundNames("PickUp-" + sound))
            {
                audioManager.PlaySound("PickUp-" + sound);
            }
        }

    

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, maxDistance);
        }
    } 
}
