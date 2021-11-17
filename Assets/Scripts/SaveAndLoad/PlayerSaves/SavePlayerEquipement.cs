using QuantumTek.EncryptedSave;
using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePlayerEquipement : SaveableManager
{

    QI_ItemDatabase allItemDatabase;
    EquipmentManager equipmentManager;

    List<string> names = new List<string>();
    List<int> index = new List<int>();


    private void Start()
    {
        allItemDatabase = AllItemsDatabaseManager.instance.allItemsDatabase;
        equipmentManager = EquipmentManager.instance;
    }

    public override void Save()
    {
        names.Clear();
        index.Clear();
        for (int i = 0; i < equipmentManager.currentEquipment.Length; i++)
        {
            if (equipmentManager.currentEquipment[i] != null)
            {
                names.Add(equipmentManager.currentEquipment[i].Name);
                index.Add(i);
            }

        }
        
        ES_Save.Save(names, saveableName + "name");
        ES_Save.Save(index, saveableName + "index");
    }
    public override void Load()
    {
        allItemDatabase ??= AllItemsDatabaseManager.instance.allItemsDatabase;
        names.Clear();
        index.Clear();
        names = ES_Save.Load<List<string>>(saveableName + "name");
        index = ES_Save.Load<List<int>>(saveableName + "index");
        EquipmentManager.instance.UnEquipAndDestroyAll();
        for (int i = 0; i < names.Count; i++)
        {

            EquipmentManager.instance.Equip(allItemDatabase.GetItem(names[i]), index[i]);

        }
    }

}
