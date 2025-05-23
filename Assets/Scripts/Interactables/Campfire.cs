﻿using Klaxon.GOAD;
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
        //[HideInInspector]
        public bool hasBP;
        


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
            GameEventManager.onPlayerPositionUpdateEvent.RemoveListener(CheckPlayerDistance);
        }

        void CheckPlayerDistance()
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
            
            if (!isLit)
                SetFire("extinguish", true);
            else
                SetFire("light", false);

        }

        public void ToggleFire(bool active)
        {
            hasBP = false;
            isLit = active;
            if (active)
                SetFire("extinguish", true);
            else
                SetFire("light", false);
        }

        public override void LongInteract(GameObject interactor)
        {

            base.LongInteract(interactor);
            


            if (isLit)
            {
                Notifications.instance.SetNewNotification(LocalizationSettings.StringDatabase.GetLocalizedString($"Variable-Texts", "Fire pick up"), null, 0, NotificationsType.Warning);
                return;
            }
            var item = GetComponent<QI_Item>().Data;
            if (PlayerInformation.instance.playerInventory.AddItem(item, 1, false))
            {
                

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
                GameEventManager.onPlayerPositionUpdateEvent.AddListener(CheckPlayerDistance);
            else
                GameEventManager.onPlayerPositionUpdateEvent.RemoveListener(CheckPlayerDistance);
            fireFlicker.StartLightFlicker(active);
            if (sound.clips.Length > 0)
            {
                if (active)
                    PlaySound();
                else
                    StopSound();
            }
            GameEventManager.onLightsToggleEvent.Invoke();
        }
        void PlaySound()
        {
            sound.SetSource(source, 0);
            sound.Play();
        }
        void StopSound()
        {
            sound.SetSource(source, 0);
            sound.Stop();
        }
        public void BPSetFire()
        {
            bool isDay = RealTimeDayNightCycle.instance.dayState == RealTimeDayNightCycle.DayState.Day;
            if (!isLit && !isDay || isLit && isDay)
                ToggleFire(!isDay);
        }

        

    } 
}
