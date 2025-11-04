using QuantumTek.QuantumInventory;
using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(QI_ItemDatabase))]
public class ItemDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draw the default inspector fields

        QI_ItemDatabase scriptableObject = (QI_ItemDatabase)target;

        if (GUILayout.Button("Get All Items"))
        {
            scriptableObject.Items.Clear();
            // Perform your custom context menu action here
            scriptableObject.Items = Resources.LoadAll<QI_ItemData>("Items/").ToList();
            scriptableObject.Items = scriptableObject.Items.OrderBy(x => x.Type).ThenBy(x => x.name).ToList();
        }
    }
}
