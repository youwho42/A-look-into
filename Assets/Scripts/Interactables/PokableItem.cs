using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokableItem : MonoBehaviour
{
    int timesPoked;
    public int TimesPoked { get { return timesPoked; } }
    public MiniGameDificulty GameDificulty;

    public void SetTimesPoked(int times)
    {
        timesPoked = times;
    }
    public virtual void PokeItemSuccess()
    {
        timesPoked++;
    }

    public virtual void PokeItemFail()
    {
        timesPoked++;
    }
}
