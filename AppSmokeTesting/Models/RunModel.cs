using System.Text.Json.Serialization;

namespace AppSmokeTesting.Models
{
    public class RunModel
    {
        [JsonPropertyName("executions")]
        public ExecutionModel[] Executions { get; set; }
    }
}