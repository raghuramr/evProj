using System.Text.Json;
using System.Text.Json.Serialization;

public class ExecutionModel
{
    [JsonPropertyName("item")]
    public ItemModel Item { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("response")]
    public ResponseModel Response { get; set; }

}
