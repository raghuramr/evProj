using System.Text.Json.Serialization;

namespace AppSmokeTesting.Models
{
    public class PostmanResponseModel
    {
        [JsonPropertyName("run")]
        public RunModel Run { get; set; }
    }
}