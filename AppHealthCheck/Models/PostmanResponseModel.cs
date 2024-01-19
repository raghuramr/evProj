using System.Text.Json.Serialization;

namespace AppHealthCheck.Models
{
    public class PostmanResponseModel
    {
        [JsonPropertyName("run")]
        public RunModel Run { get; set; }
    }
}