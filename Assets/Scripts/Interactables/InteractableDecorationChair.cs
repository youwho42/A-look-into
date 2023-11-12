using Klaxon.StatSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaxon.Interactable 
{ 
    public class InteractableDecorationChair : InteractableDecoration
    {

        bool isSitting;
        PlayerInformation player;
        public bool facingRight;
        public StatChanger gumptionChanger;
        Vector3 lastPos;

        public override void Start()
        {
            base.Start();
        }

        public override void SetInteractVerb()
        {
            interactVerb = localizedInteractVerb.GetLocalizedString();
        }

        public override void Interact(GameObject interactor)
        {
            base.Interact(interactor);
            player = PlayerInformation.instance;
            if (!isSitting)
            {
                StartCoroutine(PlacePlayer(transform.position));
                if (player.playerController.facingRight && !facingRight || !player.playerController.facingRight && facingRight)
                    player.playerController.Flip();
                player.playerAnimator.SetBool("IsSitting", true);
                player.isSitting = true;
                isSitting = true;
                canInteract = false;
                GameEventManager.onTimeTickEvent.AddListener(CheckAddGumption);
            }
        }

        void CheckAddGumption(int tick)
        {
            if (isSitting)
                PlayerInformation.instance.statHandler.ChangeStat(gumptionChanger);
        }


        IEnumerator PlacePlayer(Vector3 position)
        {
            lastPos = player.player.position;
            float timer = 0;
            float maxTime = 0.45f;
            while (timer < maxTime)
            {
                Vector3 pos = Vector3.Lerp(player.player.position, position, timer / maxTime);
                player.player.position = pos;
                timer += Time.deltaTime;
                yield return null;
            }

        }

        private void OnDisable()
        {
            GameEventManager.onTimeTickEvent.RemoveListener(CheckAddGumption);
        }
        private void Update()
        {
            if (!isSitting)
                return;

            if (player.playerInput.movement != Vector2.zero)
            {
                StartCoroutine(PlacePlayer(lastPos));
                player.playerAnimator.SetBool("IsSitting", false);
                playerInformation.isSitting = false;
                isSitting = false;
                canInteract = true;
                GameEventManager.onTimeTickEvent.RemoveListener(CheckAddGumption);

            }

        }

    }

}
