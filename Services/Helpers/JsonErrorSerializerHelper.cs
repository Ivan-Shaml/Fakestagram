using Fakestagram.Services.Contracts;
using System.Text.Json;

namespace Fakestagram.Services.Helpers
{
    public class JsonErrorSerializerHelper : IJsonErrorSerializerHelper
    {
        public string Serialize(Exception ex)
        {
            return JsonSerializer.Serialize(new { errorMessage = ex.Message });
        }
    }
}
