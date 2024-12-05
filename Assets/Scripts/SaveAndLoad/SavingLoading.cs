using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Klaxon.SaveSystem
{
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

        string OptionsPath;
        public SaveableOptions saveableOptions;

        

        string VersionPath;
        public SaveableVersion saveableVersion;


        string VersionItemsPath;
        public SaveableVersionItems saveableVersionItems;

        public bool SaveGame()
        {
            SavePath = $"{Application.persistentDataPath}/{PlayerInformation.instance.playerName}_save.ali";
            string BackUpPath = $"{Application.persistentDataPath}/{PlayerInformation.instance.playerName}_saveBackUp.ali";
            
            if (File.Exists(SavePath))
                File.Copy(SavePath, BackUpPath);
                
            DeleteFile(SavePath);
            
            var state = LoadFile(LoadSelectionUI.instance.currentLoadFileName, "ali");
            CaptureState(state);
            SaveFile(SavePath, state);
            bool success = false;
            if (!File.Exists(SavePath))
                File.Copy(BackUpPath, SavePath);
            else
            {
                success = true;
                DeleteFile(BackUpPath);
            }
                
            GameEventManager.onGameSavedEvent.Invoke();
            SaveVersion();
            return success;
        }

        public void SaveOptions()
        {
            OptionsPath = $"{Application.persistentDataPath}/Options_save.alio";
            DeleteFile(OptionsPath);
            var state = LoadFile("Options", "alio");
            CaptureOptions(state);
            SaveFile(OptionsPath, state);
        }

        public void SaveVersion()
        {
            VersionPath = $"{Application.persistentDataPath}/{PlayerInformation.instance.playerName}Version_save.aliv";
            DeleteFile(VersionPath);
            var state = LoadFile($"{PlayerInformation.instance.playerName}Version", "aliv");
            CaptureVersion(state);
            SaveFile(VersionPath, state);
        }

        public void SaveVersionItems()
        {
            VersionItemsPath = $"{Application.persistentDataPath}/TempVersionItems_save.alit";
            DeleteFile(VersionItemsPath);
            var state = LoadFile("TempVersionItems", "alit");
            CaptureVersionItems(state);
            SaveFile(VersionItemsPath, state);
        }



        public void LoadGame(string fileName)
        {
            var state = LoadFile(fileName, "ali");
            RestoreState(state);
        }

        public void LoadOptions()
        {
            var optionsPath = $"{Application.persistentDataPath}/Options_save.alio";
            if (!File.Exists(optionsPath))
                SaveOptions();

            var state = LoadFile("Options","alio");
            RestoreOptions(state);
        }

        public void LoadVersion()
        {
            var state = LoadFile($"{PlayerInformation.instance.playerName}Version", "aliv");
            RestoreVersion(state);
        }

        public void LoadVersionItems()
        {
            var state = LoadFile("TempVersionItems", "alit");
            RestoreVersionItems(state);
        }

        private void SaveFile(string path, object state)
        {
            using (var stream = File.Open(path, FileMode.Create))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, state);
            }
        }
        
        private Dictionary<string, object> LoadFile(string fileName, string extention)
        {

            LoadPath = $"{Application.persistentDataPath}/{fileName}_save.{extention}";
            
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
            foreach (var saveableWorldEntity in FindObjectsByType<SaveableWorldEntity>(FindObjectsSortMode.None))
            {
                state[saveableWorldEntity.ID] = saveableWorldEntity.CaptureState();
            }
            foreach (var saveableItemEntity in FindObjectsByType<SaveableItemEntity>(FindObjectsSortMode.None))
            {
                state[saveableItemEntity.ID] = saveableItemEntity.CaptureState();
            }

        }
        

        private void RestoreState(Dictionary<string, object> state)
        {
            
            foreach (var saveableWorldEntity in FindObjectsByType<SaveableWorldEntity>(FindObjectsSortMode.None))
            {
                
                if (state.TryGetValue(saveableWorldEntity.ID, out object value))
                {
                    saveableWorldEntity.RestoreState(value);
                }
                
            }
            //ConsoleDebuggerUI.instance.SetDebuggerText("restoring file");
            foreach (var saveableItemEntity in FindObjectsByType<SaveableItemEntity>(FindObjectsSortMode.None))
            {
                if (state.TryGetValue(saveableItemEntity.ID, out object value))
                {
                    saveableItemEntity.RestoreState(value);
                }
            }
        }


        private void CaptureOptions(Dictionary<string, object> state)
        {
            state[saveableOptions.optionsName] = saveableOptions.CaptureState();
        }

        private void RestoreOptions(Dictionary<string, object> state)
        {
            if (state.TryGetValue(saveableOptions.optionsName, out object value))
            {
                saveableOptions.RestoreState(value);
            }
        }

        private void CaptureVersion(Dictionary<string, object> state)
        {
            state[saveableVersion.versionName] = saveableVersion.CaptureState();
        }

        private void RestoreVersion(Dictionary<string, object> state)
        {
            if (state.TryGetValue(saveableVersion.versionName, out object value))
            {
                saveableVersion.RestoreState(value);
            }
        }

        private void CaptureVersionItems(Dictionary<string, object> state)
        {
            state[saveableVersionItems.versionItemsName] = saveableVersionItems.CaptureState();
            foreach (var saveableItemEntity in FindObjectsByType<SaveableItemEntity>(FindObjectsSortMode.None))
            {
                if(saveableItemEntity.version != "")
                    state[saveableItemEntity.ID] = saveableItemEntity.CaptureState();
            }

        }

        private void RestoreVersionItems(Dictionary<string, object> state)
        {
            if (state.TryGetValue(saveableVersionItems.versionItemsName, out object value))
            {
                saveableVersionItems.RestoreState(value);
            }
            foreach (var saveableItemEntity in FindObjectsByType<SaveableItemEntity>(FindObjectsSortMode.None))
            {
                if (state.TryGetValue(saveableItemEntity.ID, out object itemValue))
                {
                    saveableItemEntity.RestoreState(itemValue);
                }
            }
        }

        public void DeleteFile(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }

    }

}