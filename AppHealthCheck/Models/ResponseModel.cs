using System.Text.Json.Serialization;

namespace AppHealthCheck.Models
{
    public class ResponseModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("stream")]
        public StreamModel Stream { get; set; }

        [JsonPropertyName("responseTime")]
        public int ResponseTime { get; set; }
    }
}