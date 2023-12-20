using System.IO;
using System.Text.Json.Serialization;

public class Response
{
    public string id { get; set; }
    public string name { get; set; }
    public string status { get; set; }
    public int code { get; set; }
    [JsonPropertyName("stream")]
    public StreamData StreamData { get; set; }
}
