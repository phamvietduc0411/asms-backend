using ASMS.Repositories.Entities;
using ASMS.Services.Model.Building;
using ASMS.Services.Model.ContainerType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Interfaces
{
    public interface IContainerTypeService
    {
        Task<ContainerType> GetByIdAsync(int id);
        Task<ContainerType> AddContainerTypeAsync(CreateContainerTypeRequest request);
        Task<ContainerType> UpdateContainerTypeAsync(ContainerType newContainerType);
    }
}
