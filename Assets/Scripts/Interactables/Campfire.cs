using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Rendering.Universal;

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

    public override void LongInteract(GameObject interactor)
    {

        base.LongInteract(interactor);
        if (isLit)
        {
            Notifications.instance.SetNewNotification(LocalizationSettings.StringDatabase.GetLocalizedString($"Variable-Texts", "Fire pick up"), null, 0, NotificationsType.Warning);

            //NotificationManager.instance.SetNewNotification("You might want to put that fire out, no?", NotificationManager.NotificationType.Warning);
            return;
        }

        if (PlayerInformation.instance.playerInventory.AddItem(GetComponent<QI_Item>().Data, 1, false))
        {
            if (TryGetComponent(out ReplaceObjectOnItemDrop obj))
                obj.ShowObjects(true);

            Destroy(gameObject);
        }
    }

    void SetFire(string _interactVerb, bool active)
    {
        isLit = active;
        usedWord = isLit? extinguish : light;
        fireAnimation.SetActive(active);
        lightFlicker.enabled = active;
        fireFlicker.canFlicker = active;
        SetInteractVerb();
        //interactVerb = _interactVerb;
        fireFlicker.StartLightFlicker(active);
        if(sound.clips.Length > 0) 
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
