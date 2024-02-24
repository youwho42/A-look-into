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

        public void SaveGame()
        {
            SavePath = $"{Application.persistentDataPath}/{PlayerInformation.instance.playerName}_save.ali";
            DeleteFile(SavePath);
            var state = LoadFile(LoadSelectionUI.instance.currentLoadFileName);
            CaptureState(state);
            SaveFile(state);
            GameEventManager.onGameSavedEvent.Invoke();
        }

        public void SaveOptions()
        {
            OptionsPath = $"{Application.persistentDataPath}/Options_save.ali";
            DeleteFile(OptionsPath);
            var state = LoadFile("Options");
            CaptureOptions(state);
            SaveOptions(state);
        }



        public void LoadGame(string fileName)
        {
            var state = LoadFile(fileName);
            RestoreState(state);
        }

        public void LoadOptions()
        {
            var state = LoadFile("Options");
            RestoreOptions(state);
        }

        private void SaveFile(object state)
        {
            using (var stream = File.Open(SavePath, FileMode.Create))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, state);
            }
        }
        private void SaveOptions(object state)
        {
            using (var stream = File.Open(OptionsPath, FileMode.Create))
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
                if (state.TryGetValue(saveableWorldEntity.ID, out object value))
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



        public bool SaveExists()
        {
            return File.Exists(SavePath);
        }

        public void DeleteFile(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }

    }

}