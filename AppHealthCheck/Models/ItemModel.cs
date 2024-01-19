using System.Text.Json.Serialization;

namespace AppHealthCheck.Models
{
    public class ItemModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}