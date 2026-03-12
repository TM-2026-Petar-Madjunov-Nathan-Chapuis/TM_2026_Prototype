using System.Collections.Generic;

namespace TM.Saving
{
    [System.Serializable]
    public class SaveFile
    {
        public List<SaveData> saveData = new List<SaveData>();
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