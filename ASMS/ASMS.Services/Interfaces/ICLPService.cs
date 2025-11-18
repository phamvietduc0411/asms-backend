using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Services.Model.CLP;

namespace ASMS.Services.Interfaces
{
    public interface ICLPService
    {
        Task<List<ContainerPlacementDto>> FindSuitableContainersAsync(FindContainerRequest request);
    }
}
