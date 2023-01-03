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

    private string SavePath;
    //private string LoadPath => $"{Application.persistentDataPath}/{PlayerInformation.instance.playerName}_save.ali";
    private string LoadPath;

    
    public void Save()
    {
        SavePath = $"{Application.persistentDataPath}/{PlayerInformation.instance.playerName}_save.ali";
        DeleteFile();
        var state = LoadFile(LoadSelectionUI.instance.currentLoadFileName);
        CaptureState(state);
        SaveFile(state);
    }

   

    
    public void Load(string fileName)
    {
        var state = LoadFile(fileName);
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

    private Dictionary<string, object> LoadFile(string fileName)
    {
        LoadPath = $"{Application.persistentDataPath}/{fileName}_save.ali";
        if (!File.Exists(LoadPath))
        {
            return new Dictionary<string, object>();
        }
        
        using (FileStream stream = File.Open(LoadPath, FileMode.Open))
        {
            var formatter = new BinaryFormatter();
            return (Dictionary<string, object>)formatter.Deserialize(stream);
        }
    }

    private void CaptureState(Dictionary<string, object> state)
    {
        foreach (var saveableWorldEntity in FindObjectsOfType<SaveableWorldEntity>())
        {
            state[saveableWorldEntity.ID] = saveableWorldEntity.CaptureState();
        }
        foreach (var saveableItemEntity in FindObjectsOfType<SaveableItemEntity>())
        {
            state[saveableItemEntity.ID] = saveableItemEntity.CaptureState();
        }
        
    }

    private void RestoreState(Dictionary<string, object> state)
    {
        foreach (var saveableWorldEntity in FindObjectsOfType<SaveableWorldEntity>())
        {
            if(state.TryGetValue(saveableWorldEntity.ID, out object value))
            {
                saveableWorldEntity.RestoreState(value);
            }
        }
        foreach (var saveableItemEntity in FindObjectsOfType<SaveableItemEntity>())
        {
            if (state.TryGetValue(saveableItemEntity.ID, out object value))
            {
                saveableItemEntity.RestoreState(value);
            }
        }
    }

    public bool SaveExists()
    {
        return File.Exists(SavePath);
    }

    public void DeleteFile()
    {
        if(File.Exists(SavePath))
            File.Delete(SavePath);
    }
    
}
