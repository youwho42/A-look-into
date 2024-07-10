using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActivateSpyglass : MonoBehaviour
{
    
    //List<GatherableItem> currentAnimals = new List<GatherableItem>();
    Dictionary<GatherableItem, SpriteRenderer> allAnimals = new Dictionary<GatherableItem, SpriteRenderer>();
    public LayerMask detectionLayer;
    public GameObject spyglassReticle;
    bool spyglassAiming;
    public bool SpyglassAiming
    {
        get { return spyglassAiming;}
    }
    int currentSelected;
    [HideInInspector]
    public GatherableItem selectedAnimal;
    private void Start()
    {
        GameEventManager.onSpyglassAimEvent.AddListener(SlowTimeEvent);
        GameEventManager.onSpyglassAimChageSelectedEvent.AddListener(ChangeSelectedAnimal);
    }
    private void OnDisable()
    {
        GameEventManager.onSpyglassAimEvent.RemoveListener(SlowTimeEvent);
        GameEventManager.onSpyglassAimChageSelectedEvent.RemoveListener(ChangeSelectedAnimal);

    }

    private void Update()
    {

        if (spyglassAiming && allAnimals.Count > 0)
        {
            CheckAnimalsStillVisible();
            GetAllAnimals();
            SetAnimalSelected(selectedAnimal);
        }
    }

    public void SlowTimeEvent(bool active)
    {
        if (UIScreenManager.instance.GetCurrentUI() != UIScreenType.None || EquipmentManager.instance.currentEquipment[(int)EquipmentSlot.Hands] == null || Time.timeScale == 0f)
            return;
        if (EquipmentManager.instance.currentEquipment[(int)EquipmentSlot.Hands].AnimationName != "Spyglass")
            return;
        if (!active)
        {
            PlayerInformation.instance.playerInput.isInUI = false;
            spyglassReticle.SetActive(false);
            spyglassAiming = false;
            allAnimals.Clear();
            selectedAnimal = null;
            Time.timeScale = 1f;
            return;
        }
            
        if (EquipmentManager.instance.currentEquipment[(int)EquipmentSlot.Hands].AnimationName == "Spyglass")
        {
            
            PlayerInformation.instance.playerInput.isInUI = true;
            spyglassAiming = true;
            currentSelected = 0;
            GetAllAnimals();
            ChangeSelectedAnimal(0);
            Time.timeScale = 0.25f;
        }
            
    }

    void GetAllAnimals()
    {

        
        Vector2 pointA = Camera.main.rect.min;
        Vector2 pointB = Camera.main.rect.max;
        
        pointA = Camera.main.ViewportToWorldPoint(pointA);
        pointB = Camera.main.ViewportToWorldPoint(pointB);
        var all = Physics2D.OverlapAreaAll(pointA, pointB, detectionLayer);
        if(all.Length > 0)
        {
            for (int i = 0; i < all.Length; i++)
            {
                if (all[i].GetComponentInParent<IAnimal>() != null) 
                { 
                    var animal = all[i].GetComponent<GatherableItem>();
                    if (animal == null || animal.hasBeenHarvested)
                        continue;
                    if(!allAnimals.ContainsKey(animal))
                    {
                        allAnimals.Add(animal, animal.GetComponent<SpriteRenderer>());
                        
                    }
                }
            }
        }

    }

    void CheckAnimalsStillVisible()
    {
        if(!spyglassAiming) 
            return;
        bool selectedChanged = false;
        List<GatherableItem> removes = new List<GatherableItem>();
        foreach (var item in allAnimals)
        {
            if (!allAnimals[item.Key].isVisible)
                removes.Add(item.Key);
        }
        for (int i = 0; i < removes.Count; i++)
        {
            if (removes[i] == selectedAnimal)
                selectedChanged= true;
            allAnimals.Remove(removes[i]);
        }
        if(selectedChanged)
            ChangeSelectedAnimal(0);
    }

    void ChangeSelectedAnimal(int dir)
    {
        currentSelected += dir;
        if(currentSelected > allAnimals.Count-1)
            currentSelected = 0;
        else if (currentSelected < 0)
            currentSelected = allAnimals.Count-1;

        int x = 0;
        foreach (var animal in allAnimals)
        {
            if (currentSelected == x)
            {
                selectedAnimal = animal.Key;
                return;
            }
            x++;
        }
        SetAnimalSelected(selectedAnimal);
    }

    void SetAnimalSelected(GatherableItem animal)
    {
        if (animal == null)
            return;
        Vector3 pos = allAnimals[animal].transform.position;
        pos.z = allAnimals[animal].transform.position.z + 0.5f;
        spyglassReticle.transform.position = pos;
        
        if (!spyglassReticle.activeInHierarchy)
                    spyglassReticle.SetActive(true);
    }

    public Vector2 GetSelectedAnimalPosition()
    {
        return allAnimals[selectedAnimal].transform.position;
    }
}
