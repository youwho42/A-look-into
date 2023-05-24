using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFixArea
{
    bool Fix(List<FixableAreaIngredient> ingredients);
}
