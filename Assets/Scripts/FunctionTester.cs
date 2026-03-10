using TM.Saving;
using UnityEngine;

public class FunctionTester : MonoBehaviour, ISaveable
{
    public string UID => "testesrserser";

    private void Start()
    {
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