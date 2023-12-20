using System.Text.Json.Serialization;

namespace AppSmokeTesting.Models
{
    public class Run
    {
        [JsonPropertyName("executions")]
        public Execution[] Executions { get; set; }
    }
}