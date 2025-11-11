using ASMS.Repositories.Entities;
using ASMS.Services.Model.ContainerType;

namespace ASMS.Services.Interfaces
{
    public interface IContainerTypeService
    {
        Task<ContainerType> GetByIdAsync(int id);
        Task<ContainerType> AddContainerTypeAsync(CreateContainerTypeRequest request);
        Task<ContainerType> UpdateContainerTypeAsync(ContainerType newContainerType);
        Task<List<GetContainerTypeResponse>> GetAllAsync();
    }
}
