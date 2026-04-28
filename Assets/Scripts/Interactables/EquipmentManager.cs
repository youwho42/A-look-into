using QuantumTek.QuantumInventory;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public EquipmentData[] currentEquipment;
    int totalSlots;
    public static EquipmentManager instance;
    public SpriteRenderer handEquipmentHolder;
    public SpriteRenderer lightEquipmentHolder;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
            
        totalSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        currentEquipment = new EquipmentData[totalSlots];
    }

    

    public void Equip(QI_ItemData newItem, int equipedIndex)
    {
        currentEquipment[equipedIndex] = newItem as EquipmentData;
        if (equipedIndex == (int)EquipmentSlot.Hands)
        {
            //handEquipmentHolder.sprite = newItem.EquipedItemImage;
            var go = Instantiate(newItem.EquipedItem, handEquipmentHolder.transform);
            go.transform.localPosition = Vector3.zero;
        }
        if (equipedIndex == (int)EquipmentSlot.Light)
        {
            var go = Instantiate(newItem.EquipedItem, lightEquipmentHolder.transform);
            go.transform.localPosition = Vector3.zero;
        }
        
        GameEventManager.onEquipmentUpdateEvent.Invoke();
    }
  
    public bool UnEquipToInventory(QI_ItemData itemData, int equipedIndex)
    {
       
        if(PlayerInformation.instance.playerInventory.AddItem(itemData, 1, false))
        {
            currentEquipment[equipedIndex] = null;
            
            if (equipedIndex == (int)EquipmentSlot.Hands)
            {
                Destroy(handEquipmentHolder.transform.GetChild(0).gameObject);
            }
            if (equipedIndex == (int)EquipmentSlot.Light)
            {
                Destroy(lightEquipmentHolder.transform.GetChild(0).gameObject);
            }
            GameEventManager.onEquipmentUpdateEvent.Invoke();
            return true;
        }
        return false;
    }

    public bool UnEquipAllToInventory()
    {
        for (int i = 0; i < currentEquipment.Length; i++)
        {

            if (currentEquipment[i] == null)
                continue;
        
            if (PlayerInformation.instance.playerInventory.AddItem(currentEquipment[i], 1, false))
            {
                currentEquipment[i] = null;

                if (i == (int)EquipmentSlot.Hands)
                {
                    Destroy(handEquipmentHolder.transform.GetChild(0).gameObject);
                }
                if (i == (int)EquipmentSlot.Light)
                {
                    Destroy(lightEquipmentHolder.transform.GetChild(0).gameObject);
                }
                GameEventManager.onEquipmentUpdateEvent.Invoke();
                return true;
            }
        }
        return false;
    }

    public bool UnequipBackpack(QI_ItemData itemData, int equipedIndex)
    {
        if(PlayerInformation.instance.playerInventory.Stacks.Count < 12)
        {
            if (PlayerInformation.instance.playerInventory.AddItem(itemData, 1, false))
            {
                currentEquipment[equipedIndex] = null;
                GameEventManager.onEquipmentUpdateEvent.Invoke();
                return true;
            }
        }
        return false;
    }

    public void UnEquipAndDestroy(int equipedIndex)
    {
        
        if (equipedIndex == (int)EquipmentSlot.Hands)
        {
            Destroy(handEquipmentHolder.transform.GetChild(0).gameObject);
        }
        currentEquipment[equipedIndex] = null;
        GameEventManager.onEquipmentUpdateEvent.Invoke();
    }
    public void UnEquipAndDestroyAll()
    {
        if(handEquipmentHolder.transform.childCount > 0)
            Destroy(handEquipmentHolder.transform.GetChild(0).gameObject);
        if (lightEquipmentHolder.transform.childCount > 0)
            Destroy(lightEquipmentHolder.transform.GetChild(0).gameObject);
        for (int i = 0; i < currentEquipment.Length; i++)
        {
            currentEquipment[i] = null;
        }
        
        
        GameEventManager.onEquipmentUpdateEvent.Invoke();
    }

    public bool HasItemEquipped(EquipmentSlot slot)
    {
        if (currentEquipment[(int)slot] != null)
            return true;
        return false;
    }

    public bool HasAnyItemEquipped()
    {
        
        foreach (var item in currentEquipment)
        {
            if (item != null)
                return true;
        }
        return false;
    }

    public EquipmentTier GetEquipmentTier(EquipmentSlot slot)
    {
        return currentEquipment[(int)slot].equipmentTier;
    }

    public bool HasCurrentHandEquipment(EquipmentData equipment)
    {
       return currentEquipment[(int)EquipmentSlot.Hands] == equipment;
    }
}
