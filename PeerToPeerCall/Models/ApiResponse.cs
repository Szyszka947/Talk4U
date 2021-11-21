using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PeerToPeerCall.Models
{
    public class ApiResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        [JsonIgnore]
        public int Code { get; set; }
        public Dictionary<string, List<string>> Data { get; set; } = new Dictionary<string, List<string>>();

    }
}
