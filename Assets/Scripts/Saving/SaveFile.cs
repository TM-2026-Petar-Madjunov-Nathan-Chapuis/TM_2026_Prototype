using System;
using System.Collections.Generic;

namespace TM.Saving
{
    [System.Serializable]
    public class SaveFile
    {
        public List<SaveData> saveData = new List<SaveData>();
        public DateTime createdAtUtc;
        public DateTime lastSavedAtUtc;
        public double totalPlaySeconds;
    }
    [System.Serializable]
    public class SaveData
    {
        public string UID;
        public string data;

        public SaveData(string UID, string data)
        {
            this.UID = UID;
            this.data = data;
        }
    }
}