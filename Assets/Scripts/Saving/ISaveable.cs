using System;

namespace TM.Saving
{
    public interface ISaveable
    {
        string UID { get; }
        public object SaveData();
        public void LoadData(string data);
    }
}
