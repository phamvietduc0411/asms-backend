using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.Shelves;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Services
{
    public class ShelfService : IShelfService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ShelfService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ShelfResponse>> GetAllAsync()
        {
            var entities = await _unitOfWork.Shelves.GetAllAsync();
            return _mapper.Map<IEnumerable<ShelfResponse>>(entities);
        }

        public async Task<ShelfResponse?> GetByCodeAsync(string shelfCode)
        {
            var entity = await _unitOfWork.Shelves.GetByCodeAsync(shelfCode);
            return _mapper.Map<ShelfResponse>(entity);
        }

        public async Task<ShelfResponse> CreateAsync(CreateShelfRequest request)
        {
            var entity = _mapper.Map<Shelf>(request);
            await _unitOfWork.Shelves.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<ShelfResponse>(entity);
        }

        public async Task<ShelfResponse?> UpdateAsync(string shelfCode, UpdateShelfRequest request)
        {
            var existing = await _unitOfWork.Shelves.GetByCodeAsync(shelfCode);
            if (existing == null) return null;

            _mapper.Map(request, existing);
            await _unitOfWork.Shelves.UpdateAsync(existing);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ShelfResponse>(existing);
        }

        public async Task<bool> DeleteAsync(string shelfCode)
        {
            var entity = await _unitOfWork.Shelves.GetByCodeAsync(shelfCode);
            if (entity == null) return false;

            await _unitOfWork.Shelves.DeleteAsync(shelfCode);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
