using System.Text.Json;
using System.Text.Json.Serialization;

public class Execution
{
    [JsonPropertyName("item")]
    public Item Item { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("response")]
    public Response Response { get; set; }

}
