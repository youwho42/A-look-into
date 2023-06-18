
using QuantumTek.QuantumInventory;
using UnityEngine;

public class PlayerPurse : MonoBehaviour
{
    public QI_Currency currency;
    int purseAmount;

    private void Start()
    {
        SetPurseAmount(10);
    }

    public int GetPurseAmount()
    {
        return purseAmount;
    }
    public void SetPurseAmount(int amount)
    {
        purseAmount = amount;
        GameEventManager.onStatUpdateEvent.Invoke();
    }

    public void AddToPurse(int amount)
    {
        purseAmount += amount;
        GameEventManager.onStatUpdateEvent.Invoke();
    }

    public void RemoveFromPurse(int amount)
    {
        purseAmount -= amount;
        GameEventManager.onStatUpdateEvent.Invoke();
    }
}
