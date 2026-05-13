namespace StarCooperation
{
    public interface ISerializableMessagePayload
    {
        string Serialize();
        void Deserialize(string data);
        ISerializableMessagePayload CreateUninitializedInstance();
    }
}