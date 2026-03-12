using System;
using Newtonsoft.Json;
using UnityEngine;

namespace TM.Saving
{
    [Serializable]
    public struct UUID : IEquatable<UUID>
    {
        [JsonProperty] private int uuid;

        public static UUID NewUUID()
        {
            int value = System.Security.Cryptography.RandomNumberGenerator.GetInt32(0, int.MaxValue);
            UUID uUID = new UUID
            {
                uuid = value
            };
            return uUID;
        }
        public bool Equals(UUID other)
        {
            return other.uuid == this.uuid;
        }

        public override bool Equals(object obj)  // override from Iquetable class
        {
            return obj is UUID other && Equals(other);
        }

        public override int GetHashCode()
        {
            return this.uuid.GetHashCode();
        }
    }
}