using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Distance
{
    public class DistanceCalculationResponse
    {
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public double DistanceInKm { get; set; }
        public int DistanceInMeters { get; set; }
        public int DurationInMinutes { get; set; }
        public int DurationInSeconds { get; set; }
        public decimal EstimatedCost { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
