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
            try
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
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        public void Load()
        {
            // try
            // {
                string path = Application.persistentDataPath + "/" + fileName + ".json";
                string json = File.ReadAllText(path);
                SaveFile saveFile = JsonConvert.DeserializeObject<SaveFile>(json);
                MonoBehaviour[] monobehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                Dictionary<String, String> lookupTable = new Dictionary<string, string>();
                foreach (SaveData saveData in saveFile.saveData)
                {
                    lookupTable.Add(saveData.UID, saveData.data);
                }
                foreach (MonoBehaviour monoBehaviour in monobehaviours)
                {
                    if (monoBehaviour is ISaveable iSaveable)
                    {
                        iSaveable.LoadData(lookupTable[iSaveable.UID]);
                    }
                }
                Debug.Log("GAme Loaded at " + path);
            // }
            // catch (Exception e)
            // {
            //     Debug.LogError(e);
            // }
        }
    }
}