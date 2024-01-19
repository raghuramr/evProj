using Newtonsoft.Json;

namespace AppHealthCheck.Models
{
    public class MailRecepientsModel
    {
        [JsonProperty("To")]
        public string ToList { get; set; }

        [JsonProperty("CC")]
        public string CCList { get; set; }
    }
}