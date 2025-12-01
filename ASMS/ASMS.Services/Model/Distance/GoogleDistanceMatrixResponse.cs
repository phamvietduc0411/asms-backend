using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Distance
{
    public class GoogleDistanceMatrixResponse
    {
        [JsonPropertyName("destination_addresses")]
        public List<string> DestinationAddresses { get; set; } = new();

        [JsonPropertyName("origin_addresses")]
        public List<string> OriginAddresses { get; set; } = new();

        [JsonPropertyName("rows")]
        public List<DistanceMatrixRow> Rows { get; set; } = new();

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
    }
}
