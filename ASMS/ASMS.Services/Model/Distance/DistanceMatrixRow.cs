using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Distance
{
    public class DistanceMatrixRow
    {
        [JsonPropertyName("elements")]
        public List<DistanceMatrixElement> Elements { get; set; } = new();
    }
}
