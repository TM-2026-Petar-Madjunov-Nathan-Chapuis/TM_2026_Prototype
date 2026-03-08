using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using UnityEngine;

namespace TM.Saving
{
    public class SavingManager : MonoBehaviour
    {
        public string fileName;
        public void Save()
        {
            try
            {
                string path = Application.persistentDataPath + "/" + fileName + ".sav";
                SaveFile saveFile = new SaveFile();
                MonoBehaviour[] monobehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                foreach (MonoBehaviour monobehavior in monobehaviours)
                {
                    if (monobehavior is ISaveable iSaveable)
                    {
                        saveFile.saveData.Add(new SaveData(iSaveable.UID, JsonUtility.ToJson(iSaveable.SaveData(), true)));
                    }
                }
                string json = JsonUtility.ToJson(saveFile, true);
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
            try
            {
                string path = Application.persistentDataPath + "/" + fileName + ".sav";
                string json = File.ReadAllText(path);
                SaveFile saveFile = JsonUtility.FromJson<SaveFile>(json);
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
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}