using Newtonsoft.Json.Linq;
using System.Text.Json.Serialization;

namespace AppHealthCheck.Models
{
    public class StreamModel
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("data")]
        public int[] Data { get; set; }
    }
}