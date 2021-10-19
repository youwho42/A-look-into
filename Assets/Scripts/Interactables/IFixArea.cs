using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFixArea
{
    void Fix(List<FixableAreaIngredient> ingredients);
}
