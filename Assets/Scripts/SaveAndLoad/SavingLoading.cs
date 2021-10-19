using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SavingLoading : MonoBehaviour
{

    public static SavingLoading instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        else
            instance = this;
    }

    private string SavePath => $"{Application.persistentDataPath}/save.ali";
    
    [ContextMenu("Save")]
    public void Save()
    {
        var state = LoadFile();
        CaptureState(state);
        SaveFile(state);
    }

    [ContextMenu("Load")]
    public void Load()
    {
        var state = LoadFile();
        RestoreState(state);
        
    }

    private void SaveFile(object state)
    {
        
        using (var stream = File.Open(SavePath, FileMode.Create))
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, state);
        }
    }

    private Dictionary<string, object> LoadFile()
    {
        if (!File.Exists(SavePath))
        {
            return new Dictionary<string, object>();
        }
        
        using (FileStream stream = File.Open(SavePath, FileMode.Open))
        {
            var formatter = new BinaryFormatter();
            return (Dictionary<string, object>)formatter.Deserialize(stream);
        }
    }

    private void  CaptureState(Dictionary<string, object> state)
    {
        foreach (var saveable in FindObjectsOfType<SaveableEntity>())
        {
            state[saveable.ID] = saveable.CaptureState();
        }
    }

    private void RestoreState(Dictionary<string, object> state)
    {

        foreach (var saveable in FindObjectsOfType<SaveableEntity>())
        {
            if(state.TryGetValue(saveable.ID, out object value))
            {
                saveable.RestoreState(value);
            }
        }
        
    }

    public void DeleteFile()
    {
        File.Delete(SavePath);
    }
    
}
