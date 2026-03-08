using TM.Saving;
using UnityEngine;

public class FunctionTester : MonoBehaviour, ISaveable
{
    public string UID { get; set; }

    private void Start()
    {
        UID = "test";
        FindAnyObjectByType<TM.Saving.SavingManager>().Save();
        FindAnyObjectByType<TM.Saving.SavingManager>().Load();
    }

    public object SaveData()
    {
        return "asdf";
    }

    public void LoadData(string data)
    {
        Debug.Log(data);
    }
}