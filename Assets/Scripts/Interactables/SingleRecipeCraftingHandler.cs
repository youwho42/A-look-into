using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SingleRecipeCraftingHandler : MonoBehaviour
{
    public QI_Inventory EndInventory;
    public QI_Inventory StartInventory;

    public List<QI_CraftingQueue> Queues { get; set; } = new List<QI_CraftingQueue>();
    public UnityEvent OnCrafted;
    public UnityEvent OnCrafting;
    public UnityEvent OnCraftedFailed;

    private void Update()
    {
        if(Queues.Count > 0)
            Work(0);
        
    }

    private void Work(int index)
    {
        OnCrafting.Invoke();
        var queue = Queues[index];
        queue.Timer = Mathf.Clamp(queue.Timer - Time.deltaTime, 0, queue.Timer + Time.deltaTime);
        Queues[index] = queue;

        if (Mathf.Approximately(queue.Timer, 0))
        {
            if (!EndInventory.AddItem(queue.Item, queue.Amount, false))
            {
                Instantiate(queue.Item.ItemPrefab, transform.position + new Vector3(0, -0.2f, 0), Quaternion.identity);
            }
            Queues.RemoveAt(index);
            OnCrafted.Invoke();
        }
    }

    /// <summary>
    /// Adds a queue to the list of items to craft, from the given recipe. Returns if it was able to craft, based on available materials. Also removes those materials.
    /// </summary>
    /// <param name="recipe">The recipe to craft.</param>
    /// <param name="amount">How many times to craft this recipe.</param>
    public bool Craft(QI_CraftingRecipe recipe, int amount)
    {
        foreach (var ingredient in recipe.Ingredients)
            if (StartInventory.GetStock(ingredient.Item.Name) < ingredient.Amount * amount)
            {
                OnCraftedFailed.Invoke();
                return false;
            }


        foreach (var ingredient in recipe.Ingredients)
            StartInventory.RemoveItem(ingredient.Item, ingredient.Amount * amount);

        Queues.Add(new QI_CraftingQueue { Item = recipe.Product.Item, Amount = recipe.Product.Amount * amount, Timer = recipe.CraftingTime });
        return true;
    }
}
