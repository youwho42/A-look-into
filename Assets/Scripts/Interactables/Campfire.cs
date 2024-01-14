using Klaxon.StatSystem;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Rendering.Universal;

namespace Klaxon.Interactable
{
    public class Campfire : Interactable
    {
        public GameObject fireAnimation;
        public Light2D lightFlicker;
        public FireFlicker fireFlicker;

        public bool isLit;
        public SoundSet sound;
        AudioSource source;
        string light = "light";
        string extinguish = "extinguish";
        string usedWord = "";

        public StatChanger gumptionStatChanger;
        PlayerInformation player;
        float mainVolume;
        private void Awake()
        {
            source = GetComponent<AudioSource>();
        }
        public override void Start()
        {
            base.Start();
            usedWord = light;
            SetFire(usedWord, false);
            player = PlayerInformation.instance;
            
        }
        private void OnDisable()
        {
            GameEventManager.onTimeTickEvent.RemoveListener(CheckPlayerDistance);
        }

        void CheckPlayerDistance(int tick)
        {
            if (isLit)
            {
                if (PlayerDistanceToggle.instance.GetPlayerDistance(transform, player.player.position) <= 1)
                    player.statHandler.ChangeStat(gumptionStatChanger);
            }
            
        }

        public override void SetInteractVerb()
        {
            interactVerb = LocalizationSettings.StringDatabase.GetLocalizedString($"Variable-Texts", usedWord);
        }

        public override void Interact(GameObject interactor)
        {
            
            base.Interact(interactor);
            if (!HasReadHowTo())
                return;
            if (!isLit)
                SetFire("extinguish", true);
            else
                SetFire("light", false);

        }

        public void ToggleFire(bool active)
        {
            isLit = active;
            if (active)
                SetFire("extinguish", true);
            else
                SetFire("light", false);
        }

        public override void LongInteract(GameObject interactor)
        {

            base.LongInteract(interactor);
            if (!HasReadHowTo())
                return;


            if (isLit)
            {
                Notifications.instance.SetNewNotification(LocalizationSettings.StringDatabase.GetLocalizedString($"Variable-Texts", "Fire pick up"), null, 0, NotificationsType.Warning);

                //NotificationManager.instance.SetNewNotification("You might want to put that fire out, no?", NotificationManager.NotificationType.Warning);
                return;
            }
            var item = GetComponent<QI_Item>().Data;
            if (PlayerInformation.instance.playerInventory.AddItem(item, 1, false))
            {
                if (replaceObjectOnDrop != null)
                    replaceObjectOnDrop.ShowObjects(true);

                PlayerInformation.instance.statHandler.RemoveModifiableModifier(item.placementGumption);

                Destroy(gameObject);
            }
        }

        void SetFire(string _interactVerb, bool active)
        {
            isLit = active;
            usedWord = isLit ? extinguish : light;
            fireAnimation.SetActive(active);
            lightFlicker.enabled = active;
            fireFlicker.canFlicker = active;
            SetInteractVerb();
            if(isLit)
                GameEventManager.onTimeTickEvent.AddListener(CheckPlayerDistance);
            else
                GameEventManager.onTimeTickEvent.RemoveListener(CheckPlayerDistance);
            //interactVerb = _interactVerb;
            fireFlicker.StartLightFlicker(active);
            if (sound.clips.Length > 0)
            {
                if (active)
                    PlaySound();
                else
                    StopSound();
            }
        }
        void PlaySound()
        {
            sound.SetSource(source, 0);
            mainVolume = sound.volume;
            sound.Play();
        }
        void StopSound()
        {
            sound.SetSource(source, 0);
            sound.Stop();
        }




    } 
}
