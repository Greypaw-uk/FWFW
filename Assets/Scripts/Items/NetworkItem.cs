using Unity.Netcode;
using Unity.Collections;

public struct NetworkItem : INetworkSerializable
{
    public FixedString64Bytes Name;
    public float Weight;
    public float Price;
    public FixedString64Bytes IconPath;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer)
        where T : IReaderWriter
    {
        serializer.SerializeValue(ref Name);
        serializer.SerializeValue(ref Weight);
        serializer.SerializeValue(ref Price);
        serializer.SerializeValue(ref IconPath);
    }
}

