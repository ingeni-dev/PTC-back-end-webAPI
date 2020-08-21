using Newtonsoft.Json;

namespace PTCwebApi.Models.PTCModels.Entities {
    public class CheckToolingInWare {
        [JsonProperty ("COUNT(1)")]
        public string Count { get; set; }
    }
}