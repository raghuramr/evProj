using Newtonsoft.Json;

public class MailRecepientsModel
{
    [JsonProperty("To")]
    public string ToList { get; set; }

    [JsonProperty("CC")]
    public string CCList { get; set; }
}
