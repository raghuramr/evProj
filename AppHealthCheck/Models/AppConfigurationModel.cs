using Newtonsoft.Json;

namespace AppHealthCheck.Models
{
    internal class AppConfigurationsModel
    {
        [JsonProperty("AppName")]
        public string AppName { get; set; }

        [JsonProperty("ApplicationName")]
        public string ApplicationName { get; set; }

        [JsonProperty("Environments")]
        public string[] Environments { get; set; }

        [JsonProperty("MailRecepients")]
        public MailRecepientsModel MailRecepients { get; set; }
    }
}