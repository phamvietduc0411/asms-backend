using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.Container;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Services
{
    public class ContainerService : IContainerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ContainerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ContainerResponse>> GetAllAsync()
        {
            var containers = await _unitOfWork.Containers.GetAllAsync();
            return _mapper.Map<IEnumerable<ContainerResponse>>(containers);
        }

        public async Task<ContainerResponse?> GetByCodeAsync(string code)
        {
            var container = await _unitOfWork.Containers.GetByCodeAsync(code);
            return container == null ? null : _mapper.Map<ContainerResponse>(container);
        }

        public async Task<ContainerResponse> CreateAsync(CreateContainerRequest request)
        {
            var container = _mapper.Map<Container>(request);
            container.IsActive = true;

            await _unitOfWork.Containers.AddAsync(container);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ContainerResponse>(container);
        }

        public async Task<ContainerResponse?> UpdateAsync(string code, UpdateContainerRequest request)
        {
            var container = await _unitOfWork.Containers.GetByCodeAsync(code);
            if (container == null) return null;

            _mapper.Map(request, container);
            await _unitOfWork.Containers.UpdateAsync(container);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ContainerResponse>(container);
        }

        public async Task<bool> DeleteAsync(string code)
        {
            var container = await _unitOfWork.Containers.GetByCodeAsync(code);
            if (container == null) return false;

            await _unitOfWork.Containers.DeleteAsync(code);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
