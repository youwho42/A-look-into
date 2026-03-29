using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokableItem : MonoBehaviour
{
    int totalAttemptedTimesPoked;
    public int TotalAttemptedTimesPoked { get { return totalAttemptedTimesPoked; } }

    int successfulTimesPoked;
    public int SuccessfulTimesPoked { get { return successfulTimesPoked; } }

    int failedTimesPoked;
    public int FailedTimesPoked { get { return failedTimesPoked; } }

    public MiniGameDificulty GameDificulty;
    
    public void SetTimesPoked(int attempts, int success, int fails)
    {
        totalAttemptedTimesPoked = attempts;
        successfulTimesPoked = success;
        failedTimesPoked = fails;
    }
    public virtual void PokeItemSuccess()
    {
        totalAttemptedTimesPoked++;
        successfulTimesPoked++;
    }

    public virtual void PokeItemFail()
    {
        totalAttemptedTimesPoked++;
        failedTimesPoked++;
    }
}
