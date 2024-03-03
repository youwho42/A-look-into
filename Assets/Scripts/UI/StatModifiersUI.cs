using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StatModifiersUI : MonoBehaviour
{
    public GameObject modifierHolder;
    public ModifierDisplay modifierObject;
    public List<ModifierDisplay> modifiers = new List<ModifierDisplay>();

    private void Start()
    {
        ClearModifiers();
    }
    public void OnEnable()
    {
        GameEventManager.onStatUpdateEvent.AddListener(UpdateModifierUI);
    }
    public void OnDisable()
    {
        GameEventManager.onStatUpdateEvent.RemoveListener(UpdateModifierUI);
    }

    void UpdateModifierUI()
    {
        ClearModifiers();
        var stats = PlayerInformation.instance.statHandler;
        foreach (var stat in stats.statObjects)
        {
            foreach (var mod in stat.GetModifierList())
            {
                bool found = false;
                foreach (var obj in modifiers)
                {
                    if(obj.statModifier == mod)
                    {
                        found = true;
                        obj.SetModifierUI();
                        break;
                    }
                }
                if(!found && mod.modIcon != null)
                {
                    
                    var go = Instantiate(modifierObject, modifierHolder.transform);
                    go.statModifier = mod;
                    modifiers.Add(go);
                    go.SetModifierUI();
                }

            }
        }
    }

    void ClearModifiers()
    {
        modifiers.Clear();
        foreach (Transform child in modifierHolder.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
