using System;

[Serializable]
public struct UUID : IEquatable<UUID>
{
    private int uuid;

    public static UUID NewUUID()
    {
        int value = System.Security.Cryptography.RandomNumberGenerator.GetInt32(0, int.MaxValue);
        UUID uUID = new UUID
        {
            uuid = value
        };
        return uUID;
    }
    public override bool Equals(UUID other)
    {
        return other.uuid == this.uuid;
    }
    public override int GetHashCode()
    {
        return this.uuid.GetHashCode();
    }

    public interface IUUID
    {
        UUID UUID { get; }
    }
}