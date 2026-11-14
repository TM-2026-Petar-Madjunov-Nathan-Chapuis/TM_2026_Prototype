using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TM.Saving
{
    public class SaveSlotManager : MonoBehaviour
    {
        public static SaveSlotManager Instance { get; private set; }

        public string CurrentSaveFile { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) //cf. input manager
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject); //must persist through scenes.
        }

        public void SelectSaveFile(string fileName)
        {
            CurrentSaveFile = fileName;
            SceneManager.LoadScene(1);
        }
        public void DeleteSaveFile(string fileName)
        {
            string path = Application.persistentDataPath + "/" + fileName + ".json";

            if (File.Exists(path)) File.Delete(path);

            if (CurrentSaveFile == fileName) CurrentSaveFile = null;
        }
        public string[] GetAllSaveFiles()
        {
            string[] paths = Directory.GetFiles(Application.persistentDataPath, "*.json");
            string[] fileNames = new string[paths.Length];

            for (int i = 0; i < paths.Length; i++)
            {
                fileNames[i] = Path.GetFileNameWithoutExtension(paths[i]);
                Debug.Log(fileNames[i]);
            }
            return fileNames;
        }

        public SaveFile GetSaveFileInfo(string fileName)
        {
            string path = Application.persistentDataPath + "/" + fileName + ".json";

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                SaveFile saveFile = JsonConvert.DeserializeObject<SaveFile>(json);
                return saveFile;
            } 
            return null;
        }
    }
}