using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class UuidFinder
{
    private readonly static string path = "Assets/ScriptableObject";
    public static finder(UUID uUID)
    {

    }
    private static string[] findAllSOFolders()
    {
        if (!Directory.Exists(path))
        {
            Debug.LogError("UuidFinder couldn't find the SO folder");
            return null;
        }
        else
        {
            return Directory.GetDirectories(path);
        }   
    }
}