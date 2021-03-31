namespace GameCore.Serialization
{
    public interface ISerializable<T>
    {
        void SerializeTo(T serializer);

        void DeserializeFrom(T serializer);
    }
}
