using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Campfire : Interactable
{
    public GameObject fireAnimation;
    public Light2D lightFlicker;
    public FireFlicker fireFlicker;
    bool isLit;
    public SoundSet sound;
    AudioSource source;
    float mainVolume;
    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }
    public override void Start()
    {
        base.Start();
        SetFire("light", false);
        
        GameEventManager.onVolumeChangedEvent.AddListener(ChangeVolume);

    }
    private void OnDisable()
    {
        GameEventManager.onVolumeChangedEvent.RemoveListener(ChangeVolume);
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
            NotificationManager.instance.SetNewNotification("You might want to put that fire out, no?", NotificationManager.NotificationType.Warning);
            return;
        }

        if (PlayerInformation.instance.playerInventory.AddItem(GetComponent<QI_Item>().Data, 1, false))
            Destroy(gameObject);

    }

    void SetFire(string _interactVerb, bool active)
    {
        isLit = active;
        fireAnimation.SetActive(active);
        lightFlicker.enabled = active;
        fireFlicker.canFlicker = active;
        interactVerb = _interactVerb;
        fireFlicker.StartLightFlicker(active);
        if (active)
            PlaySound();
        else
            StopSound();
    }
    void PlaySound()
    {
        sound.SetSource(source, 0);
        mainVolume = sound.volume;
        ChangeVolume();
        sound.Play(AudioTrack.Effects);
    }
    void StopSound()
    {
        sound.SetSource(source, 0);
        sound.Stop();
    }

    void ChangeVolume()
    {
        source.volume = mainVolume * PlayerPreferencesManager.instance.GetTrackVolume(AudioTrack.Effects);
    }


}
