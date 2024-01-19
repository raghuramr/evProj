using System.Text.Json.Serialization;

namespace AppHealthCheck.Models
{
    public class RunModel
    {
        [JsonPropertyName("executions")]
        public ExecutionModel[] Executions { get; set; }
    }
}