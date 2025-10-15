using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model;
using ASMS.Services.Model.ProductType;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Services
{
    public class ProductTypeService : IProductTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ProductTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ProductType> AddProductTypeAsync(CreateTypeRequest newType)
        {
            var type = _mapper.Map<ProductType>(newType);
            await _unitOfWork.ProductType.AddAsync(type);
            await _unitOfWork.CompleteAsync();
            return type;
        }

        public async Task<ProductType?> GetByIdAsync(int id)
        {
            var type = await _unitOfWork.ProductType.GetEntityByIdAsync(id);
            if (type == null)
            {
                return null;
            }
            return type;
        }

        public async Task<ProductType> UpdateProductTypeAsync(ProductType productType)
        {
            await _unitOfWork.ProductType.UpdateAsync(productType);
            await _unitOfWork.CompleteAsync();
            return productType;
        }
    }
}
