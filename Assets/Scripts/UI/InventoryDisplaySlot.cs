using QuantumTek.QuantumInventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class InventoryDisplaySlot : MonoBehaviour
{
    public QI_ItemData item;
    public Image icon;
    
    public TextMeshProUGUI itemAmount;
    public QI_Inventory inventory;
    public EquipmentManager equipmentManager;

    
    public void ShowInformation()
    {
        if (item == null)
            return;
        ItemInformationDisplayUI.instance.ShowInformationDisplay(item);
    }
    public void HideInformation()
    {
        ItemInformationDisplayUI.instance.HideInformationDisplay();
    }
    
    public void UseItem()
    {
        if (item == null)
            return;
        item.UseItem();
        
    }

    public void AddItem(QI_ItemData newItem, int amount)
    {
        item = newItem;
        icon.sprite = item.Icon;
        
        itemAmount.text = amount.ToString();
        icon.enabled = true;
    }
    public void RemoveItem()
    {
        if (item == null)
            return;
        inventory.RemoveItem(item, 1);
    }
    public void DropItem(Vector3 position)
    {
        if (item == null)
            return;
        var go = Instantiate(item.ItemPrefab, position, Quaternion.identity);
        if(go.TryGetComponent(out SaveableItem entity))
        {
            entity.GenerateId();
        }
        if (go.TryGetComponent(out SaveableItem itemToDrop))
        {
            itemToDrop.GenerateId();
        }
        if (go.TryGetComponent(out ReplaceObjectOnItemDrop obj))
        {
            obj.CheckForObjects();
        }

        inventory.RemoveItem(item, 1);
    }



    public void DragItem()
    {
        
        icon.transform.localPosition = GetMousePosition();
    }

    public void EndDragItem()
    {
        //Check if in player vicinity :)
        if (CheckPlayerVicinity() && CheckForGameObjects())
        {
            DropItem(GetDropPosition());
            icon.transform.localPosition = Vector3.zero;
        }
        else
        {
            
            icon.transform.localPosition = Vector3.zero;
        }
    }

    Vector3 GetDropPosition()
    {
        Vector3 temp = Camera.main.ScreenToWorldPoint(icon.transform.position);
        temp.z = PlayerInformation.instance.player.position.z;
        return temp;
    }

    bool CheckForGameObjects()
    {
        var t = Physics2D.OverlapPoint(GetDropPosition());
        if (t == null)
            return true;

        if (t.CompareTag("Grass"))
            return true;
        if (t.CompareTag("Path"))
            return true;


        NotificationManager.instance.SetNewNotification("You can't place this here");
        return false;
    }

    bool CheckPlayerVicinity()
    {
        Vector3 playerPos = PlayerInformation.instance.player.position;
        
        float dist = Vector2.Distance(playerPos, GetDropPosition());
        if (dist <= 0.5f)
            return true;

        NotificationManager.instance.SetNewNotification("Try placing the " + item.Name + " closer to you.");
        return false;
    }
   
    Vector2 GetMousePosition()
    {
        Vector2 movePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform as RectTransform,
            Input.mousePosition, Camera.current,
            out movePos);
        return movePos;
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        itemAmount.text = "";
        icon.enabled = false;
    }
    Vector3 ItemDropOffset()
    {
        return new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f),0);
    }
}
