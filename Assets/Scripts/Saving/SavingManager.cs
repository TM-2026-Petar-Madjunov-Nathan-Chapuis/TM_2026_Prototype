using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace TM.Saving
{
    public class SavingManager : MonoBehaviour
    {
        public string fileName;
        public void Save()
        {
                string path = Application.persistentDataPath + "/" + fileName + ".json";
                SaveFile saveFile = new SaveFile();
                MonoBehaviour[] monobehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                foreach (MonoBehaviour monobehavior in monobehaviours)
                {
                    if (monobehavior is ISaveable iSaveable)
                    {
                        saveFile.saveData.Add(new SaveData(iSaveable.UID, JsonConvert.SerializeObject(iSaveable.SaveData())));
                    }
                }
                string json = JsonConvert.SerializeObject(saveFile);
                File.WriteAllText(path, json);
                Debug.Log("Game saved at " + path);
        }
        public void Load()
        {
                string path = Application.persistentDataPath + "/" + fileName + ".json";
                if (!File.Exists(path))
                {
                    Debug.LogWarning("No save file found at " + path);
                    return;
                }

                string json = File.ReadAllText(path);
                SaveFile saveFile = JsonConvert.DeserializeObject<SaveFile>(json);
                if (saveFile == null || saveFile.saveData == null)
                {
                    Debug.LogError("Save file is invalid: " + path);
                    return;
                }

                MonoBehaviour[] monobehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                Dictionary<String, String> lookupTable = new Dictionary<string, string>();
                foreach (SaveData saveData in saveFile.saveData)
                {
                    lookupTable[saveData.UID] = saveData.data;
                }
                foreach (MonoBehaviour monoBehaviour in monobehaviours)
                {
                    if (monoBehaviour is ISaveable iSaveable)
                    {
                        if (lookupTable.TryGetValue(iSaveable.UID, out string data))
                        {
                            iSaveable.LoadData(data);
                        }
                    }
                }
                Debug.Log("GAme Loaded at " + path);
        }
    }
}