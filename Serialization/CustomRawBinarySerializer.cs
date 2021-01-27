namespace GameCore.Serialization
{
    public abstract class CustomRawBinarySerializer
    {
        public abstract void Deserialize(byte[] bytes);

        public abstract byte[] Serialize();
    }
}
