using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model;
using ASMS.Services.Model.ContainerType;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Services
{
    public class ContainerTypeService : IContainerTypeService
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;

        public ContainerTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ContainerType> AddContainerTypeAsync(CreateContainerTypeRequest request)
        {
            var type = _mapper.Map<ContainerType>(request);
            await _unitOfWork.ContainerType.AddAsync(type);
            await _unitOfWork.CompleteAsync();
            return type;
        }

        public async Task<ContainerType> GetByIdAsync(int id)
        {
            var result = await _unitOfWork.ContainerType.GetEntityByIdAsync(id);
            if (result == null)
            {
                return null;
            }
            return result;
        }

        public async Task<ContainerType> UpdateContainerTypeAsync(ContainerType newContainerType)
        {
            await _unitOfWork.ContainerType.UpdateAsync(newContainerType);
            await _unitOfWork.CompleteAsync();
            return newContainerType;
        }


    }
}
