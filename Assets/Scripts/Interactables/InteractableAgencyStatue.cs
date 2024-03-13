using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Klaxon.Interactable
{
    public class InteractableAgencyStatue : Interactable, IResetAtDawn
    {
        
        public QI_ItemData agencyItem;
        public int agencyAmount;
        Material material;
        public SpriteRenderer rend;
        [ColorUsageAttribute(false, true)]
        public Color initialColor;
        public float onIntensity;
        public float fullIntensity;
        public float offIntensity;
        public bool hasBeenActivated;
        ParticlesToPlayer particles;
        AudioSource source;
        public SoundSet activateSound;
        public SoundSet aquireSound;
        float mainVolume;
        DrawZasYDisplacement agencySpawnDisplacement;
        public override void Start()
        {
            base.Start();
            GetMaterial();
            if (!hasBeenActivated)
            {
                float factor = Mathf.Pow(2, onIntensity);
                material.SetColor("_EmissionColor", initialColor * factor);

            }
            source = GetComponent<AudioSource>();
            particles = GetComponent<ParticlesToPlayer>();
            agencySpawnDisplacement = GetComponent<DrawZasYDisplacement>();
            //SetToManager();
            //GameEventManager.onTimeHourEvent.AddListener(ResetStatue);
        }
        //public void OnDisable()
        //{
        //    GameEventManager.onTimeHourEvent.RemoveListener(ResetStatue);
        //}

        void GetMaterial()
        {
            material = rend.material;

        }
        public void ResetAtDawn()
        {
            float factor = Mathf.Pow(2, onIntensity);
            material.SetColor("_EmissionColor", initialColor * factor);
            hasBeenActivated = false;
            canInteract = true;
        }
        //void ResetStatue(int time)
        //{
        //    if (time == 5)
        //    {
        //        float factor = Mathf.Pow(2, onIntensity);
        //        material.SetColor("_EmissionColor", initialColor * factor);
        //        hasBeenActivated = false;
        //        canInteract = true;
        //    }
        //}

        public override void Interact(GameObject interactor)
        {
            base.Interact(interactor);

            if (!hasBeenActivated && PlayerInformation.instance.playerInventory.CheckInventoryHasSpace(agencyItem))
                StartCoroutine(InteractCo(interactor));
            //else
            //    Notifications.instance.SetNewNotification(LocalizationSettings.StringDatabase.GetLocalizedString($"Variable-Texts", "Inventory full"), null, 0, NotificationsType.Warning);
        
        }

        IEnumerator InteractCo(GameObject interactor)
        {
            canInteract = false;
            hasBeenActivated = true;

            PlaySound(activateSound);
            float elapsedTime = 0;
            float waitTime = 1.5f;
            while (elapsedTime < waitTime)
            {
                float c = Mathf.Lerp(onIntensity, fullIntensity, (elapsedTime / waitTime));
                float factor = Mathf.Pow(2, c);
                SetMaterialColor(factor);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            PlaySound(aquireSound);
            particles.SpawnParticles(agencyAmount, agencySpawnDisplacement);
            yield return new WaitForSeconds(0.5f);


            elapsedTime = 0;
            waitTime = 0.5f;
            while (elapsedTime < waitTime)
            {
                float c = Mathf.Lerp(fullIntensity, 0, (elapsedTime / waitTime));
                float factor = Mathf.Pow(2, c);
                SetMaterialColor(factor);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            yield return null;
            elapsedTime = 0;
            waitTime = 0.5f;

            while (elapsedTime < waitTime)
            {
                float c = Mathf.Lerp(0, Mathf.Abs(offIntensity), (elapsedTime / waitTime));
                float factor = Mathf.Pow(2, c) * -1;
                SetMaterialColor(factor);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            PlayerInformation.instance.playerInventory.AddItem(agencyItem, agencyAmount, false);
            Notifications.instance.SetNewNotification("", agencyItem, agencyAmount, NotificationsType.Inventory);
            //NotificationManager.instance.SetNewNotification($"{agencyItem.Name} {agencyAmount}", NotificationManager.NotificationType.Inventory);
        }

        public void SetMaterialColor(float intensity)
        {
            if (material == null)
                GetMaterial();
            material.SetColor("_EmissionColor", initialColor * intensity);
        }

        public void SetSaveColor(bool isOn)
        {
            if (material == null)
                GetMaterial();
            float factor;
            if (isOn)
                factor = Mathf.Pow(2, onIntensity);
            else
                factor = Mathf.Pow(2, Mathf.Abs(offIntensity)) * -1;


            material.SetColor("_EmissionColor", initialColor * factor);
        }

        void PlaySound(SoundSet set)
        {

            int t = Random.Range(0, set.clips.Length);
            set.SetSource(source, t);
            mainVolume = set.volume;
            set.Play();

        }

        //public void SetToManager()
        //{
        //    ResetAtDawnManager.instance.AddToManager(this);
        //}
    }

}