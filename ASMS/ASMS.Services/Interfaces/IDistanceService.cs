using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Services.Model.Distance;

namespace ASMS.Services.Interfaces
{
    public interface IDistanceService
    {
        Task<DistanceCalculationResponse> CalculateDistanceAsync(DistanceCalculationRequest request);
    }
}
