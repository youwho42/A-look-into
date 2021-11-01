using QuantumTek.QuantumInventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AnimalSpawner : MonoBehaviour
{
    [Serializable]
    public struct Animal
    {
        public GameObject animalPrefab;
        public List<QI_ItemData> animalReason;
        public int reasonAmountNeeded;
        public GameObject animalHome;
        public AnimationCurve spawnChanceCurve;
    }

    public List<Animal> possibleAnimals = new List<Animal>();

    public Grid groundGrid;
    public Tilemap groundMap;
    public List<Vector3> possibleTiles = new List<Vector3>();

    private void Start()
    {
        DayNightCycle.instance.FullHourEventCallBack.AddListener(SetNewAnimals);

        for (int x = groundMap.cellBounds.xMin + 7; x <= groundMap.cellBounds.xMax -7; x++)
        {
            for (int y = groundMap.cellBounds.yMin + 7; y <= groundMap.cellBounds.yMax - 7; y++)
            {
                possibleTiles.Add(new Vector3Int(x, y, 0));
            }
        }


    }
    void SetNewAnimals(int time)
    {
        
            CheckForViableTree();
        
    }

    void CheckForViableTree()
    {
        foreach (var tile in possibleTiles)
        {
            var hit = Physics2D.OverlapCircleAll((Vector2)tile, 0.33f);
            if (hit.Length > 0)
            {
                for (int i = 0; i < hit.Length; i++)
                {
                    if(hit[i].TryGetComponent(out PlantLifeCycle plant))
                    {
                        if(plant.currentCycle >= plant.gatherableCycle)
                            CheckEnvironment(plant);
                    }
                }
            }
        }
    }

    public void CheckEnvironment(PlantLifeCycle plant)
    {
        Dictionary<Animal, int> animals = new Dictionary<Animal, int>();
        // check that reasons are met to spawn an animal
        var hit = Physics2D.OverlapCircleAll(plant.transform.position, 1);
        if (hit.Length > 0)
        {
            foreach (var collider2D in hit)
            {
                if (collider2D.TryGetComponent(out QI_Item item))
                {
                    foreach (var animal in possibleAnimals)
                    {
                        for (int i = 0; i < animal.animalReason.Count; i++)
                        {
                            if (item.Data == animal.animalReason[i])
                            {
                                if (!animals.ContainsKey(animal))
                                    animals.Add(animal, 1);
                                else
                                    animals[animal]++;
                            }
                        }
                    }
                }
            }
        }
        // animal can be spawned
        foreach (var animal in possibleAnimals)
        {
            if (animals.TryGetValue(animal, out int i))
            {

                if (i >= animal.reasonAmountNeeded)
                {
                    // get chance to spawn animal
                    if (animal.spawnChanceCurve.Evaluate(UnityEngine.Random.Range(0.0f, 1.0f)) < 0.1f)
                    {
                        var go = Instantiate(animal.animalPrefab, plant.transform.position, Quaternion.identity);

                        if (go.TryGetComponent(out IAnimal thisAnimal))
                        {
                            thisAnimal.SetHome(plant.transform);
                        }

                      
                    }
                }
            }

        }
    }
    
}
