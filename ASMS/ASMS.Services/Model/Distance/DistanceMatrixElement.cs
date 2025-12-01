using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Distance
{
    public class DistanceMatrixElement
    {
        [JsonPropertyName("distance")]
        public DistanceInfo? Distance { get; set; }

        [JsonPropertyName("duration")]
        public DurationInfo? Duration { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
    }
}
