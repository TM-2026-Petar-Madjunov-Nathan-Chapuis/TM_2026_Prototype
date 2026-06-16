using System.Collections.Generic;
using UnityEngine;
namespace TM.Saving
{
    public static class UuidFinder
    {
        private readonly static string path = "ScriptableObject/";
        public static ScriptableObject findUnique(UUID uUID)
        {
            ScriptableObject[] SOs = Resources.LoadAll<ScriptableObject>(path);
            foreach (ScriptableObject so in SOs)
            {
                if (so is IUUID iuuid)
                {
                    if (iuuid.Equals(uUID))
                    {
                        return so;
                    }
                }
            }
            Debug.LogError("Tried to find no existant UUID");
            return null;
        }
        public static Dictionary<UUID, ScriptableObject> findMultiple(UUID?[] uUIDs) // UUID might be null but we just dropp the null values and output the dict lookup containing only true UUID
        {
            Dictionary<UUID, ScriptableObject> lookup = getAllUUIDs();
            Dictionary<UUID, ScriptableObject> final = new Dictionary<UUID, ScriptableObject>();
            foreach (UUID? uID in uUIDs)
            {
                if (uID != null && !final.ContainsKey(uID.Value))
                {
                    final.Add(uID.Value, lookup[uID.Value]);
                }
            }
            return final;
        }
        public static Dictionary<UUID, ScriptableObject> getAllUUIDs()
        {
            ScriptableObject[] SOs = Resources.LoadAll<ScriptableObject>(path);
            Dictionary<UUID, ScriptableObject> lookup = new Dictionary<UUID, ScriptableObject>();
            foreach (ScriptableObject so in SOs)
            {
                if (so is IUUID iuuiD)
                {
                    lookup.Add(iuuiD.UUID, so);
                }
            }
            return lookup;
        }
    }
}