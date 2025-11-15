using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Common;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.ShelfType;
using AutoMapper;

namespace ASMS.Services.Services
{
    public class ShelfTypeService : IShelfTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ShelfTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedList<GetShelfTypeResponse>> GetAllAsync(int pageNumber, int pageSize)
        {
            var result = await _unitOfWork.ShelvesTypes.GetAllAsync(pageNumber, pageSize);

            var mappedItems = _mapper.Map<List<GetShelfTypeResponse>>(result.Items);

            return new PaginatedList<GetShelfTypeResponse>(
                mappedItems,
                result.CurrentPage,
                result.PageSize,
                result.TotalRecords)
            {
                TotalPages = result.TotalPages
            };
        }

        public async Task<GetShelfTypeResponse> GetByIdAsync(int id)
        {
            var shelfType = await _unitOfWork.ShelvesTypes.GetByIdAsync(id);
            if (shelfType == null)
                return null;

            return _mapper.Map<GetShelfTypeResponse>(shelfType);
        }

        public async Task<GetShelfTypeResponse> CreateAsync(CreateShelfTypeRequest request)
        {
            var shelfType = _mapper.Map<ShelfType>(request);

            await _unitOfWork.ShelvesTypes.AddAsync(shelfType);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<GetShelfTypeResponse>(shelfType);
        }

        public async Task<GetShelfTypeResponse> UpdateAsync(int id, UpdateShelfTypeRequest request)
        {
            var shelfType = await _unitOfWork.ShelvesTypes.GetByIdAsync(id);
            if (shelfType == null)
                return null;

            _mapper.Map(request, shelfType);

            await _unitOfWork.ShelvesTypes.UpdateAsync(shelfType);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<GetShelfTypeResponse>(shelfType);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var shelfType = await _unitOfWork.ShelvesTypes.GetByIdAsync(id);
            if (shelfType == null)
                return false;

            await _unitOfWork.ShelvesTypes.DeleteAsync(shelfType);
            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}
