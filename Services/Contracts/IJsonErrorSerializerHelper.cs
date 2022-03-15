namespace Fakestagram.Services.Contracts
{
    public interface IJsonErrorSerializerHelper
    {
        string Serialize(Exception ex);
    }
}
